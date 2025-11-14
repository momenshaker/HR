import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

interface TimesheetEntryDto {
  id: string;
  timesheetId: string;
  dateUtc: string;
  departmentId?: string | null;
  projectCode?: string | null;
  taskCode?: string | null;
  hours: number;
  description?: string | null;
}

interface TimesheetDto {
  id: string;
  employeeId: string;
  weekStartUtc: string;
  status: TimesheetStatus;
  submittedAtUtc?: string | null;
  approvedAtUtc?: string | null;
  managerId?: string | null;
  notes?: string | null;
  entries: TimesheetEntryDto[];
}

type TimesheetStatus = 'Draft' | 'Submitted' | 'Approved' | 'Rejected';

interface PaginatedResponse<T> {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}

@Component({
  selector: 'app-timesheets-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatSelectModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatSnackBarModule,
    MatProgressBarModule
  ],
  templateUrl: './timesheets.component.html',
  styleUrls: ['./timesheets.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TimesheetsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly snackbar = inject(MatSnackBar);

  private readonly timesheetsUrl = `${this.config.apiBaseUrl}/Timesheets`;
  private readonly approvalsUrl = `${this.config.apiBaseUrl}/TimesheetApprovals`;

  readonly statusOptions: TimesheetStatus[] = ['Draft', 'Submitted', 'Approved', 'Rejected'];

  readonly timesheetForm = this.fb.nonNullable.group({
    employeeId: ['', Validators.required],
    weekStart: ['', Validators.required]
  });

  readonly entryForm = this.fb.group({
    id: [''],
    dateUtc: ['', Validators.required],
    departmentId: [''],
    projectCode: [''],
    taskCode: [''],
    hours: [8, [Validators.required, Validators.min(0)]],
    description: ['']
  });

  readonly approvalsForm = this.fb.nonNullable.group({
    managerId: ['', Validators.required],
    status: ['Submitted'],
    notes: ['']
  });

  readonly currentTimesheet = signal<TimesheetDto | null>(null);
  readonly approvals = signal<readonly TimesheetDto[]>([]);
  readonly approvalsMeta = signal({ pageNumber: 1, pageSize: 20, totalCount: 0 });
  readonly timesheetLoading = signal(false);
  readonly approvalsLoading = signal(false);

  readonly entries = computed(() => this.currentTimesheet()?.entries ?? []);

  loadTimesheet(): void {
    if (this.timesheetForm.invalid) {
      this.timesheetForm.markAllAsTouched();
      return;
    }

    const { employeeId, weekStart } = this.timesheetForm.getRawValue();
    let params = new HttpParams().set('employeeId', employeeId).set('weekStart', weekStart);

    this.timesheetLoading.set(true);
    this.http.get<TimesheetDto>(this.timesheetsUrl, { params }).subscribe({
      next: (timesheet) => {
        this.currentTimesheet.set(timesheet);
        this.snackbar.open('Timesheet loaded.', 'Dismiss', { duration: 2500 });
        this.timesheetLoading.set(false);
      },
      error: () => {
        this.timesheetLoading.set(false);
        this.snackbar.open('Unable to load timesheet.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  editEntry(entry: TimesheetEntryDto): void {
    this.entryForm.patchValue({
      id: entry.id,
      dateUtc: entry.dateUtc?.substring(0, 10),
      departmentId: entry.departmentId ?? '',
      projectCode: entry.projectCode ?? '',
      taskCode: entry.taskCode ?? '',
      hours: entry.hours,
      description: entry.description ?? ''
    });
  }

  resetEntryForm(): void {
    this.entryForm.reset({
      id: '',
      dateUtc: '',
      departmentId: '',
      projectCode: '',
      taskCode: '',
      hours: 8,
      description: ''
    });
  }

  saveEntry(): void {
    const timesheet = this.currentTimesheet();
    if (!timesheet) {
      this.snackbar.open('Load a timesheet first.', 'Dismiss', { duration: 2500 });
      return;
    }
    if (this.entryForm.invalid) {
      this.entryForm.markAllAsTouched();
      return;
    }

    const payload = { ...this.entryForm.getRawValue(), hours: Number(this.entryForm.controls.hours.value ?? 0) };
    this.timesheetLoading.set(true);
    this.http.put<TimesheetEntryDto>(`${this.timesheetsUrl}/${timesheet.id}/entries`, payload).subscribe({
      next: () => {
        this.snackbar.open('Entry saved.', 'Dismiss', { duration: 2000 });
        this.resetEntryForm();
        this.loadTimesheet();
      },
      error: () => {
        this.timesheetLoading.set(false);
      }
    });
  }

  submitTimesheet(): void {
    const timesheet = this.currentTimesheet();
    if (!timesheet) {
      return;
    }
    this.timesheetLoading.set(true);
    this.http.post<TimesheetDto>(`${this.timesheetsUrl}/${timesheet.id}:submit`, {}).subscribe({
      next: (updated) => {
        this.snackbar.open('Timesheet submitted for approval.', 'Dismiss', { duration: 3000 });
        this.currentTimesheet.set(updated);
        this.timesheetLoading.set(false);
      },
      error: () => this.timesheetLoading.set(false)
    });
  }

  loadApprovals(): void {
    if (this.approvalsForm.invalid) {
      this.approvalsForm.markAllAsTouched();
      return;
    }
    const { managerId, status } = this.approvalsForm.getRawValue();
    let params = new HttpParams().set('managerId', managerId);
    if (status) {
      params = params.set('status', status);
    }

    this.approvalsLoading.set(true);
    this.http.get<PaginatedResponse<TimesheetDto>>(this.approvalsUrl, { params }).subscribe({
      next: (response) => {
        this.approvals.set(response.items);
        this.approvalsMeta.set({
          pageNumber: response.pageNumber,
          pageSize: response.pageSize,
          totalCount: response.totalCount
        });
        this.approvalsLoading.set(false);
      },
      error: () => {
        this.approvalsLoading.set(false);
        this.snackbar.open('Unable to load approvals.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  approve(timesheet: TimesheetDto): void {
    if (this.approvalsForm.invalid) {
      this.approvalsForm.markAllAsTouched();
      return;
    }
    const managerId = this.approvalsForm.controls.managerId.value;
    const notes = this.approvalsForm.controls.notes.value ?? '';
    this.http
      .post<TimesheetDto>(`${this.timesheetsUrl}/${timesheet.id}:approve`, {
        managerId,
        notes
      })
      .subscribe({
        next: () => {
          this.snackbar.open('Timesheet approved.', 'Dismiss', { duration: 2500 });
          this.loadApprovals();
          if (this.currentTimesheet()?.id === timesheet.id) {
            this.loadTimesheet();
          }
        }
      });
  }

  reject(timesheet: TimesheetDto): void {
    if (this.approvalsForm.invalid) {
      this.approvalsForm.markAllAsTouched();
      return;
    }
    const managerId = this.approvalsForm.controls.managerId.value;
    const reason = this.approvalsForm.controls.notes.value?.trim();
    if (!reason) {
      this.snackbar.open('Provide rejection reason in the notes field.', 'Dismiss', { duration: 3000 });
      return;
    }

    this.http
      .post<TimesheetDto>(`${this.timesheetsUrl}/${timesheet.id}:reject`, {
        managerId,
        reason
      })
      .subscribe({
        next: () => {
          this.snackbar.open('Timesheet rejected.', 'Dismiss', { duration: 2500 });
          this.loadApprovals();
          if (this.currentTimesheet()?.id === timesheet.id) {
            this.loadTimesheet();
          }
        }
      });
  }

  statusChipColor(status: TimesheetStatus): 'primary' | 'accent' | 'warn' | undefined {
    if (status === 'Approved') {
      return 'primary';
    }
    if (status === 'Submitted') {
      return 'accent';
    }
    if (status === 'Rejected') {
      return 'warn';
    }
    return undefined;
  }
}
