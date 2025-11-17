import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { EntityCrudFactory } from '@core/data-access';
import { OrganizationSummary } from '../organizations/organizations.component';
import { PayrollApiService, PayrollBreakdown, PayrollComponentAmount, PayrollItem, PayrollRun, PayrollStatus } from './payroll.api';

@Component({
  selector: 'app-payroll-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressBarModule,
    MatChipsModule,
    MatTableModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './payroll.component.html',
  styleUrls: ['./payroll.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PayrollPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly payrollApi = inject(PayrollApiService);
  private readonly organizationService = inject(EntityCrudFactory).create<unknown, unknown, OrganizationSummary>(
    'organizations'
  );

  readonly statuses: readonly PayrollStatus[] = ['Draft', 'Calculated', 'UnderReview', 'Approved', 'Locked', 'Paid'];
  readonly organizations = signal<ReadonlyArray<OrganizationSummary>>([]);

  readonly creationForm = this.fb.nonNullable.group({
    organizationId: ['', Validators.required],
    periodStart: ['', Validators.required],
    periodEnd: ['', Validators.required],
    payDate: ['', Validators.required],
    notes: ['']
  });

  readonly filterForm = this.fb.nonNullable.group({
    organizationId: [''],
    status: ['']
  });

  readonly runs = signal<readonly PayrollRun[]>([]);
  readonly runsLoading = signal(false);
  readonly items = signal<readonly PayrollItem[]>([]);
  readonly itemsLoading = signal(false);
  readonly selectedRunId = signal<string | null>(null);
  readonly selectedItem = signal<PayrollItem | null>(null);

  readonly selectedRun = computed(() => {
    const runId = this.selectedRunId();
    if (!runId) {
      return null;
    }
    return this.runs().find((run) => run.id === runId) ?? null;
  });

  readonly runColumns = ['period', 'payDate', 'status', 'totals', 'actions'] as const;
  readonly itemColumns = ['employee', 'gross', 'deductions', 'net', 'currency', 'breakdown'] as const;

  ngOnInit(): void {
    this.loadRuns();
    this.loadOrganizations();
  }

  refresh(): void {
    this.loadRuns();
  }

  createRun(): void {
    if (this.creationForm.invalid) {
      this.creationForm.markAllAsTouched();
      return;
    }

    const formValue = this.creationForm.getRawValue();
    const periodEndDate = this.toDate(formValue.periodEnd);
    const payDate = this.toDate(formValue.payDate);
    if (periodEndDate && payDate && payDate < periodEndDate) {
      this.snackbar.open('Pay date must be on or after the payroll period end.', 'Dismiss', { duration: 3000 });
      return;
    }

    const payload = {
      ...formValue,
      periodStart: this.formatDateValue(formValue.periodStart),
      periodEnd: this.formatDateValue(formValue.periodEnd),
      payDate: this.formatDateValue(formValue.payDate)
    };
    this.runsLoading.set(true);
    this.payrollApi.createRun(payload).subscribe({
      next: (run) => {
        this.snackbar.open('Payroll period created.', 'Dismiss', { duration: 2500 });
        this.creationForm.reset({
          organizationId: '',
          periodStart: '',
          periodEnd: '',
          payDate: '',
          notes: ''
        });
        this.selectedRunId.set(run.id);
        this.loadRuns();
        this.loadItems(run.id);
      },
      error: () => this.runsLoading.set(false)
    });
  }

  applyFilters(): void {
    this.loadRuns();
  }

  selectRun(run: PayrollRun): void {
    this.selectedRunId.set(run.id);
    this.selectedItem.set(null);
    this.loadItems(run.id);
  }

  calculate(run: PayrollRun): void {
    this.transitionRun(run, this.payrollApi.calculate(run.id), 'Payroll calculated.');
  }

  moveToReview(run: PayrollRun): void {
    this.transitionRun(run, this.payrollApi.moveToReview(run.id), 'Moved to under review.');
  }

  approve(run: PayrollRun): void {
    this.transitionRun(run, this.payrollApi.approve(run.id), 'Payroll approved.');
  }

  lock(run: PayrollRun): void {
    this.transitionRun(run, this.payrollApi.lock(run.id), 'Payroll locked for payment.');
  }

  markPaid(run: PayrollRun): void {
    this.transitionRun(run, this.payrollApi.markPaid(run.id), 'Marked as paid.');
  }

  generatePayslips(run: PayrollRun): void {
    this.runsLoading.set(true);
    this.payrollApi.generatePayslips(run.id).subscribe({
      next: () => {
        this.snackbar.open('Payslips generated.', 'Dismiss', { duration: 2500 });
        this.runsLoading.set(false);
      },
      error: () => this.runsLoading.set(false)
    });
  }

  selectItem(item: PayrollItem): void {
    this.selectedItem.set(item);
  }

  canCalculate(run: PayrollRun): boolean {
    return run.status === 'Draft';
  }

  canMoveToReview(run: PayrollRun): boolean {
    return run.status === 'Calculated';
  }

  canApprove(run: PayrollRun): boolean {
    return run.status === 'UnderReview';
  }

  canLock(run: PayrollRun): boolean {
    return run.status === 'Approved';
  }

  canMarkPaid(run: PayrollRun): boolean {
    return run.status === 'Locked';
  }

  canGeneratePayslips(run: PayrollRun): boolean {
    return run.status === 'Locked' || run.status === 'Paid';
  }

  getEarnings(item: PayrollItem): readonly PayrollComponentAmount[] {
    if (item.earnings && item.earnings.length > 0) {
      return item.earnings;
    }
    const breakdown = this.parseBreakdown(item);
    return breakdown?.earnings ?? [];
  }

  getDeductions(item: PayrollItem): readonly PayrollComponentAmount[] {
    if (item.deductionComponents && item.deductionComponents.length > 0) {
      return item.deductionComponents;
    }
    const breakdown = this.parseBreakdown(item);
    return breakdown?.deductions ?? [];
  }

  private parseBreakdown(item: PayrollItem): PayrollBreakdown | null {
    if (!item.breakdownJson) {
      return null;
    }
    try {
      const parsed = JSON.parse(item.breakdownJson) as PayrollBreakdown;
      return parsed;
    } catch {
      return null;
    }
  }

  private formatDateValue(value: any): string {
    if (!value) {
      return '';
    }

    const dateValue = new Date(value);
    if (Number.isNaN(dateValue.getTime())) {
      return '';
    }

    const [datePart = ''] = dateValue.toISOString().split('T');
    return datePart;
  }

  private loadRuns(): void {
    this.runsLoading.set(true);
    const { organizationId, status } = this.filterForm.getRawValue();
    const statusValue = status ? (status as PayrollStatus) : undefined;
    this.payrollApi
      .listRuns({ organizationId: organizationId || undefined, status: statusValue })
      .subscribe({
        next: (runs) => {
          this.runs.set(runs);
          this.runsLoading.set(false);
          const selectedId = this.selectedRunId();
          if (selectedId && !runs.some((run) => run.id === selectedId)) {
            this.selectedRunId.set(null);
            this.items.set([]);
            this.selectedItem.set(null);
          }
        },
        error: () => this.runsLoading.set(false)
      });
  }

  private loadItems(runId: string): void {
    this.itemsLoading.set(true);
    this.payrollApi.listItems(runId).subscribe({
      next: (items) => {
        this.items.set(items);
        this.selectedItem.set(items[0] ?? null);
        this.itemsLoading.set(false);
      },
      error: () => this.itemsLoading.set(false)
    });
  }

  private loadOrganizations(): void {
    this.organizationService.list({ page: 1, pageSize: 250 }).subscribe({
      next: (response) => this.organizations.set(response.items),
      error: () => this.snackbar.open('Failed to load organizations.', 'Dismiss', { duration: 3000 })
    });
  }

  private transitionRun(run: PayrollRun, observable: ReturnType<PayrollApiService['calculate']>, message: string): void {
    this.runsLoading.set(true);
    observable.subscribe({
      next: (updated) => {
        this.updateRunList(updated);
        this.selectedRunId.set(updated.id);
        this.runsLoading.set(false);
        this.snackbar.open(message, 'Dismiss', { duration: 2500 });
      },
      error: () => this.runsLoading.set(false)
    });
  }

  private updateRunList(updated: PayrollRun): void {
    const currentRuns = this.runs();
    const index = currentRuns.findIndex((r) => r.id === updated.id);
    if (index === -1) {
      this.runs.set([updated, ...currentRuns]);
      return;
    }
    const clone = [...currentRuns];
    clone.splice(index, 1, updated);
    this.runs.set(clone);
  }

  private toDate(value?: string | Date | null): Date | null {
    if (!value) {
      return null;
    }

    const parsed = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(parsed.getTime())) {
      return null;
    }

    return parsed;
  }
}
