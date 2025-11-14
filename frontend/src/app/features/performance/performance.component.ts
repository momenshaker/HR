import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

interface PerformanceGoalDto {
  id?: string;
  title: string;
  description?: string;
  weight?: number;
  alignment?: string;
  status?: string;
}

interface PerformanceKpiDto {
  id?: string;
  name: string;
  targetValue: number;
  actualValue: number;
  unitOfMeasure?: string;
  status?: string;
}

interface PerformanceFeedbackDto {
  id?: string;
  feedbackType: string;
  comments: string;
  submittedBy: string;
  submittedAtUtc: string;
}

interface CompensationReviewDto {
  effectiveDate: string;
  currentBaseSalary: number;
  proposedBaseSalary: number;
  bonusRecommendation: number;
  currency: string;
  notes?: string;
}

interface PerformanceReviewDto {
  id: string;
  employeeId: string;
  cycleName: string;
  periodStart: string;
  periodEnd: string;
  overallScore: number;
  managerComments: string;
  goalsSummary: string;
  goals: PerformanceGoalDto[];
  keyPerformanceIndicators: PerformanceKpiDto[];
  feedbackCycles: PerformanceFeedbackDto[];
  compensationReview?: CompensationReviewDto | null;
  submittedAtUtc: string;
}

interface CreatePerformanceReviewRequest {
  employeeId: string;
  cycleName: string;
  periodStart: string;
  periodEnd: string;
  overallScore: number;
  managerComments: string;
  goalsSummary: string;
  goals: PerformanceGoalRequest[];
  keyPerformanceIndicators: PerformanceKpiRequest[];
  feedbackCycles: PerformanceFeedbackRequest[];
  compensationReview?: CompensationReviewRequest | null;
}

interface PerformanceGoalRequest {
  id?: string | null;
  title: string;
  description?: string;
  weight?: number;
  alignment?: string;
  status?: string;
  parentGoalId?: string | null;
}

interface PerformanceKpiRequest {
  id?: string | null;
  name: string;
  targetValue: number;
  actualValue: number;
  unitOfMeasure?: string;
  status?: string;
}

interface PerformanceFeedbackRequest {
  id?: string | null;
  feedbackType: string;
  comments: string;
  submittedBy: string;
  submittedAtUtc: string;
}

interface CompensationReviewRequest {
  effectiveDate: string;
  currentBaseSalary: number;
  proposedBaseSalary: number;
  bonusRecommendation: number;
  currency: string;
  notes?: string;
}

type GoalFormGroup = FormGroup<{
  id: FormControl<string | null>;
  title: FormControl<string>;
  description: FormControl<string>;
  weight: FormControl<number | null>;
  alignment: FormControl<string>;
  status: FormControl<string>;
  parentGoalId: FormControl<string | null>;
}>;

type KpiFormGroup = FormGroup<{
  id: FormControl<string | null>;
  name: FormControl<string>;
  targetValue: FormControl<number | null>;
  actualValue: FormControl<number | null>;
  unitOfMeasure: FormControl<string>;
  status: FormControl<string>;
}>;

type FeedbackFormGroup = FormGroup<{
  id: FormControl<string | null>;
  feedbackType: FormControl<string>;
  comments: FormControl<string>;
  submittedBy: FormControl<string>;
  submittedAtUtc: FormControl<string>;
}>;

type CompensationFormGroup = FormGroup<{
  effectiveDate: FormControl<string>;
  currentBaseSalary: FormControl<number | null>;
  proposedBaseSalary: FormControl<number | null>;
  bonusRecommendation: FormControl<number | null>;
  currency: FormControl<string>;
  notes: FormControl<string>;
}>;

