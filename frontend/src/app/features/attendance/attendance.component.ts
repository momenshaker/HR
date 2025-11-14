import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { DateRangePickerComponent } from '@shared/components/date-range-picker/date-range-picker.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { EntityCrudFactory } from '@core/data-access';

interface AttendancePunchDetail {
  id: string;
  type: string;
  timestampUtc: string;
  source: string;
  deviceId: string;
  location: string;
  notes: string;
}

interface AttendanceRecordDetail {
  id: string;
  employeeId: string;
  workDate: string;
  shiftName: string;
  scheduledStartTimeUtc?: string;
  scheduledEndTimeUtc?: string;
  checkInTimeUtc?: string;
  checkOutTimeUtc?: string;
  scheduledWorkMinutes: number;
  breakMinutes: number;
  gracePeriodMinutes: number;
  totalWorkedMinutes: number;
  lateMinutes: number;
  earlyLeaveMinutes: number;
  overtimeMinutes: number;
  absenceMinutes: number;
  status: string;
  source: string;
  remarks: string;
  punches: AttendancePunchDetail[];
  employeeName?: string;
}

interface AttendanceRecordListItem {
  id: string;
  employeeId: string;
  employeeName: string;
  workDate: string;
  status: string;
  checkIn: string;
  checkOut: string;
  events: string;
}

interface PunchInput {
  id?: string | null;
  type?: string;
  timestamp?: string;
  source?: string;
  deviceId?: string;
  location?: string;
  notes?: string;
}

interface PunchPayload {
  id?: string;
  type: string;
  timestampUtc: string;
  source: string;
  deviceId: string;
  location: string;
  notes: string;
}

interface AttendanceMutationRequest {
  employeeId: string;
  workDate: string;
  shiftName: string;
  scheduledStartTimeUtc?: string;
  scheduledEndTimeUtc?: string;
  checkInTimeUtc?: string;
  checkOutTimeUtc?: string;
  scheduledWorkMinutes: number;
  breakMinutes: number;
  gracePeriodMinutes: number;
  totalWorkedMinutes: number;
  lateMinutes: number;
  earlyLeaveMinutes: number;
  overtimeMinutes: number;
  absenceMinutes: number;
  status: string;
  source: string;
  remarks: string;
  punches: PunchPayload[];
}

