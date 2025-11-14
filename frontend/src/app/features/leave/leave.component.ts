import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  effect,
  inject,
  signal
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { AuthStore } from '@core/auth/auth.store';
import { EntityCrudFactory } from '@core/data-access';
import { LeaveApiService, LeaveBalance, LeaveRequest, LeaveRequestFilters, LeaveType } from './leave.api';
import { LeaveActionDialogComponent } from './leave-action-dialog.component';

type DisplayedLeaveStatus = 'Draft' | 'PendingApproval' | 'Approved' | 'Rejected' | 'Cancelled';

interface EmployeeSummary {
  id: string;
  fullName: string;
}

@Component({
  selector: 'app-leave-requests-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule,
    MatDialogModule,
    LeaveActionDialogComponent,
    DataTableComponent
  ],
  templateUrl: './leave.component.html',
  styleUrls: ['./leave.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LeaveRequestsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  private readonly leaveApi = inject(LeaveApiService);
  private readonly authStore = inject(AuthStore);
  private readonly employeeRequester = inject(EntityCrudFactory).create<never, never, EmployeeSummary>('employees');

  readonly requestForm = this.fb.nonNullable.group({
    typeId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    reason: ['']
  });

  readonly filterForm = this.fb.nonNullable.group({
    status: ['']
  });

  readonly balanceYear = signal(new Date().getFullYear());
  readonly yearOptions = [new Date().getFullYear() - 1, new Date().getFullYear(), new Date().getFullYear() + 1];

  readonly loading = signal(false);
  readonly actionLoading = signal(false);
  readonly requests = signal<ReadonlyArray<LeaveRequest>>([]);
  readonly total = signal(0);
  readonly balances = signal<ReadonlyArray<LeaveBalance>>([]);
  readonly leaveTypes = signal<ReadonlyArray<LeaveType>>([]);
  readonly selectedRequest = signal<LeaveRequest | null>(null);
  readonly employeeNames = signal<Record<string, string>>({});
  readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly statuses: readonly DisplayedLeaveStatus[] = ['Draft', 'PendingApproval', 'Approved', 'Rejected', 'Cancelled'];
  readonly columns = {
    employee: 'Employee',
    type: 'Type',
    dates: 'Dates',
    days: 'Days',
    status: 'Status',
    actions: 'Actions'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  readonly isManager = computed(() => this.authStore.roles().includes('Manager'));
  readonly isApprover = computed(() =>
    this.authStore
      .roles()
      .some((role) => role === 'Manager' || role === 'HR' || role === 'Admin')
  );
  readonly currentEmployeeId = computed(() => this.authStore.user()?.employeeId ?? null);
  readonly leaveTypeMap = computed(() =>
    this.leaveTypes().reduce<Record<string, LeaveType>>((map, type) => {
      map[type.id] = type;
      return map;
    }, {})
  );

  ngOnInit(): void {
    this.loadLeaveTypes();
    this.loadEmployees();
    this.filterForm.valueChanges.subscribe(() => {
      this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
      this.loadRequests(this.querySignal());
    });
    effect(() => {
      const employeeId = this.currentEmployeeId();
      const year = this.balanceYear();
      if (!employeeId) {
        this.balances.set([]);
        return;
      }
      this.loadBalances(employeeId, year);
    });
    this.loadRequests(this.querySignal());
  }

  submitRequest(draft = false): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }
    const employeeId = this.currentEmployeeId();
    if (!employeeId) {
      this.snackbar.open('Employee profile missing, unable to submit leave.', 'Dismiss', { duration: 3000 });
      return;
    }
    const payload = this.buildPayload(draft, employeeId);
    if (!payload) {
      return;
    }
    this.actionLoading.set(true);
    this.leaveApi.createRequest(payload).subscribe({
      next: (request) => {
        this.requestForm.reset({ typeId: '', startDate: '', endDate: '', reason: '' });
        const message = draft ? 'Leave draft saved' : 'Leave request submitted';
        this.handleActionResult(request, message);
      },
      error: () => this.actionLoading.set(false)
    });
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.loadRequests(query);
  }

  refresh(): void {
    this.loadRequests(this.querySignal());
  }

  viewDetails(request: LeaveRequest): void {
    this.selectedRequest.set(request);
  }

  submitDraft(request: LeaveRequest): void {
    if (!this.isOwner(request)) {
      return;
    }
    const employeeId = this.currentEmployeeId();
    if (!employeeId) {
      this.snackbar.open('Employee profile missing, unable to submit request.', 'Dismiss', { duration: 3000 });
      return;
    }
    this.actionLoading.set(true);
    this.leaveApi.submitRequest(request.id, employeeId).subscribe({
      next: (updated) => this.handleActionResult(updated, 'Leave request submitted'),
      error: () => this.actionLoading.set(false)
    });
  }

  approve(request: LeaveRequest): void {
    if (!this.canApprove(request)) {
      return;
    }
    const managerId = this.currentEmployeeId();
    if (!managerId) {
      this.snackbar.open('Manager profile missing, unable to approve.', 'Dismiss', { duration: 3000 });
      return;
    }
    this.actionLoading.set(true);
    this.leaveApi.approveRequest(request.id, managerId).subscribe({
      next: (updated) => this.handleActionResult(updated, 'Leave request approved'),
      error: () => this.actionLoading.set(false)
    });
  }

  reject(request: LeaveRequest): void {
    if (!this.canApprove(request)) {
      return;
    }
    const managerId = this.currentEmployeeId();
    if (!managerId) {
      this.snackbar.open('Manager profile missing, unable to reject.', 'Dismiss', { duration: 3000 });
      return;
    }
    const dialogRef = this.dialog.open(LeaveActionDialogComponent, {
      width: '420px',
      data: {
        title: 'Reject leave request',
        message: `Provide a reason for rejecting ${this.getEmployeeLabel(request.employeeId)}’s leave.`,
        confirmLabel: 'Reject'
      }
    });
    dialogRef.afterClosed().subscribe((reason) => {
      if (!reason) {
        return;
      }
      this.actionLoading.set(true);
      this.leaveApi.rejectRequest(request.id, managerId, { reason }).subscribe({
        next: (updated) => this.handleActionResult(updated, 'Leave request rejected'),
        error: () => this.actionLoading.set(false)
      });
    });
  }

  cancel(request: LeaveRequest): void {
    if (!this.canCancel(request)) {
      return;
    }
    const employeeId = this.currentEmployeeId();
    if (!employeeId) {
      this.snackbar.open('Employee profile missing, unable to cancel.', 'Dismiss', { duration: 3000 });
      return;
    }
    this.actionLoading.set(true);
    this.leaveApi.cancelRequest(request.id, employeeId).subscribe({
      next: (updated) => this.handleActionResult(updated, 'Leave request cancelled'),
      error: () => this.actionLoading.set(false)
    });
  }

  setBalanceYear(year: number): void {
    this.balanceYear.set(year);
  }

  getStatusLabel(status: string): string {
    return status;
  }

  formatDate(value?: string | null): string {
    if (!value) {
      return '—';
    }
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value : date.toLocaleDateString();
  }

  previewDays(): number {
    const start = this.requestForm.controls.startDate.value;
    const end = this.requestForm.controls.endDate.value;
    if (!start || !end) {
      return 0;
    }
    const startDate = new Date(start);
    const endDate = new Date(end);
    if (Number.isNaN(startDate.getTime()) || Number.isNaN(endDate.getTime())) {
      return 0;
    }
    const diff = Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)) + 1;
    return diff > 0 ? diff : 0;
  }

  getEmployeeLabel(id: string): string {
    return this.employeeNames()[id] ?? id;
  }

  canApprove(request: LeaveRequest): boolean {
    return this.isManager() && request.status === 'PendingApproval';
  }

  canCancel(request: LeaveRequest): boolean {
    if (!this.isOwner(request)) {
      return false;
    }
    return ['PendingApproval', 'Approved', 'Draft'].includes(request.status);
  }

  canSubmitDraft(request: LeaveRequest): boolean {
    return this.isOwner(request) && request.status === 'Draft';
  }

  private isOwner(request: LeaveRequest): boolean {
    return request.employeeId === this.currentEmployeeId();
  }

  private loadLeaveTypes(): void {
    this.leaveApi.getTypes().subscribe({
      next: (types) => this.leaveTypes.set(types),
      error: () => this.snackbar.open('Failed to load leave types.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadEmployees(): void {
    this.employeeRequester
      .list({ page: 1, pageSize: 250 })
      .subscribe({
        next: (response) => {
          const map: Record<string, string> = {};
          (response.data ?? []).forEach((employee) => {
            map[employee.id] = employee.fullName;
          });
          this.employeeNames.set(map);
        },
        error: () => {
          this.snackbar.open('Failed to load employee directory.', 'Dismiss', { duration: 3000 });
        }
      });
  }

  private loadBalances(employeeId: string, year: number): void {
    this.leaveApi.getBalances(employeeId, year).subscribe({
      next: (balances) => this.balances.set(balances),
      error: () => this.snackbar.open('Failed to load leave balances.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadRequests(query: DataTableQuery): void {
    this.loading.set(true);
    this.leaveApi.listRequests(this.buildFilters(query)).subscribe({
      next: (page) => {
        this.requests.set(page.items);
        this.total.set(page.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private buildFilters(query: DataTableQuery): LeaveRequestFilters {
    const status = this.filterForm.controls.status.value;
    const filters: LeaveRequestFilters = {
      status: status || undefined,
      page: query.pageIndex + 1,
      pageSize: query.pageSize
    };
    const employeeId = this.currentEmployeeId();
    if (this.isManager()) {
      if (employeeId) {
        filters.managerId = employeeId;
      }
    } else if (!this.isApprover()) {
      if (employeeId) {
        filters.employeeId = employeeId;
      }
    }
    return filters;
  }

  private buildPayload(draft: boolean, employeeId: string) {
    const form = this.requestForm.getRawValue();
    return {
      employeeId,
      leaveTypeId: form.typeId,
      startDate: form.startDate,
      endDate: form.endDate,
      reason: form.reason?.trim() ?? undefined,
      draft
    };
  }

  private handleActionResult(result: LeaveRequest, message: string): void {
    this.snackbar.open(message, 'Dismiss', { duration: 3000 });
    this.selectedRequest.set(result);
    this.actionLoading.set(false);
    this.loadRequests(this.querySignal());
    const employeeId = this.currentEmployeeId();
    if (employeeId) {
      this.loadBalances(employeeId, this.balanceYear());
    }
  }
}
