import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { EntityCrudFactory } from '@core/data-access';

interface VacancyDto {
  id: string;
  title: string;
  department: string;
  location: string;
  employmentType: string;
  status: string;
  postedAtUtc: string;
  closingAtUtc?: string | null;
  hiringTeam: readonly string[];
  pipelineStages: readonly string[];
}

interface CreateVacancyRequest {
  title: string;
  department: string;
  location: string;
  employmentType: string;
  description: string;
  responsibilities: readonly string[];
  requirements: readonly string[];
  pipelineStages: readonly string[];
  hiringTeam: readonly string[];
  applicationUrl: string;
  closingAtUtc?: string | null;
}

@Component({
  selector: 'app-recruitment-page',
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
    DataTableComponent
  ],
  templateUrl: './recruitment.component.html',
  styleUrls: ['./recruitment.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecruitmentPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<
    CreateVacancyRequest,
    CreateVacancyRequest,
    VacancyDto
  >('Vacancies');

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    department: ['', [Validators.required, Validators.maxLength(150)]],
    location: ['', [Validators.required, Validators.maxLength(150)]],
    employmentType: ['Full-time', Validators.required],
    description: ['', [Validators.required, Validators.maxLength(4000)]],
    responsibilities: [''],
    requirements: [''],
    pipelineStages: ['Screening\nInterview\nOffer'],
    hiringTeam: [''],
    applicationUrl: [''],
    closingAtUtc: ['']
  });

  readonly loading = signal(false);
  readonly jobs = signal<ReadonlyArray<VacancyDto>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    title: 'Role',
    department: 'Department',
    location: 'Location',
    employmentType: 'Type',
    status: 'Status',
    postedAtUtc: 'Posted',
    closingAtUtc: 'Closes'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  readonly employmentTypes = ['Full-time', 'Part-time', 'Contract', 'Temporary', 'Internship'] as const;

  publish(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const payload = this.buildPayload();

    this.service.create(payload).subscribe({
      next: () => {
        this.snackbar.open('Job published', 'Dismiss', { duration: 3000 });
        this.form.reset({
          title: '',
          department: '',
          location: '',
          employmentType: 'Full-time',
          description: '',
          responsibilities: '',
          requirements: '',
          pipelineStages: 'Screening\nInterview\nOffer',
          hiringTeam: '',
          applicationUrl: '',
          closingAtUtc: ''
        });
        this.load(this.querySignal());
      },
      error: () => this.loading.set(false)
    });
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  refresh(): void {
    this.load(this.querySignal());
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    this.service
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection
      })
      .subscribe({
        next: (response) => {
          this.jobs.set(response.data);
          this.total.set(response.meta?.totalItems ?? response.data.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  private buildPayload(): CreateVacancyRequest {
    const formValue = this.form.getRawValue();
    return {
      title: formValue.title,
      department: formValue.department,
      location: formValue.location,
      employmentType: formValue.employmentType,
      description: formValue.description,
      responsibilities: this.toCollection(formValue.responsibilities),
      requirements: this.toCollection(formValue.requirements),
      pipelineStages: this.toCollection(formValue.pipelineStages),
      hiringTeam: this.toCollection(formValue.hiringTeam),
      applicationUrl: formValue.applicationUrl ?? '',
      closingAtUtc: formValue.closingAtUtc ? new Date(formValue.closingAtUtc).toISOString() : null
    };
  }

  private toCollection(value?: string | null): readonly string[] {
    if (!value) {
      return [];
    }
    return value
      .split(/\r?\n|,/)
      .map((item) => item.trim())
      .filter((item) => item.length > 0);
  }
}
