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
import { MatTabsModule } from '@angular/material/tabs';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

interface RatingScaleDto {
  id: string;
  name: string;
  minScore: number;
  maxScore: number;
  allowHalfPoints: boolean;
  levels: RatingScaleLevelDto[];
}

interface RatingScaleLevelDto {
  id: string;
  ratingScaleId: string;
  score: number;
  label: string;
  description: string;
}

interface TemplateItemDefinitionDto {
  id: string;
  sectionDefinitionId: string;
  name: string;
  description: string;
  defaultWeight: number;
}

interface TemplateSectionDefinitionDto {
  id: string;
  templateId: string;
  name: string;
  weight: number;
  items: TemplateItemDefinitionDto[];
}

interface EvaluationTemplateDto {
  id: string;
  name: string;
  description: string;
  targetRole: string;
  ratingScaleId: string;
  isDefault: boolean;
  isActive: boolean;
  sections: TemplateSectionDefinitionDto[];
}

interface PerformanceCycleAssignmentDto {
  employeeId: string;
  managerId?: string | null;
  department: string;
}

type PerformanceCycleStatus = 'Draft' | 'Active' | 'Closed' | 'Archived';

interface PerformanceCycleDto {
  id: string;
  name: string;
  description: string;
  periodStart: string;
  periodEnd: string;
  selfEvaluationStart: string;
  selfEvaluationEnd: string;
  managerEvaluationStart: string;
  managerEvaluationEnd: string;
  status: PerformanceCycleStatus;
  templateId: string;
  ratingScaleId: string;
  includedEmployees: PerformanceCycleAssignmentDto[];
  createdAt: string;
  createdBy: string;
  evaluationCount: number;
}

interface EvaluationSummaryDto {
  id: string;
  employeeId: string;
  managerId?: string;
  cycleId: string;
  status: string;
  overallScore: number;
  cycleName: string;
  templateName: string;
}

interface EvaluationItemDto {
  id: string;
  templateItemDefinitionId: string;
  name: string;
  weight: number;
  selfScore?: number;
  selfComment: string;
  managerScore?: number;
  managerComment: string;
  finalScore?: number;
}

interface EvaluationSectionDto {
  id: string;
  name: string;
  weight: number;
  score: number;
  comments: string;
  items: EvaluationItemDto[];
}

interface EvaluationDto {
  id: string;
  employeeId: string;
  managerId?: string;
  cycleId: string;
  templateId: string;
  overallScore: number;
  status: string;
  finalCommentsEmployee: string;
  finalCommentsManager: string;
  sections: EvaluationSectionDto[];
  createdAt: string;
  updatedAt: string;
}

type TemplateSectionFormGroup = FormGroup<{
  name: FormControl<string>;
  weight: FormControl<number | null>;
  items: FormArray<TemplateItemFormGroup>;
}>;

type TemplateItemFormGroup = FormGroup<{
  name: FormControl<string>;
  description: FormControl<string>;
  defaultWeight: FormControl<number | null>;
}>;

type CycleForm = FormGroup<{
  name: FormControl<string>;
  description: FormControl<string>;
  periodStart: FormControl<string>;
  periodEnd: FormControl<string>;
  selfEvaluationStart: FormControl<string>;
  selfEvaluationEnd: FormControl<string>;
  managerEvaluationStart: FormControl<string>;
  managerEvaluationEnd: FormControl<string>;
  templateId: FormControl<string>;
  ratingScaleId: FormControl<string>;
  includedEmployees: FormControl<string>;
  createdBy: FormControl<string>;
}>;

type EvaluationItemFormGroup = FormGroup<{
  itemId: FormControl<string>;
  selfScore: FormControl<number | null>;
  selfComment: FormControl<string>;
  managerScore: FormControl<number | null>;
  managerComment: FormControl<string>;
}>;

type EvaluationSectionFormGroup = FormGroup<{
  sectionId: FormControl<string>;
  items: FormArray<EvaluationItemFormGroup>;
}>;