@Component({
  selector: 'app-performance-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressBarModule
  ],
  templateUrl: './performance.component.html',
  styleUrls: ['./performance.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PerformancePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly snackbar = inject(MatSnackBar);

  private readonly resourceUrl = `${this.config.apiBaseUrl}/PerformanceReviews`;

  readonly scoreOptions = [0, 1, 2, 3, 4, 5];
  readonly goalStatuses = ['NotStarted', 'InProgress', 'Completed', 'OnHold'];
  readonly kpiStatuses = ['OnTrack', 'AtRisk', 'Behind'];
  readonly feedbackTypes = ['Manager', 'Peer', 'Self', 'Report'];
  readonly displayedColumns = ['cycle', 'employee', 'period', 'score', 'submitted', 'actions'] as const;

  readonly searchControl = new FormControl('', { nonNullable: true });

  readonly reviewForm = this.fb.group({
    employeeId: ['', Validators.required],
    cycleName: ['', [Validators.required, Validators.maxLength(100)]],
    periodStart: ['', Validators.required],
    periodEnd: ['', Validators.required],
    overallScore: [3, [Validators.min(0), Validators.max(5)]],
    managerComments: [''],
    goalsSummary: [''],
    goals: this.fb.array<GoalFormGroup>([]),
    kpis: this.fb.array<KpiFormGroup>([]),
    feedback: this.fb.array<FeedbackFormGroup>([]),
    compensation: this.fb.group({
      effectiveDate: [''],
      currentBaseSalary: [null],
      proposedBaseSalary: [null],
      bonusRecommendation: [null],
      currency: ['USD'],
      notes: ['']
    }) as CompensationFormGroup
  });

  readonly loading = signal(false);
  readonly reviews = signal<readonly PerformanceReviewDto[]>([]);
  readonly selectedReview = signal<PerformanceReviewDto | null>(null);

  readonly filteredReviews = computed(() => {
    const search = this.searchControl.value?.toLowerCase().trim();
    if (!search) {
      return this.reviews();
    }
    return this.reviews().filter((review) => {
      return (
        review.cycleName.toLowerCase().includes(search) ||
        review.employeeId.toLowerCase().includes(search) ||
        review.goalsSummary?.toLowerCase().includes(search)
      );
    });
  });

  ngOnInit(): void {
    this.addGoal();
    this.addKpi();
    this.addFeedback();
    this.loadReviews();
  }

  get goals(): FormArray<GoalFormGroup> {
    return this.reviewForm.controls.goals;
  }

  get kpis(): FormArray<KpiFormGroup> {
    return this.reviewForm.controls.kpis;
  }

  get feedback(): FormArray<FeedbackFormGroup> {
    return this.reviewForm.controls.feedback;
  }

  get compensation(): CompensationFormGroup {
    return this.reviewForm.controls.compensation;
  }

  addGoal(): void {
    this.goals.push(
      this.fb.group({
        id: this.fb.control<string | null>(null),
        title: this.fb.control('', Validators.required),
        description: this.fb.control(''),
        weight: this.fb.control<number | null>(null),
        alignment: this.fb.control(''),
        status: this.fb.control('NotStarted'),
        parentGoalId: this.fb.control<string | null>(null)
      })
    );
  }

  removeGoal(index: number): void {
    this.goals.removeAt(index);
  }

  addKpi(): void {
    this.kpis.push(
      this.fb.group({
        id: this.fb.control<string | null>(null),
        name: this.fb.control('', Validators.required),
        targetValue: this.fb.control<number | null>(null, Validators.required),
        actualValue: this.fb.control<number | null>(null, Validators.required),
        unitOfMeasure: this.fb.control(''),
        status: this.fb.control('OnTrack')
      })
    );
  }

  removeKpi(index: number): void {
    this.kpis.removeAt(index);
  }

  addFeedback(): void {
    this.feedback.push(
      this.fb.group({
        id: this.fb.control<string | null>(null),
        feedbackType: this.fb.control('Manager', Validators.required),
        comments: this.fb.control('', Validators.required),
        submittedBy: this.fb.control('', Validators.required),
        submittedAtUtc: this.fb.control(new Date().toISOString())
      })
    );
  }

  removeFeedback(index: number): void {
    this.feedback.removeAt(index);
  }

  loadReviews(): void {
    this.loading.set(true);
    this.http.get<PerformanceReviewDto[]>(this.resourceUrl).subscribe({
      next: (reviews) => {
        this.reviews.set(reviews);
        if (reviews.length > 0 && !this.selectedReview()) {
          this.selectedReview.set(reviews[0]);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  selectReview(review: PerformanceReviewDto): void {
    this.selectedReview.set(review);
  }

  createReview(): void {
    if (this.reviewForm.invalid) {
      this.reviewForm.markAllAsTouched();
      return;
    }

    const payload = this.buildPayload();
    this.loading.set(true);

    this.http.post<PerformanceReviewDto>(this.resourceUrl, payload).subscribe({
      next: (created) => {
        this.snackbar.open('Performance review created.', 'Dismiss', { duration: 3500 });
        this.reviewForm.reset({
          employeeId: '',
          cycleName: '',
          periodStart: '',
          periodEnd: '',
          overallScore: 3,
          managerComments: '',
          goalsSummary: ''
        });
        this.goals.clear();
        this.kpis.clear();
        this.feedback.clear();
        this.compensation.reset({
          effectiveDate: '',
          currentBaseSalary: null,
          proposedBaseSalary: null,
          bonusRecommendation: null,
          currency: 'USD',
          notes: ''
        });
        this.addGoal();
        this.addKpi();
        this.addFeedback();
        this.reviews.set([created, ...this.reviews()]);
        this.selectedReview.set(created);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  deleteReview(review: PerformanceReviewDto): void {
    if (!review?.id) {
      return;
    }
    this.loading.set(true);
    this.http.delete(`${this.resourceUrl}/${review.id}`).subscribe({
      next: () => {
        this.snackbar.open('Performance review deleted.', 'Dismiss', { duration: 3000 });
        const updated = this.reviews().filter((item) => item.id !== review.id);
        this.reviews.set(updated);
        if (this.selectedReview()?.id === review.id) {
          this.selectedReview.set(updated[0] ?? null);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private buildPayload(): CreatePerformanceReviewRequest {
    const formValue = this.reviewForm.getRawValue();

    const goals = this.goals.controls
      .map((group) => ({
        id: this.optional(group.controls.id.value),
        title: group.controls.title.value,
        description: group.controls.description.value,
        weight: group.controls.weight.value ?? 0,
        alignment: group.controls.alignment.value,
        status: group.controls.status.value,
        parentGoalId: this.optional(group.controls.parentGoalId.value)
      }))
      .filter((goal) => goal.title.trim().length > 0);

    const kpis = this.kpis.controls
      .map((group) => ({
        id: this.optional(group.controls.id.value),
        name: group.controls.name.value,
        targetValue: Number(group.controls.targetValue.value ?? 0),
        actualValue: Number(group.controls.actualValue.value ?? 0),
        unitOfMeasure: group.controls.unitOfMeasure.value,
        status: group.controls.status.value
      }))
      .filter((kpi) => kpi.name.trim().length > 0);

    const feedback = this.feedback.controls
      .map((group) => ({
        id: this.optional(group.controls.id.value),
        feedbackType: group.controls.feedbackType.value,
        comments: group.controls.comments.value,
        submittedBy: group.controls.submittedBy.value,
        submittedAtUtc: group.controls.submittedAtUtc.value || new Date().toISOString()
      }))
      .filter((item) => item.comments.trim().length > 0);

    const compensationRaw = formValue.compensation;
    const compensation: CompensationReviewRequest | null = compensationRaw?.effectiveDate
      ? {
          effectiveDate: compensationRaw.effectiveDate,
          currentBaseSalary: Number(compensationRaw.currentBaseSalary ?? 0),
          proposedBaseSalary: Number(compensationRaw.proposedBaseSalary ?? 0),
          bonusRecommendation: Number(compensationRaw.bonusRecommendation ?? 0),
          currency: compensationRaw.currency ?? 'USD',
          notes: compensationRaw.notes ?? ''
        }
      : null;

    return {
      employeeId: formValue.employeeId,
      cycleName: formValue.cycleName,
      periodStart: formValue.periodStart,
      periodEnd: formValue.periodEnd,
      overallScore: Number(formValue.overallScore ?? 0),
      managerComments: formValue.managerComments ?? '',
      goalsSummary: formValue.goalsSummary ?? '',
      goals,
      keyPerformanceIndicators: kpis,
      feedbackCycles: feedback,
      compensationReview: compensation
    };
  }

  private optional(value: string | null | undefined): string | undefined {
    return value && value.trim().length > 0 ? value : undefined;
  }

  trackByReview(_: number, review: PerformanceReviewDto): string {
    return review.id;
  }
}
