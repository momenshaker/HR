import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { OnboardingApiService, OnboardingRequest } from './onboarding.api';
import { PlanApiService } from '../subscriptions/plan.api';
import { Plan } from '../subscriptions/subscriptions.types';
import { Router } from '@angular/router';

@Component({
  selector: 'app-onboarding-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './onboarding-page.component.html',
  styleUrls: ['./onboarding-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OnboardingPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly onboardingApi = inject(OnboardingApiService);
  private readonly planApi = inject(PlanApiService);
  private readonly router = inject(Router);
  private readonly snackbar = inject(MatSnackBar);

  readonly accountForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    phoneNumber: ['']
  });

  readonly organizationForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    code: ['', [Validators.required, Validators.maxLength(50)]],
    description: ['', [Validators.maxLength(500)]],
    industry: ['', [Validators.maxLength(150)]],
    region: ['', [Validators.maxLength(100)]],
    headquartersAddress: ['', [Validators.required, Validators.maxLength(300)]],
    timeZone: ['', [Validators.maxLength(50)]],
    primaryContactEmail: ['', [Validators.email, Validators.maxLength(150)]],
    websiteUrl: ['', [Validators.maxLength(200)]],
    billingAddressLine1: ['', [Validators.required, Validators.maxLength(200)]],
    billingAddressLine2: ['', [Validators.maxLength(200)]],
    billingCity: ['', [Validators.required, Validators.maxLength(100)]],
    billingState: ['', [Validators.required, Validators.maxLength(100)]],
    billingPostalCode: ['', [Validators.required, Validators.maxLength(20)]],
    billingCountry: ['', [Validators.required, Validators.maxLength(100)]],
    billingPhone: ['', [Validators.maxLength(30)]]
  });

  readonly subscriptionForm = this.fb.nonNullable.group({
    planId: ['', [Validators.required]],
    seats: [5, [Validators.required, Validators.min(1)]],
    trialPeriodDays: [14, [Validators.min(0), Validators.max(365)]]
  });

  readonly stepLabels = ['Account setup', 'Organization details', 'Subscription'];
  readonly currentStep = signal(0);
  readonly busy = signal(false);
  readonly error = signal<string | null>(null);
  readonly loadingPlans = signal(true);
  readonly plans = signal<readonly Plan[]>([]);
  readonly selectedPlanId = signal<string>('');

  readonly selectedPlan = computed(() =>
    this.plans().find((plan) => plan.id === this.selectedPlanId())
  );

  readonly selectedPlanSeats = computed(() =>
    this.plans().find((plan) => plan.id === this.selectedPlanId())?.entitlements[0]?.quantity
  );

  ngOnInit(): void {
    this.loadPlans();
  }

  nextStep(): void {
    const form = this.getFormForStep(this.currentStep());
    if (form.invalid) {
      form.markAllAsTouched();
      return;
    }
    this.error.set(null);
    this.currentStep.update((value) => Math.min(value + 1, this.stepLabels.length - 1));
  }

  previousStep(): void {
    this.error.set(null);
    this.currentStep.update((value) => Math.max(0, value - 1));
  }

  selectPlan(plan: Plan): void {
    this.selectedPlanId.set(plan.id);
    this.subscriptionForm.controls.planId.setValue(plan.id, { emitEvent: false });
  }

  submit(): void {
    if (this.busy()) {
      return;
    }

    if (this.subscriptionForm.invalid || this.accountForm.invalid || this.organizationForm.invalid) {
      this.currentStep.set(this.stepLabels.length - 1);
      this.subscriptionForm.markAllAsTouched();
      this.accountForm.markAllAsTouched();
      this.organizationForm.markAllAsTouched();
      return;
    }

    const payload: OnboardingRequest = {
      account: {
        fullName: this.accountForm.controls.fullName.value,
        email: this.accountForm.controls.email.value,
        password: this.accountForm.controls.password.value,
        phoneNumber: this.accountForm.controls.phoneNumber.value
      },
      organization: {
        name: this.organizationForm.controls.name.value,
        code: this.organizationForm.controls.code.value,
        description: this.organizationForm.controls.description.value,
        industry: this.organizationForm.controls.industry.value,
        region: this.organizationForm.controls.region.value,
        headquartersAddress: this.organizationForm.controls.headquartersAddress.value,
        timeZone: this.organizationForm.controls.timeZone.value,
        primaryContactEmail: this.organizationForm.controls.primaryContactEmail.value,
        websiteUrl: this.organizationForm.controls.websiteUrl.value,
        billingAddressLine1: this.organizationForm.controls.billingAddressLine1.value,
        billingAddressLine2: this.organizationForm.controls.billingAddressLine2.value,
        billingCity: this.organizationForm.controls.billingCity.value,
        billingState: this.organizationForm.controls.billingState.value,
        billingPostalCode: this.organizationForm.controls.billingPostalCode.value,
        billingCountry: this.organizationForm.controls.billingCountry.value,
        billingPhone: this.organizationForm.controls.billingPhone.value
      },
      subscription: {
        planId: this.subscriptionForm.controls.planId.value,
        seats: this.subscriptionForm.controls.seats.value,
        trialPeriodDays: this.subscriptionForm.controls.trialPeriodDays.value ?? undefined
      }
    };

    this.busy.set(true);
    const snackBarRef = this.snackbar.open('Workspace created. You can now sign in.', 'Go to login', {
      duration: 5000
    });
    snackBarRef.onAction().subscribe(() => this.router.navigate(['/auth/login']));
    this.onboardingApi.start(payload).subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (errorResponse) => {
        const message = this.extractErrorMessage(errorResponse);
        this.error.set(message);
        this.busy.set(false);
      }
    });
  }

  private loadPlans(): void {
    this.loadingPlans.set(true);
    this.planApi.list().subscribe({
      next: (items) => {
        debugger;
        this.plans.set(items);
        if (items.length > 0) {
          const defaultPlan = items[0];
          if (defaultPlan) {
            this.selectPlan(defaultPlan);
          }
        }
        this.loadingPlans.set(false);
      },
      error: () => {
        this.error.set('Unable to load plans. Please try again later.');
        this.loadingPlans.set(false);
      }
    });
  }

  private getFormForStep(step: number) {
    return step === 0 ? this.accountForm : step === 1 ? this.organizationForm : this.subscriptionForm;
  }

  private extractErrorMessage(response: unknown): string {
    if (response && typeof response === 'object' && 'error' in response) {
      const body = (response as { error: { message?: string } }).error;
      if (body?.message) {
        return body.message;
      }
    }

    return 'Unable to complete onboarding. Please try again.';
  }
}
