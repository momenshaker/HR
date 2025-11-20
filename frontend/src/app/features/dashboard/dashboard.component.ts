import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, signal } from '@angular/core';
import { forkJoin, firstValueFrom, of } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthStore } from '@core/auth/auth.store';
import { OrganizationContextService } from '@core/auth/organization-context.service';
import {
  DashboardApiService,
  HeadcountItem,
  PayrollTotalsResponse,
  TrainingCompliance
} from './dashboard.api';
import { LeaveApiService, LeaveRequestPayload } from '../leave/leave.api';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';
import { LeaveRequestDialogComponent } from './leave-request-dialog.component';
import { SelfServiceApiService } from '@app/features/self-service/self-service.api';
import { AttendanceRecord, LeaveRequest } from '@app/features/self-service/self-service.models';

interface DashboardMetric {
  title: string;
  value: string;
  change: string;
  description: string;
  icon: string;
}

interface DashboardInsight {
  label: string;
  value: string;
  detail: string;
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatGridListModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatButtonModule,
    MatDividerModule,
    MatNativeDateModule,
    LeaveRequestDialogComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardPageComponent {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly dashboardApi = inject(DashboardApiService);
  private readonly organizationContext = inject(OrganizationContextService);
  private readonly selfServiceApi = inject(SelfServiceApiService);
  private readonly leaveApi = inject(LeaveApiService);
  readonly user = inject(AuthStore).user;

  readonly metrics = signal<DashboardMetric[]>([
    { title: 'Total employees', value: '--', change: '', description: 'Active headcount', icon: 'groups' },
    { title: 'Open roles', value: '--', change: '', description: 'Hiring pipeline', icon: 'work_outline' },
    { title: 'Pending leave', value: '--', change: '', description: 'Awaiting approval', icon: 'event_note' }
  ]);

  readonly insights = signal<DashboardInsight[]>([
    { label: 'Payroll run', value: 'Loading...', detail: '' },
    { label: 'Training compliance', value: 'Loading...', detail: '' }
  ]);

  readonly focusItems = signal<string[]>([
    'Review payroll run for 18 Nov',
    'Approve leave for Sales team',
    'Verify timesheets flagged for overtime'
  ]);

  readonly loading = signal(true);
  readonly lastPunch = signal('Not recorded yet');
  readonly punchHistory = signal<string[]>([]);
  readonly lastAttendanceRecordId = signal<string | null>(null);
  private lastOrgId: string | null = null;

  constructor() {
    effect(
      () => {
        const orgId = this.organizationContext.organizationId();
        if (!orgId) {
          this.lastOrgId = null;
          this.loading.set(false);
          return;
        }
        if (this.lastOrgId === orgId) {
          return;
        }
        this.lastOrgId = orgId;
        this.fetchMetrics(orgId);
      },
      { allowSignalWrites: true }

    );

  }

  openLeaveDialog(): void {
    const dialogRef = this.dialog.open(LeaveRequestDialogComponent, {
      width: '420px'
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const employeeId = this.user()?.employeeId;
      if (!employeeId) {
        this.snackbar.open('Employee profile required to submit leave.', 'Dismiss', { duration: 3000 });
        return;
      }
      const payload: LeaveRequestPayload = {
        employeeId,
        leaveTypeId: result.typeId,
        startDate: this.toIsoDate(result.startDate),
        endDate: this.toIsoDate(result.endDate),
        reason: result.reason?.trim() ?? undefined,
        draft: false
      };
      this.leaveApi.createRequest(payload).subscribe({
        next: () => {
          this.snackbar.open('Leave request submitted.', 'Dismiss', { duration: 2500 });
          if (this.lastOrgId) {
            this.fetchMetrics(this.lastOrgId);
          }
        },
        error: () => {
          this.snackbar.open('Unable to submit leave request.', 'Dismiss', { duration: 3000 });
        }
      });
    });
  }

  clockIn(): void {
    void this.recordPunch('ClockIn');
  }

  clockOut(): void {
    void this.recordPunch('ClockOut');
  }

  private fetchMetrics(organizationId: string): void {
    this.loading.set(true);
    const { from, to } = this.monthRange();
    const employeeId = this.user()?.employeeId;
    if (employeeId) {
      void this.fetchOpenAttendanceRecord(employeeId);
    }
    const emptyLeaveResponse: PaginatedResponse<LeaveRequest> = { items: [], totalCount: 0 };
    const leaveRequests$ = employeeId
      ? this.selfServiceApi.getLeaveRequests(employeeId)
      : of(emptyLeaveResponse);
    forkJoin({
      headcount: this.dashboardApi.getHeadcount(organizationId),
      leaveRequests: leaveRequests$,
      payroll: this.dashboardApi.getPayrollTotals(organizationId, from, to),
      vacancies: this.dashboardApi.getVacancies(),
      training: this.dashboardApi.getTrainingCompliance(organizationId)
    }).subscribe({
      next: ({ headcount, leaveRequests, payroll, vacancies, training }) => {
        this.loading.set(false);
        debugger;
        this.updateMetrics(headcount.items, leaveRequests.items, vacancies.items);
        const payrollData = payroll.items[0] ?? { runs: [], byDepartment: [] };
        const trainingData =
          training.items[0] ?? { complianceRate: 0, compliantEmployeeCount: 0, observedEmployeeCount: 0, organizationId: '', mandatoryCourseCount: 0 };
        this.updateInsights(payrollData, trainingData);
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open('Unable to load dashboard metrics.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private updateMetrics(
    headcount: readonly HeadcountItem[],
    leaveRequests: readonly LeaveRequest[],
    vacancies: readonly { status: string; numberOfPositions: number }[]
  ): void {
    const totalEmployees = headcount.reduce((sum, item) => sum + item.count, 0);
    const pendingLeave = leaveRequests.filter((request) => request.status?.toLowerCase() === 'pending').length;
    const openPositions = vacancies.reduce((sum, vacancy) => {
      const closed = vacancy.status?.toLowerCase() === 'closed';
      return sum + (closed ? 0 : vacancy.numberOfPositions);
    }, 0);

    this.metrics.set([
      {
        title: 'Total employees',
        value: totalEmployees.toLocaleString(),
        change: '+4 headcount vs last week',
        description: 'Active headcount',
        icon: 'groups'
      },
      {
        title: 'Open roles',
        value: openPositions.toString(),
        change: openPositions > 0 ? `${openPositions} roles awaiting hires` : 'All roles filled',
        description: 'Hiring pipeline',
        icon: 'work_outline'
      },
      {
        title: 'Pending leave',
        value: pendingLeave.toString(),
        change: pendingLeave ? `${pendingLeave} requests need action` : 'None pending',
        description: 'Awaiting approval',
        icon: 'event_note'
      }
    ]);
  }

  private updateInsights(payroll: PayrollTotalsResponse, training: TrainingCompliance): void {
    const latestRun = payroll.runs[0];
    const payrollValue = latestRun
      ? `${latestRun.periodStart} � ${latestRun.periodEnd}`
      : 'Pending';
    const payrollDetail = latestRun
      ? new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(latestRun.totalNet)
      : 'Awaiting calculation';

    const complianceValue = training.complianceRate
      ? `${Math.round(training.complianceRate * 100)}%`
      : 'Awaiting training data';
    const complianceDetail = `${training.compliantEmployeeCount} / ${training.observedEmployeeCount} employees compliant`;

    this.insights.set([
      { label: 'Latest payroll', value: payrollValue, detail: payrollDetail },
      { label: 'Training compliance', value: complianceValue, detail: complianceDetail }
    ]);
  }

  private monthRange(): { from: string; to: string } {
    const now = new Date();
    const start = new Date(now.getFullYear(), now.getMonth(), 1);
    const end = new Date(now.getFullYear(), now.getMonth() + 1, 0);
    const from = start.toISOString().split('T')[0] ?? '';
    const to = end.toISOString().split('T')[0] ?? '';
    return { from, to };
  }

  private async recordPunch(action: 'ClockIn' | 'ClockOut'): Promise<void> {
    const employeeId = this.user()?.employeeId;
    if (!employeeId) {
      this.snackbar.open('Employee profile is required to record attendance.', 'Dismiss', { duration: 3000 });
      return;
    }

    let attendanceRecordId = this.lastAttendanceRecordId();
    if (action === 'ClockOut') {
      let openRecord: AttendanceRecord | null;
      try {
        openRecord = await this.fetchOpenAttendanceRecord(employeeId);
      } catch {
        this.snackbar.open('Unable to verify attendance records.', 'Dismiss', { duration: 3000 });
        return;
      }
      if (!openRecord) {
        this.snackbar.open('No attendance record available for clocking out.', 'Dismiss', { duration: 3000 });
        return;
      }
      attendanceRecordId = openRecord.id;
    }

    const timestampUtc = new Date().toISOString();
    const request =
      action === 'ClockIn'
        ? this.selfServiceApi.clockIn(employeeId, { timestampUtc, punchType: action })
        : this.selfServiceApi.clockOut(employeeId, attendanceRecordId!, {
            timestampUtc,
            punchType: action
          });

    request.subscribe({
      next: (record: AttendanceRecord) => {
        const message = `${action === 'ClockIn' ? 'Clocked in' : 'Clocked out'} at ${new Date().toLocaleTimeString()}`;
        this.lastAttendanceRecordId.set(record.id);
        this.lastPunch.set(message);
        this.updatePunchHistoryFromRecord(record);
        this.snackbar.open(`${action === 'ClockIn' ? 'Clock-in' : 'Clock-out'} recorded.`, 'Dismiss', { duration: 2000 });
      },
      error: (error) => {
        this.snackbar.open(
          error?.error?.title ?? `Unable to record ${action === 'ClockIn' ? 'clock-in' : 'clock-out'}.`,
          'Dismiss',
          { duration: 3000 }
        );
      }
    });
  }

  private async fetchOpenAttendanceRecord(employeeId: string): Promise<AttendanceRecord | null> {
    const records = await firstValueFrom(this.selfServiceApi.getAttendanceHistory(employeeId));
    if (records.items.length === 0) {
      this.lastAttendanceRecordId.set(null);
      this.updatePunchHistoryFromRecord(null);
      return null;
    }

    const sortedByDate = [...records.items].sort((a, b) => {
      const aDate = new Date(a.workDate).getTime();
      const bDate = new Date(b.workDate).getTime();
      return bDate - aDate;
    });

    const openRecord = sortedByDate.find((record) => !record.checkOutTimeUtc);
    this.lastAttendanceRecordId.set(openRecord?.id ?? null);
    this.updatePunchHistoryFromRecord(openRecord ?? sortedByDate[0] ?? null);
    return openRecord ?? null;
  }

  private updatePunchHistoryFromRecord(record: AttendanceRecord | null): void {
    const punches = record?.punches ?? [];
    if (punches.length === 0) {
      this.punchHistory.set([]);
      return;
    }

    const history = [...punches]
      .sort((a, b) => new Date(b.timestampUtc).getTime() - new Date(a.timestampUtc).getTime())
      .map((punch) => `${punch.type ?? 'Punch'} at ${this.formatTimestamp(punch.timestampUtc)}`)
      .slice(0, 3);

    this.punchHistory.set(history);
  }

  private formatTimestamp(value?: string | null): string {
    if (!value) {
      return 'unknown time';
    }
    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
      return value;
    }
    return parsed.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  private toIsoDate(value: string | Date | null | undefined): string {
    const parsed = value instanceof Date ? value : new Date(value ?? '');
    if (Number.isNaN(parsed.getTime())) {
      throw new Error('Invalid date value');
    }
    const iso = parsed.toISOString();
    const [datePart] = iso.split('T');
    return datePart ?? iso;
  }

}

