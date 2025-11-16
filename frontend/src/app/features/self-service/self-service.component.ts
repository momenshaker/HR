import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthStore } from '@core/auth/auth.store';
import { finalize } from 'rxjs';
import { SelfServiceApiService } from './self-service.api';
import {
  DelegatedAuthority,
  EmployeeOrganizationSnapshot,
  LeaveRequest,
  LeaveType,
  SalarySlip,
  SelfServiceAccount,
  TrainingCourse
} from './self-service.models';

interface AdminConfig {
  showAttendanceCard: boolean;
  showPunchButton: boolean;
}

@Component({
  selector: 'app-self-service-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    MatListModule,
    MatSelectModule,
    MatSlideToggleModule
  ],
  templateUrl: './self-service.component.html',
  styleUrls: ['./self-service.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SelfServicePageComponent implements OnInit {
  private readonly authStore = inject(AuthStore);
  private readonly api = inject(SelfServiceApiService);
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);

  readonly user = computed(() => this.authStore.user());
  readonly employeeId = computed(() => this.user()?.employeeId ?? null);

  readonly leaveRequests = signal<LeaveRequest[]>([]);
  readonly snapshot = signal<EmployeeOrganizationSnapshot | null>(null);
  readonly authorities = signal<DelegatedAuthority[]>([]);
  readonly salarySlips = signal<SalarySlip[]>([]);
  readonly trainingCourses = signal<TrainingCourse[]>([]);
  readonly account = signal<SelfServiceAccount | null>(null);
  readonly lastAttendanceRecordId = signal<string | null>(null);
  readonly loading = signal(false);

  private lastLoadedEmployeeId: string | null = null;
  private readonly adminConfigKey = 'self-service-admin-config';
  private readonly defaultAdminConfig: AdminConfig = {
    showAttendanceCard: true,
    showPunchButton: true
  };

  readonly adminConfig = signal<AdminConfig>(this.defaultAdminConfig);
  readonly isAdmin = computed(() => (this.user()?.roles ?? []).includes('Admin'));
  readonly attendanceCardVisible = computed(() => this.adminConfig().showAttendanceCard);

  readonly leaveForm = this.fb.nonNullable.group({
    leaveTypeId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    reason: [''],
    attachmentPath: ['']
  });

  readonly accountForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    oauthProvider: ['local', Validators.required],
    externalIdentifier: ['', Validators.required],
    isMfaEnabled: [false],
    isLocked: [false],
    featureAccess: ['']
  });

  readonly leaveTypes = signal<LeaveType[]>([]);

  ngOnInit(): void {
    this.loadAdminConfig();
    this.loadLeaveTypes();
    effect(() => {
      const id = this.employeeId();
      if (!id) {
        this.resetSignals();
        this.lastLoadedEmployeeId = null;
        return;
      }
      if (id === this.lastLoadedEmployeeId) {
        return;
      }
      this.lastLoadedEmployeeId = id;
      this.loadEmployeeData(id);
    });
  }

  submitLeaveRequest(): void {
    if (this.leaveForm.invalid) {
      this.leaveForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeId();
    if (!employeeId) {
      return;
    }

    const raw = this.leaveForm.getRawValue();
    if (raw.startDate && raw.endDate && raw.startDate > raw.endDate) {
      this.snackbar.open('Start date must be before the end date.', 'Dismiss', { duration: 4000 });
      return;
    }

    this.loading.set(true);
    this.api
      .submitLeaveRequest(employeeId, {
        employeeId,
        leaveTypeId: raw.leaveTypeId,
        startDate: raw.startDate,
        endDate: raw.endDate,
        reason: raw.reason?.trim(),
        attachmentPath: raw.attachmentPath?.trim()
      })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: () => {
          this.snackbar.open('Leave request submitted.', 'Dismiss', { duration: 3000 });
          this.leaveForm.reset({
            leaveTypeId: '',
            startDate: '',
            endDate: '',
            reason: '',
            attachmentPath: ''
          });
          this.loadLeaveRequests(employeeId);
        },
        error: () => {
          this.snackbar.open('Unable to submit leave request.', 'Dismiss', { duration: 3000 });
        }
      });
  }

  recordAttendance(action: 'ClockIn' | 'ClockOut'): void {
    const employeeId = this.employeeId();
    if (!employeeId) {
      return;
    }

    if (action === 'ClockOut' && !this.lastAttendanceRecordId()) {
      this.snackbar.open('No attendance record available for clocking out.', 'Dismiss', { duration: 3000 });
      return;
    }

    const timestampUtc = new Date().toISOString();
    this.loading.set(true);
    const request =
      action === 'ClockIn'
        ? this.api.clockIn(employeeId, { timestampUtc, punchType: action })
        : this.api.clockOut(employeeId, this.lastAttendanceRecordId()!, { timestampUtc, punchType: action });

    request
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (record) => {
          this.snackbar.open(`${action === 'ClockIn' ? 'Clock-in' : 'Clock-out'} recorded.`, 'Dismiss', {
            duration: 3000
          });
          this.lastAttendanceRecordId.set(record.id);
        },
        error: (error: HttpErrorResponse) => {
          this.snackbar.open(
            error?.error?.title ?? `Unable to record ${action === 'ClockIn' ? 'clock-in' : 'clock-out'}.`,
            'Dismiss',
            { duration: 3000 }
          );
        }
      });
  }

  submitAccount(): void {
    if (this.accountForm.invalid) {
      this.accountForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeId();
    if (!employeeId) {
      return;
    }

    const raw = this.accountForm.getRawValue();
    const payload = {
      email: raw.email.trim(),
      oauthProvider: raw.oauthProvider.trim(),
      externalIdentifier: raw.externalIdentifier.trim(),
      isMfaEnabled: raw.isMfaEnabled,
      isLocked: raw.isLocked,
      lastSignInUtc: undefined,
      featureAccess: this.parseAccessList(raw.featureAccess)
    };

    const request$ = this.account()
      ? this.api.updateAccount(employeeId, payload)
      : this.api.createAccount(employeeId, { employeeId, ...payload });

    request$.subscribe({
      next: (account) => {
        this.snackbar.open('Account details saved.', 'Dismiss', { duration: 3000 });
        this.account.set(account);
      },
      error: () => {
        this.snackbar.open('Unable to save account details.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  removeAccount(): void {
    const employeeId = this.employeeId();
    if (!employeeId) {
      return;
    }

    this.api.deleteAccount(employeeId).subscribe({
      next: () => {
        this.snackbar.open('Self-service account deleted.', 'Dismiss', { duration: 3000 });
        this.account.set(null);
        this.accountForm.reset({
          email: '',
          oauthProvider: 'local',
          externalIdentifier: '',
          isMfaEnabled: false,
          isLocked: false,
          featureAccess: ''
        });
      },
      error: () => {
        this.snackbar.open('Unable to delete account.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private loadEmployeeData(employeeId: string): void {
    this.loadLeaveRequests(employeeId);
    this.loadOrganizationSnapshot(employeeId);
    this.loadDelegatedAuthorities(employeeId);
    this.loadSalarySlips(employeeId);
    this.loadTrainingCourses(employeeId);
    this.loadAccount(employeeId);
  }

  private loadLeaveTypes(): void {
    this.api.getLeaveTypes().subscribe({
      next: (types) => this.leaveTypes.set(types),
      error: () => this.snackbar.open('Failed to load leave types.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadLeaveRequests(employeeId: string): void {
    this.api.getLeaveRequests(employeeId).subscribe({
      next: (requests) => this.leaveRequests.set(requests),
      error: () => this.snackbar.open('Failed to load leave requests.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadOrganizationSnapshot(employeeId: string): void {
    this.api.getOrganizationSnapshot(employeeId).subscribe({
      next: (snapshot) => this.snapshot.set(snapshot),
      error: () => this.snackbar.open('Unable to load organisation snapshot.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadDelegatedAuthorities(employeeId: string): void {
    this.api.getDelegatedAuthorities(employeeId).subscribe({
      next: (authorities) => this.authorities.set(authorities),
      error: () => this.snackbar.open('Unable to load delegated authorities.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadSalarySlips(employeeId: string): void {
    this.api.getSalarySlips(employeeId).subscribe({
      next: (slips) => this.salarySlips.set(slips),
      error: () => this.snackbar.open('Unable to load salary slips.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadTrainingCourses(employeeId: string): void {
    this.api.getTrainingCourses(employeeId).subscribe({
      next: (courses) => this.trainingCourses.set(courses),
      error: () => this.snackbar.open('Unable to load training courses.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadAccount(employeeId: string): void {
    this.api.getAccount(employeeId).subscribe({
      next: (account) => {
        this.account.set(account);
        this.patchAccountForm(account);
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.account.set(null);
          this.accountForm.reset({
            email: '',
            oauthProvider: 'local',
            externalIdentifier: '',
            isMfaEnabled: false,
            isLocked: false,
            featureAccess: ''
          });
        } else {
          this.snackbar.open('Unable to load self-service account.', 'Dismiss', { duration: 3000 });
        }
      }
    });
  }

  private resetSignals(): void {
    this.leaveRequests.set([]);
    this.snapshot.set(null);
    this.authorities.set([]);
    this.salarySlips.set([]);
    this.trainingCourses.set([]);
    this.account.set(null);
    this.lastAttendanceRecordId.set(null);
  }

  private patchAccountForm(account: SelfServiceAccount): void {
    this.accountForm.setValue({
      email: account.email,
      oauthProvider: account.oauthProvider,
      externalIdentifier: account.externalIdentifier,
      isMfaEnabled: account.isMfaEnabled,
      isLocked: account.isLocked,
      featureAccess: (account.featureAccess ?? []).join(', ')
    });
  }

  private loadAdminConfig(): void {
    try {
      const stored = window.localStorage.getItem(this.adminConfigKey);
      if (stored) {
        const parsed = JSON.parse(stored) as Partial<AdminConfig>;
        this.adminConfig.set({ ...this.defaultAdminConfig, ...parsed });
      }
    } catch {
      // ignore malformed values
    }
  }

  private persistAdminConfig(config: AdminConfig): void {
    window.localStorage.setItem(this.adminConfigKey, JSON.stringify(config));
  }

  updateAdminConfig<K extends keyof AdminConfig>(key: K, value: AdminConfig[K]): void {
    const next = { ...this.adminConfig(), [key]: value };
    this.adminConfig.set(next);
    this.persistAdminConfig(next);
  }

  private parseAccessList(raw: string | null): string[] {
    if (!raw) {
      return [];
    }
    return raw
      .split(',')
      .map((entry) => entry.trim())
      .filter(Boolean);
  }
}