type PunchControlGroup = FormGroup<{
  id: FormControl<string | null>;
  type: FormControl<string | null>;
  timestamp: FormControl<string | null>;
  source: FormControl<string | null>;
  deviceId: FormControl<string | null>;
  location: FormControl<string | null>;
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
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatListModule,
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
  private readonly dialog = inject(MatDialog);
  private readonly service = inject(EntityCrudFactory).create<AttendanceMutationRequest, AttendanceMutationRequest, AttendanceRecordDetail>(
    'AttendanceRecords'
  );
  private readonly employeeService = inject(EntityCrudFactory).create<never, never, EmployeeSummary>('employees');

  readonly statusOptions = ['InProgress', 'Completed', 'Absent'] as const;

  readonly filterForm = this.fb.nonNullable.group({
    start: [''],
    end: [''],
    employeeId: [''],
    status: ['']
  });

  readonly attendanceForm = this.fb.group({
    employeeId: ['', Validators.required],
    workDate: ['', Validators.required],
    shiftName: [''],
    status: ['InProgress', Validators.required],
    source: [''],
    remarks: [''],
    scheduledStartTime: [''],
    scheduledEndTime: [''],
    checkInTime: [''],
    checkOutTime: [''],
    scheduledWorkMinutes: [0, Validators.min(0)],
    breakMinutes: [0, Validators.min(0)],
    gracePeriodMinutes: [0, Validators.min(0)],
    totalWorkedMinutes: [0, Validators.min(0)],
    lateMinutes: [0, Validators.min(0)],
    earlyLeaveMinutes: [0, Validators.min(0)],
    overtimeMinutes: [0, Validators.min(0)],
    absenceMinutes: [0, Validators.min(0)],
    punches: this.fb.array<PunchControlGroup>([])
  });

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly detailLoading = signal(false);
  readonly records = signal<ReadonlyArray<AttendanceRecordListItem>>([]);
  readonly total = signal(0);
  readonly editingRecordId = signal<string | null>(null);
  readonly selectedRecord = signal<AttendanceRecordDetail | null>(null);
  readonly isEditing = computed(() => this.editingRecordId() !== null);

  employees: EmployeeSummary[] = [];
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });
  private readonly employeeNameMap = new Map<string, string>();

  readonly columns = {
    employeeName: 'Employee',
    workDate: 'Date',
    checkIn: 'Check-in',
    checkOut: 'Check-out',
    status: 'Status',
    events: 'Punches',
    actions: 'Actions'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.resetFormState();
    this.loadRecords(this.querySignal());
    this.filterForm.valueChanges.subscribe(() => this.onFiltersChanged());
    this.loadEmployees();
  }

  get punchControls(): FormArray<PunchControlGroup> {
    return this.attendanceForm.controls.punches as FormArray<PunchControlGroup>;
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.loadRecords(query);
  }

  onFiltersChanged(): void {
    this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
    this.loadRecords(this.querySignal());
  }

  refresh(): void {
    this.snackbar.open('Attendance refreshed', 'Dismiss', { duration: 2000 });
    this.loadRecords(this.querySignal());
  }

  addPunch(punch?: PunchInput): void {
    this.punchControls.push(
      this.fb.group({
        id: [punch?.id ?? null],
        type: [punch?.type ?? 'ClockIn', Validators.required],
        timestamp: [punch?.timestamp ?? '', Validators.required],
        source: [punch?.source ?? ''],
        deviceId: [punch?.deviceId ?? ''],
        location: [punch?.location ?? ''],
        notes: [punch?.notes ?? '']
      }) as PunchControlGroup
    );
  }

  removePunch(index: number): void {
    this.punchControls.removeAt(index);
  }

  submit(): void {
    if (this.attendanceForm.invalid) {
      this.attendanceForm.markAllAsTouched();
      return;
    }

    const punches = this.buildPunchPayloads();
    if (!punches.length) {
      this.snackbar.open('Please add at least one punch event.', 'Dismiss', { duration: 3000 });
      return;
    }

    const payload = this.buildMutationPayload(punches);
    this.saving.set(true);
    const action = this.editingRecordId()
      ? this.service.update(this.editingRecordId()!, payload)
      : this.service.create(payload);

    action.subscribe({
      next: () => {
        this.snackbar.open(
          this.editingRecordId() ? 'Attendance record updated' : 'Attendance recorded',
          'Dismiss',
          { duration: 3000 }
        );
        this.saving.set(false);
        this.editingRecordId.set(null);
        this.selectedRecord.set(null);
        this.resetFormState();
        this.loadRecords(this.querySignal());
      },
      error: () => this.saving.set(false)
    });
  }

  viewRecord(record: AttendanceRecordListItem): void {
    this.fetchRecord(record.id);
  }

  editRecord(record: AttendanceRecordListItem | AttendanceRecordDetail): void {
    this.fetchRecord(record.id, true);
  }

  cancelEditing(): void {
    this.editingRecordId.set(null);
    this.resetFormState();
  }

  confirmDelete(record: AttendanceRecordListItem): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete attendance record',
          message: `Remove attendance for ${record.employeeName} on ${record.workDate}?`
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }
        this.loading.set(true);
        this.service.delete(record.id).subscribe({
          next: () => {
            this.snackbar.open('Attendance record deleted', 'Dismiss', { duration: 3000 });
            if (this.editingRecordId() === record.id) {
              this.cancelEditing();
            }
            if (this.selectedRecord()?.id === record.id) {
              this.selectedRecord.set(null);
            }
            this.loadRecords(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private loadRecords(query: DataTableQuery): void {
    this.loading.set(true);
    const { start, end, employeeId, status } = this.filterForm.getRawValue();
    this.service
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection,
        filters: {
          start: start || undefined,
          end: end || undefined,
          employeeId: employeeId || undefined,
          status: status || undefined
        }
      })
      .subscribe({
        next: (response) => {
          const mapped = (response.data ?? []).map((record) => this.toListItem(record));
          this.records.set(mapped);
          this.total.set(response.meta?.totalItems ?? mapped.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  private fetchRecord(id: string, edit = false): void {
    this.detailLoading.set(true);
    this.service.getById(id).subscribe({
      next: (record) => {
        const enriched = { ...record, employeeName: this.getEmployeeLabel(record.employeeId) };
        this.selectedRecord.set(enriched);
        if (edit) {
          this.editingRecordId.set(enriched.id);
          this.populateForm(enriched);
        }
        this.detailLoading.set(false);
      },
      error: () => {
        this.detailLoading.set(false);
        this.snackbar.open('Failed to load attendance record', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private populateForm(record: AttendanceRecordDetail): void {
    this.attendanceForm.reset({
      employeeId: record.employeeId,
      workDate: record.workDate,
      shiftName: record.shiftName,
      status: record.status,
      source: record.source,
      remarks: record.remarks,
      scheduledStartTime: this.toLocal(record.scheduledStartTimeUtc),
      scheduledEndTime: this.toLocal(record.scheduledEndTimeUtc),
      checkInTime: this.toLocal(record.checkInTimeUtc),
      checkOutTime: this.toLocal(record.checkOutTimeUtc),
      scheduledWorkMinutes: record.scheduledWorkMinutes,
      breakMinutes: record.breakMinutes,
      gracePeriodMinutes: record.gracePeriodMinutes,
      totalWorkedMinutes: record.totalWorkedMinutes,
      lateMinutes: record.lateMinutes,
      earlyLeaveMinutes: record.earlyLeaveMinutes,
      overtimeMinutes: record.overtimeMinutes,
      absenceMinutes: record.absenceMinutes
    });
    this.punchControls.clear();
    (record.punches ?? []).forEach((punch) =>
      this.addPunch({
        id: punch.id,
        type: punch.type,
        timestamp: this.toLocal(punch.timestampUtc),
        source: punch.source,
        deviceId: punch.deviceId,
        location: punch.location,
        notes: punch.notes
      })
    );
    this.ensurePunch(0);
  }

  private resetFormState(): void {
    this.attendanceForm.reset({
      employeeId: '',
      workDate: '',
      shiftName: '',
      status: 'InProgress',
      source: '',
      remarks: '',
      scheduledStartTime: '',
      scheduledEndTime: '',
      checkInTime: '',
      checkOutTime: '',
      scheduledWorkMinutes: 0,
      breakMinutes: 0,
      gracePeriodMinutes: 0,
      totalWorkedMinutes: 0,
      lateMinutes: 0,
      earlyLeaveMinutes: 0,
      overtimeMinutes: 0,
      absenceMinutes: 0
    });
    this.punchControls.clear();
    this.ensurePunch(0);
  }

  private ensurePunch(index: number): void {
    if (this.punchControls.length <= index) {
      this.addPunch();
    }
  }

  private loadEmployees(): void {
    this.employeeService.list({ page: 1, pageSize: 250 }).subscribe({
      next: (response) => {
        const items = response.data ?? [];
        this.employees = items;
        this.employeeNameMap.clear();
        items.forEach((employee) => this.employeeNameMap.set(employee.id, employee.fullName));
      },
      error: () => {
        this.snackbar.open('Failed to load employees', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private toListItem(record: AttendanceRecordDetail): AttendanceRecordListItem {
    const punches = record.punches ?? [];
    return {
      id: record.id,
      employeeId: record.employeeId,
      employeeName: this.getEmployeeLabel(record.employeeId),
      workDate: this.formatDate(record.workDate),
      status: record.status,
      checkIn: this.formatTime(record.checkInTimeUtc),
      checkOut: this.formatTime(record.checkOutTimeUtc),
      events: punches.map((punch) => `${punch.type} @ ${this.formatTime(punch.timestampUtc)}`).join(', ')
    };
  }

  private getEmployeeLabel(employeeId: string): string {
    return this.employeeNameMap.get(employeeId) ?? employeeId;
  }

  private formatDate(value?: string): string {
    if (!value) {
      return '';
    }
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value : date.toLocaleDateString();
  }

  formatTime(value?: string): string {
    if (!value) {
      return '—';
    }
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '—';
    }
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  private buildPunchPayloads(): PunchPayload[] {
    const rawPunches = this.punchControls.getRawValue() as PunchInput[];
    return rawPunches
      .map((entry) => {
        const type = entry.type?.trim();
        if (!type) {
          return null;
        }
        const timestamp = this.toIsoString(entry.timestamp ?? undefined);
        if (!timestamp) {
          return null;
        }
        return {
          id: entry.id || undefined,
          type,
          timestampUtc: timestamp,
          source: entry.source?.trim() ?? '',
          deviceId: entry.deviceId?.trim() ?? '',
          location: entry.location?.trim() ?? '',
          notes: entry.notes?.trim() ?? ''
        } as PunchPayload;
      })
      .filter((entry): entry is PunchPayload => entry !== null);
  }

  private buildMutationPayload(punches: PunchPayload[]): AttendanceMutationRequest {
    const formValue = this.attendanceForm.getRawValue();
    const safeString = (value?: string | null): string => value ?? '';
    const safeNumber = (value?: number | null): number => value ?? 0;
    return {
      employeeId: safeString(formValue.employeeId),
      workDate: safeString(formValue.workDate),
      shiftName: safeString(formValue.shiftName),
      scheduledStartTimeUtc: this.toIsoString(formValue.scheduledStartTime),
      scheduledEndTimeUtc: this.toIsoString(formValue.scheduledEndTime),
      checkInTimeUtc: this.toIsoString(formValue.checkInTime),
      checkOutTimeUtc: this.toIsoString(formValue.checkOutTime),
      scheduledWorkMinutes: safeNumber(formValue.scheduledWorkMinutes),
      breakMinutes: safeNumber(formValue.breakMinutes),
      gracePeriodMinutes: safeNumber(formValue.gracePeriodMinutes),
      totalWorkedMinutes: safeNumber(formValue.totalWorkedMinutes),
      lateMinutes: safeNumber(formValue.lateMinutes),
      earlyLeaveMinutes: safeNumber(formValue.earlyLeaveMinutes),
      overtimeMinutes: safeNumber(formValue.overtimeMinutes),
      absenceMinutes: safeNumber(formValue.absenceMinutes),
      status: safeString(formValue.status) || 'InProgress',
      source: safeString(formValue.source),
      remarks: safeString(formValue.remarks),
      punches
    };
  }

  private toLocal(value?: string | null): string {
    if (!value) {
      return '';
    }
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '';
    }
    const offset = date.getTimezoneOffset() * 60000;
    return new Date(date.getTime() - offset).toISOString().slice(0, 16);
  }

  private toIsoString(value?: string | null): string | undefined {
    if (!value) {
      return undefined;
    }
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return undefined;
    }
    return date.toISOString();
  }
}