type EvaluationForm = FormGroup<{
  comments: FormControl<string>;
  sections: FormArray<EvaluationSectionFormGroup>;
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
    MatProgressBarModule,
    MatTabsModule
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

  private readonly baseUrl = `${this.config.apiBaseUrl}/performance`;

  readonly loading = signal(false);
  readonly templates = signal<readonly EvaluationTemplateDto[]>([]);
  readonly ratingScales = signal<readonly RatingScaleDto[]>([]);
  readonly cycles = signal<readonly PerformanceCycleDto[]>([]);
  readonly evaluations = signal<readonly EvaluationSummaryDto[]>([]);
  readonly selectedCycle = signal<PerformanceCycleDto | null>(null);
  readonly selectedEvaluation = signal<EvaluationDto | null>(null);

  readonly cycleColumns = ['name', 'period', 'status', 'count', 'actions'] as const;
  readonly evaluationColumns = ['employee', 'status', 'score', 'actions'] as const;

  readonly templateForm = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.maxLength(200)],
    targetRole: ['Staff', Validators.required],
    ratingScaleId: ['', Validators.required],
    sections: this.fb.array<TemplateSectionFormGroup>([])
  });

  readonly cycleForm: CycleForm = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.maxLength(200)],
    periodStart: ['', Validators.required],
    periodEnd: ['', Validators.required],
    selfEvaluationStart: ['', Validators.required],
    selfEvaluationEnd: ['', Validators.required],
    managerEvaluationStart: ['', Validators.required],
    managerEvaluationEnd: ['', Validators.required],
    templateId: ['', Validators.required],
    ratingScaleId: ['', Validators.required],
    includedEmployees: ['', Validators.required],
    createdBy: ['00000000-0000-0000-0000-000000000000', Validators.required]
  });

  readonly selfForm: EvaluationForm = this.fb.group({
    comments: [''],
    sections: this.fb.array<EvaluationSectionFormGroup>([])
  });

  readonly managerForm: EvaluationForm = this.fb.group({
    comments: [''],
    sections: this.fb.array<EvaluationSectionFormGroup>([])
  });

  readonly searchControl = new FormControl('', { nonNullable: true });

  readonly filteredCycles = computed(() => {
    const term = this.searchControl.value.toLowerCase().trim();
    if (!term) {
      return this.cycles();
    }
    return this.cycles().filter((cycle) =>
      cycle.name.toLowerCase().includes(term) || cycle.description?.toLowerCase().includes(term)
    );
  });

  ngOnInit(): void {
    this.addSection();
    this.loadRatingScales();
    this.loadTemplates();
    this.loadCycles();
  }

  get sectionControls(): FormArray<TemplateSectionFormGroup> {
    return this.templateForm.controls.sections;
  }

  get selfSections(): FormArray<EvaluationSectionFormGroup> {
    return this.selfForm.controls.sections;
  }

  get managerSections(): FormArray<EvaluationSectionFormGroup> {
    return this.managerForm.controls.sections;
  }

  addSection(): void {
    this.sectionControls.push(
      this.fb.group({
        name: this.fb.nonNullable.control('', Validators.required),
        weight: this.fb.control<number | null>(null, Validators.required),
        items: this.fb.array<TemplateItemFormGroup>([])
      }) as TemplateSectionFormGroup
    );
    this.addTemplateItem(this.sectionControls.length - 1);
  }

  removeSection(index: number): void {
    this.sectionControls.removeAt(index);
  }

  addTemplateItem(sectionIndex: number): void {
    const section = this.sectionControls.at(sectionIndex);
    if (!section) {
      return;
    }
    section.controls.items.push(
      this.fb.group({
        name: this.fb.nonNullable.control('', Validators.required),
        description: this.fb.nonNullable.control(''),
        defaultWeight: this.fb.control<number | null>(null, Validators.required)
      }) as TemplateItemFormGroup
    );
  }

  removeTemplateItem(sectionIndex: number, itemIndex: number): void {
    const section = this.sectionControls.at(sectionIndex);
    section?.controls.items.removeAt(itemIndex);
  }

  addAssignmentExample(): void {
    const exampleManager = crypto.randomUUID();
    const exampleEmployee = crypto.randomUUID();
    this.cycleForm.controls.includedEmployees.setValue(`${exampleEmployee},${exampleManager},Engineering`);
  }

  createTemplate(): void {
    if (this.templateForm.invalid) {
      this.templateForm.markAllAsTouched();
      return;
    }

    const payload = this.buildTemplatePayload();
    this.loading.set(true);
    this.http.post<EvaluationTemplateDto>(`${this.baseUrl}/templates`, payload).subscribe({
      next: (template) => {
        this.templates.set([template, ...this.templates()]);
        this.templateForm.reset({
          name: '',
          description: '',
          targetRole: 'Staff',
          ratingScaleId: this.ratingScales()[0]?.id ?? ''
        });
        this.sectionControls.clear();
        this.addSection();
        this.snackbar.open('Template saved', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  createCycle(): void {
    if (this.cycleForm.invalid) {
      this.cycleForm.markAllAsTouched();
      return;
    }

    const payload = this.buildCyclePayload();
    this.loading.set(true);
    this.http.post<PerformanceCycleDto>(`${this.baseUrl}/cycles`, payload).subscribe({
      next: (cycle) => {
        this.cycles.set([cycle, ...this.cycles()]);
        this.cycleForm.reset({
          name: '',
          description: '',
          periodStart: '',
          periodEnd: '',
          selfEvaluationStart: '',
          selfEvaluationEnd: '',
          managerEvaluationStart: '',
          managerEvaluationEnd: '',
          templateId: this.templates()[0]?.id ?? '',
          ratingScaleId: this.ratingScales()[0]?.id ?? '',
          includedEmployees: '',
          createdBy: '00000000-0000-0000-0000-000000000000'
        });
        this.snackbar.open('Cycle created', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  activateCycle(cycle: PerformanceCycleDto): void {
    this.loading.set(true);
    this.http.post<PerformanceCycleDto>(`${this.baseUrl}/cycles/${cycle.id}/activate`, {}).subscribe({
      next: (activated) => {
        this.updateCycleState(activated);
        this.loadEvaluations(activated.id);
        this.snackbar.open('Cycle activated', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  closeCycle(cycle: PerformanceCycleDto): void {
    this.loading.set(true);
    this.http.post<PerformanceCycleDto>(`${this.baseUrl}/cycles/${cycle.id}/close`, {}).subscribe({
      next: (closed) => {
        this.updateCycleState(closed);
        this.snackbar.open('Cycle closed', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  selectCycle(cycle: PerformanceCycleDto): void {
    this.selectedCycle.set(cycle);
    this.loadEvaluations(cycle.id);
  }

  selectEvaluation(summary: EvaluationSummaryDto): void {
    this.loading.set(true);
    this.http.get<EvaluationDto>(`${this.baseUrl}/evaluations/${summary.id}`).subscribe({
      next: (evaluation) => {
        this.selectedEvaluation.set(evaluation);
        this.buildEvaluationForms(evaluation);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  submitSelf(): void {
    const evaluation = this.selectedEvaluation();
    if (!evaluation) {
      return;
    }

    const payload = this.buildSubmissionPayload(this.selfForm, true);
    this.loading.set(true);
    this.http.put<EvaluationDto>(`${this.baseUrl}/evaluations/${evaluation.id}/self`, payload).subscribe({
      next: (updated) => {
        this.selectedEvaluation.set(updated);
        this.snackbar.open('Self review submitted', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  submitManager(): void {
    const evaluation = this.selectedEvaluation();
    if (!evaluation) {
      return;
    }

    const payload = this.buildSubmissionPayload(this.managerForm, false);
    this.loading.set(true);
    this.http.put<EvaluationDto>(`${this.baseUrl}/evaluations/${evaluation.id}/manager`, payload).subscribe({
      next: (updated) => {
        this.selectedEvaluation.set(updated);
        this.snackbar.open('Manager review submitted', 'Dismiss', { duration: 2500 });
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  trackById(_: number, row: { id: string }): string {
    return row.id;
  }

  private loadRatingScales(): void {
    this.http.get<RatingScaleDto[]>(`${this.baseUrl}/templates/rating-scales`).subscribe((scales) => {
      this.ratingScales.set(scales);
      if (!this.templateForm.controls.ratingScaleId.value && scales[0]) {
        this.templateForm.controls.ratingScaleId.setValue(scales[0].id);
      }
      if (!this.cycleForm.controls.ratingScaleId.value && scales[0]) {
        this.cycleForm.controls.ratingScaleId.setValue(scales[0].id);
      }
    });
  }

  private loadTemplates(): void {
    this.http.get<EvaluationTemplateDto[]>(`${this.baseUrl}/templates`).subscribe((templates) => {
      this.templates.set(templates);
      if (!this.cycleForm.controls.templateId.value && templates[0]) {
        this.cycleForm.controls.templateId.setValue(templates[0].id);
      }
    });
  }

  private loadCycles(): void {
    this.loading.set(true);
    this.http.get<PerformanceCycleDto[]>(`${this.baseUrl}/cycles`).subscribe({
      next: (cycles) => {
        this.cycles.set(cycles);
        const firstCycle = cycles[0];
        if (firstCycle) {
          this.selectedCycle.set(firstCycle);
          this.loadEvaluations(firstCycle.id);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private loadEvaluations(cycleId: string): void {
    this.loading.set(true);
    this.http
      .get<EvaluationSummaryDto[]>(`${this.baseUrl}/cycles/${cycleId}/evaluations`)
      .subscribe({
        next: (evaluations) => {
          this.evaluations.set(evaluations);
          this.selectedEvaluation.set(null);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  private updateCycleState(updatedCycle: PerformanceCycleDto): void {
    const nextCycles = this.cycles().map((cycle) => (cycle.id === updatedCycle.id ? updatedCycle : cycle));
    this.cycles.set(nextCycles);
    if (this.selectedCycle()?.id === updatedCycle.id) {
      this.selectedCycle.set(updatedCycle);
    }
  }

  private buildTemplatePayload(): any {
    const value = this.templateForm.getRawValue();
    return {
      name: value.name,
      description: value.description,
      targetRole: value.targetRole,
      ratingScaleId: value.ratingScaleId,
      isDefault: true,
      isActive: true,
      sections: value.sections.map((section) => ({
        name: section.name,
        weight: Number(section.weight ?? 0),
        items: section.items.map((item) => ({
          name: item.name,
          description: item.description,
          defaultWeight: Number(item.defaultWeight ?? 0)
        }))
      }))
    };
  }

  private buildCyclePayload(): any {
    const value = this.cycleForm.getRawValue();
    const includedEmployees = value.includedEmployees
      .split(/\n|;/)
      .map((line) => line.trim())
      .filter((line) => !!line)
      .map((line) => {
        const [employeeId, managerId, department] = line.split(',').map((segment) => segment.trim());
        return {
          employeeId: this.parseGuid(employeeId),
          managerId: managerId ? this.parseGuid(managerId) : undefined,
          department: department ?? ''
        };
      });

    return {
      name: value.name,
      description: value.description,
      periodStart: value.periodStart,
      periodEnd: value.periodEnd,
      selfEvaluationStart: value.selfEvaluationStart,
      selfEvaluationEnd: value.selfEvaluationEnd,
      managerEvaluationStart: value.managerEvaluationStart,
      managerEvaluationEnd: value.managerEvaluationEnd,
      templateId: value.templateId,
      ratingScaleId: value.ratingScaleId,
      createdBy: value.createdBy,
      includedEmployees
    };
  }

  private buildEvaluationForms(evaluation: EvaluationDto): void {
    this.selfSections.clear();
    this.managerSections.clear();

    evaluation.sections.forEach((section) => {
      const sectionGroup: EvaluationSectionFormGroup = this.fb.group({
        sectionId: this.fb.nonNullable.control(section.id),
        items: this.fb.array<EvaluationItemFormGroup>([])
      });

      section.items.forEach((item) => {
        sectionGroup.controls.items.push(
          this.fb.group({
            itemId: this.fb.nonNullable.control(item.id),
            selfScore: this.fb.control<number | null>(item.selfScore ?? null),
            selfComment: this.fb.nonNullable.control(item.selfComment ?? ''),
            managerScore: this.fb.control<number | null>(item.managerScore ?? null),
            managerComment: this.fb.nonNullable.control(item.managerComment ?? '')
          }) as EvaluationItemFormGroup
        );
      });

      this.selfSections.push(sectionGroup);
      this.managerSections.push(sectionGroup);
    });

    this.selfForm.controls.comments.setValue(evaluation.finalCommentsEmployee ?? '');
    this.managerForm.controls.comments.setValue(evaluation.finalCommentsManager ?? '');
  }

  private buildSubmissionPayload(form: EvaluationForm, isSelf: boolean): any {
    const value = form.getRawValue();
    return {
      comments: value.comments,
      sections: value.sections.map((section) => ({
        sectionId: section.sectionId,
        items: section.items.map((item) => ({
          itemId: item.itemId,
          score: isSelf ? item.selfScore : item.managerScore,
          comment: isSelf ? item.selfComment : item.managerComment
        }))
      }))
    };
  }

  private parseGuid(value: string): string {
    const trimmed = value?.trim();
    const guidRegex = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
    if (guidRegex.test(trimmed)) {
      return trimmed;
    }
    return crypto.randomUUID();
  }
}
