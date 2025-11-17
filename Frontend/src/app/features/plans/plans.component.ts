import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { PlanApiService, PlanPayload } from '../subscriptions/plan.api';
import { Plan } from '../subscriptions/subscriptions.types';

@Component({
  selector: 'app-plans-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressBarModule,
    MatTableModule
  ],
  templateUrl: './plans.component.html',
  styleUrls: ['./plans.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlansPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly planApi = inject(PlanApiService);

  readonly form = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.required]],
    price: [0, [Validators.required, Validators.min(0)]],
    billingInterval: ['Monthly', [Validators.required]],
    entitlements: ['[]', [Validators.required]]
  });

  readonly plans = signal<readonly Plan[]>([]);
  readonly loading = signal(false);
  readonly editingPlanId = signal<string | null>(null);
  readonly isEditMode = computed(() => this.editingPlanId() !== null);

  ngOnInit(): void {
    this.loadPlans();
  }

  createPlan(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.buildPayload();
    this.planApi.create(payload).subscribe({
      next: () => {
        this.snackbar.open('Plan created.', 'Dismiss', { duration: 2500 });
        this.resetForm();
        this.loadPlans();
      },
      error: () => this.snackbar.open('Failed to create plan.', 'Dismiss', { duration: 3000 })
    });
  }

  editPlan(plan: Plan): void {
    this.editingPlanId.set(plan.id);
    this.form.setValue({
      code: plan.code,
      name: plan.name,
      description: plan.description,
      price: plan.price,
      billingInterval: plan.billingInterval,
      entitlements: JSON.stringify(plan.entitlements, null, 2)
    }, { emitEvent: false });
  }

  updatePlan(): void {
    if (this.form.invalid || !this.editingPlanId()) {
      return;
    }

    const payload = this.buildPayload();
    this.planApi.update(this.editingPlanId()!, payload).subscribe({
      next: () => {
        this.snackbar.open('Plan updated.', 'Dismiss', { duration: 2500 });
        this.resetForm();
        this.loadPlans();
      },
      error: () => this.snackbar.open('Failed to update plan.', 'Dismiss', { duration: 3000 })
    });
  }

  cancelEdit(): void {
    this.resetForm();
  }

  deletePlan(plan: Plan): void {
    this.planApi.delete(plan.id).subscribe({
      next: () => {
        this.snackbar.open('Plan deleted.', 'Dismiss', { duration: 2500 });
        this.loadPlans();
      },
      error: () => this.snackbar.open('Failed to delete plan.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadPlans(): void {
    this.loading.set(true);
    this.planApi.list().subscribe({
      next: (plans) => {
        this.plans.set(plans);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.open('Failed to load plans.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private resetForm(): void {
    this.form.reset({
      code: '',
      name: '',
      description: '',
      price: 0,
      billingInterval: 'Monthly',
      entitlements: '[]'
    });
    this.editingPlanId.set(null);
  }

  private buildPayload(): PlanPayload {
    const value = this.form.getRawValue();
    let entitlements;
    try {
      entitlements = JSON.parse(value.entitlements);
    } catch {
      throw new Error('Invalid entitlements JSON');
    }
    return {
      code: value.code.trim(),
      name: value.name.trim(),
      description: value.description.trim(),
      price: value.price,
      billingInterval: value.billingInterval.trim(),
      entitlements
    };
  }
}
