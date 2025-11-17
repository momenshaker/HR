import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatOptionModule } from '@angular/material/core';
import { EntityCrudFactory } from '@core/data-access';
import { OrganizationSummary } from '@app/features/organizations/organizations.component';
import { SubscriptionsApiService } from './subscriptions.api';
import { CreateSubscriptionPayload, Invoice, Plan, Subscription } from './subscriptions.types';
import { PlanApiService } from './plan.api';

@Component({
  selector: 'app-subscriptions-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressBarModule
  ],
  templateUrl: './subscriptions.component.html',
  styleUrls: ['./subscriptions.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SubscriptionsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly subscriptionsApi = inject(SubscriptionsApiService);
  private readonly planApi = inject(PlanApiService);
  private readonly organizationService = inject(EntityCrudFactory).create<
    unknown,
    unknown,
    OrganizationSummary
  >('organizations');

  readonly creationForm = this.fb.nonNullable.group({
    planId: ['', [Validators.required, Validators.maxLength(100)]],
    seats: [1, [Validators.required, Validators.min(1)]],
    trialPeriodDays: [0, [Validators.min(0), Validators.max(30)]],
    metadata: ['{}']
  });

  readonly subscriptions = signal<readonly Subscription[]>([]);
  readonly organizations = signal<readonly OrganizationSummary[]>([]);
  readonly plans = signal<readonly Plan[]>([]);
  readonly loading = signal(false);
  readonly invoiceLoading = signal(false);
  readonly latestInvoice = signal<Invoice | null>(null);
  readonly selectedSubscriptionId = signal<string | null>(null);
  readonly statusFilter = signal<string>('');
  readonly statuses = ['Active', 'Inactive', 'Canceled', 'PastDue'] as const;
  readonly selectedPlan = computed(() => {
    const planId = this.creationForm.controls.planId.value;
    return this.plans().find((plan) => plan.id === planId) ?? null;
  });
  readonly filteredSubscriptions = computed(() => this.subscriptions());
  readonly organizationMap = computed(() => {
    const map = new Map<string, OrganizationSummary>();
    for (const organization of this.organizations()) {
      map.set(organization.id, organization);
    }
    return map;
  });

  ngOnInit(): void {
    this.loadSubscriptions();
    this.loadPlans();
    this.loadOrganizations();
  }

  refresh(): void {
    this.loadSubscriptions();
  }

  applyFilters(): void {
    this.loadSubscriptions(this.statusFilter() || undefined);
  }

  create(): void {
    if (this.creationForm.invalid) {
      this.creationForm.markAllAsTouched();
      return;
    }

    const value = this.creationForm.getRawValue();
    let metadata: Record<string, string> | undefined;
    try {
      metadata = this.parseMetadata(value.metadata);
    } catch (error) {
      this.snackbar.open('Metadata must be valid JSON.', 'Dismiss', { duration: 3000 });
      return;
    }

    const payload: CreateSubscriptionPayload = {
      planId: value.planId,
      seats: value.seats,
      trialPeriodDays: value.trialPeriodDays ?? undefined,
      metadata
    };

    this.subscriptionsApi.create(payload).subscribe({
      next: () => {
        this.snackbar.open('Subscription created.', 'Dismiss', { duration: 3000 });
        this.creationForm.reset({ planId: '', seats: 1, trialPeriodDays: 0, metadata: '{}' });
        this.loadSubscriptions();
      },
      error: () => this.snackbar.open('Failed to create subscription.', 'Dismiss', { duration: 3000 })
    });
  }

  cancel(subscription: Subscription): void {
    this.subscriptionsApi.cancel(subscription.id).subscribe({
      next: () => {
        this.snackbar.open('Subscription canceled.', 'Dismiss', { duration: 3000 });
        this.loadSubscriptions();
      },
      error: () => this.snackbar.open('Failed to cancel subscription.', 'Dismiss', { duration: 3000 })
    });
  }

  loadInvoice(subscription: Subscription): void {
    this.invoiceLoading.set(true);
    this.selectedSubscriptionId.set(subscription.id);
    this.subscriptionsApi.getLatestInvoice(subscription.id).subscribe({
      next: (invoice) => {
        this.latestInvoice.set(invoice);
        this.invoiceLoading.set(false);
      },
      error: () => {
        this.invoiceLoading.set(false);
        this.snackbar.open('Unable to load invoice.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  formatDate(value?: string | null): string {
    if (!value) {
      return '—';
    }
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? value : date.toLocaleDateString();
  }

  trackBySubscription(_: number, subscription: Subscription): string {
    return subscription.id;
  }

  private loadSubscriptions(status?: string): void {
    this.loading.set(true);
    this.subscriptionsApi.list(0, 25, status).subscribe({
      next: (response) => {
        this.subscriptions.set(response.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open('Failed to load subscriptions.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private parseMetadata(value: string): Record<string, string> | undefined {
    const trimmed = value?.trim();
    if (!trimmed) {
      return undefined;
    }

    const parsed = JSON.parse(trimmed);
    if (typeof parsed !== 'object' || parsed === null) {
      throw new Error('Metadata must be an object.');
    }

    const normalized: Record<string, string> = {};
    for (const [key, entry] of Object.entries(parsed)) {
      normalized[key] = entry?.toString() ?? '';
    }
    return normalized;
  }

  private loadPlans(): void {
    this.planApi.list().subscribe({
      next: (plans) => this.plans.set(plans),
      error: () => this.snackbar.open('Failed to load plans.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadOrganizations(): void {
    this.organizationService.list({ page: 1, pageSize: 500 }).subscribe({
      next: (response) => {
        this.organizations.set(response.items);
      },
      error: () => this.snackbar.open('Failed to load organizations.', 'Dismiss', { duration: 3000 })
    });
  }

  updateOrganizations(subscription: Subscription, organizationIds: readonly string[]): void {
    this.subscriptionsApi.assignOrganization(subscription.id, organizationIds).subscribe({
      next: () => {
        this.snackbar.open('Organizations updated.', 'Dismiss', { duration: 2500 });
        this.loadSubscriptions();
      },
      error: () => this.snackbar.open('Failed to update organizations.', 'Dismiss', { duration: 3000 })
    });
  }

  formatPlanLabel(plan: Plan | null): string {
    if (!plan) {
      return '';
    }
    return `${plan.name} (${plan.billingInterval} • $${plan.price.toFixed(2)})`;
  }
}
