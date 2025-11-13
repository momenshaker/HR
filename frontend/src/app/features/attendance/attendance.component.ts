import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { DateRangePickerComponent } from '@shared/components/date-range-picker/date-range-picker.component';
import { EntityCrudFactory } from '@core/data-access';

interface PunchView {
  id: string;
  type: string;
  timestampUtc: string;
  notes?: string;
}

interface AttendanceRecord {
  id: string;
  employeeName: string;
  date: string;
  status: string;
  punches?: PunchView[];
  events?: string;
  employeeId?: string;
  workDate?: string;
}

interface PunchInput {
  type: string;
  timestamp: string;
  notes?: string;
}

interface PunchPayload {
  type: string;
  timestampUtc: string;
  notes?: string;
}

interface EmployeeSummary {
  id: string;
  fullName: string;
}

interface CreateAttendanceRequest {
  employeeId: string;
  workDate: string;
  shiftName?: string;
  overtimeMinutes: number;
  status: string;
  notes?: string;
  punches: PunchPayload[];
}

type PunchControlGroup = FormGroup<{
  type: FormControl<string | null>;
  timestamp: FormControl<string | null>;
  notes: FormControl<string | null>;
}>;

interface EmployeeSummary {
  id: string;
  fullName: string;
}

@Component({
  selector: 'app-attendance-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    DataTableComponent,
    DateRangePickerComponent
  ],
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AttendancePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<CreateAttendanceRequest, never, AttendanceRecord>('attendance');
  private readonly employeeService = inject(EntityCrudFactory).create<never, never, EmployeeSummary>('employees');

  readonly filterForm = this.fb.nonNullable.group({
    start: [''],
    end: ['']
  });

  readonly createForm = this.fb.nonNullable.group({
    employeeId: ['', Validators.required],
    workDate: ['', Validators.required],
    shiftName: [''],
    overtimeMinutes: [0],
    status: ['InProgress'],
    notes: [''],
    punches: this.fb.array<PunchControlGroup>([])
  });

  readonly loading = signal(false);
  readonly records = signal<ReadonlyArray<AttendanceRecord>>([]);
  readonly total = signal(0);
  employees: EmployeeSummary[] = [];
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    employeeName: 'Employee',
    date: 'Date',
    status: 'Status',
    events: 'Events'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.load(this.querySignal());
    this.filterForm.valueChanges.subscribe(() => this.onFiltersChanged());
    this.loadEmployees();
    this.ensurePunch(0);
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  onFiltersChanged(): void {
    this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
    this.load(this.querySignal());
  }

  refresh(): void {
    this.snackbar.open('Attendance refreshed', 'Dismiss', { duration: 2000 });
    this.load(this.querySignal());
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    const { start, end } = this.filterForm.getRawValue();
    this.service
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection,
        filters: {
          start,
          end
        }
      })
      .subscribe({
        next: (response) => {
          const mapped = (response.data ?? []).map((record) => {
            const punches = record.punches ?? [];
            return {
              id: record.id,
              employeeName: (record as AttendanceRecord).employeeName ?? record.employeeId,
              date: record.workDate,
              status: record.status,
              punches,
              events: punches
                .map((punch) => `${punch.type} @ ${new Date(punch.timestampUtc).toLocaleTimeString()}`)
                .join(', ')
            } as AttendanceRecord;
          });
          this.records.set(mapped);
          this.total.set(response.meta?.totalItems ?? mapped.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  get punchControls(): FormArray<PunchControlGroup> {
    return this.createForm.controls.punches as FormArray<PunchControlGroup>;
  }

  addPunch(punch?: PunchInput): void {
    this.punchControls.push(
      this.fb.group({
        type: [punch?.type ?? 'ClockIn', Validators.required],
        timestamp: [punch?.timestamp ?? '', Validators.required],
        notes: [punch?.notes ?? '']
      }) as PunchControlGroup
    );
  }

  removePunch(index: number): void {
    this.punchControls.removeAt(index);
  }

  submit(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const raw = this.createForm.getRawValue();
    const rawPunches = this.punchControls.getRawValue() as PunchInput[];
    const punches = rawPunches
      .map((entry) => ({
        type: entry.type,
        timestamp: entry.timestamp,
        notes: entry.notes?.trim()
      }))
      .filter((entry) => entry.type && entry.timestamp)
      .map((entry) => ({
        type: entry.type,
        timestampUtc: new Date(entry.timestamp).toISOString(),
        notes: entry.notes
      }));

    if (!punches.length) {
      this.snackbar.open('Please add at least one punch event.', 'Dismiss', { duration: 3000 });
      return;
    }

    this.loading.set(true);
    this.service
      .create({
        employeeId: raw.employeeId,
        workDate: raw.workDate,
        shiftName: raw.shiftName,
        overtimeMinutes: raw.overtimeMinutes,
        status: raw.status,
        notes: raw.notes?.trim(),
        punches
      })
      .subscribe({
        next: () => {
          this.snackbar.open('Attendance recorded', 'Dismiss', { duration: 3000 });
          this.createForm.reset({
            employeeId: '',
            workDate: '',
            shiftName: '',
            overtimeMinutes: 0,
            status: 'InProgress',
            notes: '',
            punches: []
          });
          this.ensurePunch(0);
          this.load(this.querySignal());
        },
        error: () => this.loading.set(false)
      });
  }

  private ensurePunch(index: number): void {
    if (this.punchControls.length <= index) {
      this.addPunch();
    }
  }

  private loadEmployees(): void {
    this.employeeService
      .list({ page: 1, pageSize: 250 })
      .subscribe({
        next: (response) => {
          this.employees = response.data ?? [];
        },
        error: () => {
          this.snackbar.open('Failed to load employees', 'Dismiss', { duration: 3000 });
        }
      });
  }
}
