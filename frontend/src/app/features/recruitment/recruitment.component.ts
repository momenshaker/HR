import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

interface VacancyDto {
  id: string;
  requisitionId: string;
  publicTitle: string;
  publicDescription: string;
  location: string;
  workMode: string;
  employmentType: string;
  salaryVisible: boolean;
  salaryRangeText: string;
  numberOfPositions: number;
  department: string;
  responsibilities: readonly string[];
  requirements: readonly string[];
  postingChannels: readonly string[];
  pipelineStages: readonly string[];
  hiringTeam: readonly string[];
  createdAtUtc: string;
  publishedAtUtc?: string | null;
  closedAtUtc?: string | null;
  status: string;
  applicationUrl: string;
}

interface CreateVacancyRequest {
  requisitionId: string;
  publicTitle: string;
  department: string;
  location: string;
  employmentType: string;
  workMode: string;
  salaryVisible: boolean;
  salaryRangeText: string;
  numberOfPositions: number;
  publicDescription: string;
  responsibilities: readonly string[];
  requirements: readonly string[];
  postingChannels: readonly string[];
  pipelineStages: readonly string[];
  hiringTeam: readonly string[];
  applicationUrl: string;
}

interface VacancyTableRow {
  id: string;
  title: string;
  department: string;
  location: string;
  workMode: string;
  employmentType: string;
  salary: string;
  positions: number;
  status: string;
  publishedAt: string;
  postingChannels: string;
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
    MatCheckboxModule,
    MatSnackBarModule,
    DataTableComponent
  ],
  templateUrl: './recruitment.component.html',
  styleUrls: ['./recruitment.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecruitmentPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/vacancies`;

  readonly form = this.fb.nonNullable.group({
    requisitionId: ['', Validators.required],
    publicTitle: ['', [Validators.required, Validators.maxLength(200)]],
    department: ['', [Validators.required, Validators.maxLength(150)]],
    location: ['', [Validators.required, Validators.maxLength(150)]],
    employmentType: ['Full-time', [Validators.required, Validators.maxLength(100)]],
    workMode: ['On-site', [Validators.maxLength(50)]],
    salaryVisible: [false],
    salaryRangeText: ['', [Validators.maxLength(200)]],
    numberOfPositions: [1, [Validators.required, Validators.min(1), Validators.max(1000)]],
    publicDescription: ['', [Validators.required, Validators.maxLength(4000)]],
    responsibilities: [''],
    requirements: [''],
    postingChannels: ['Careers site\nLinkedIn'],
    pipelineStages: ['Applied\nScreening\nInterview\nOffer'],
    hiringTeam: [''],
    applicationUrl: ['', [Validators.maxLength(500)]]
  });

  readonly loading = signal(false);
  readonly vacancies = signal<ReadonlyArray<VacancyTableRow>>([]);
  private readonly allVacancies = signal<ReadonlyArray<VacancyTableRow>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    title: 'Role',
    department: 'Department',
    location: 'Location',
    workMode: 'Work mode',
    employmentType: 'Type',
    salary: 'Salary',
    positions: 'Headcount',
    status: 'Status',
    publishedAt: 'Published',
    postingChannels: 'Channels'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  readonly employmentTypes = ['Full-time', 'Part-time', 'Contract', 'Temporary', 'Internship'] as const;
  readonly workModes = ['On-site', 'Hybrid', 'Remote'] as const;

  ngOnInit(): void {
    this.load(this.querySignal());
  }

  publish(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const payload = this.buildPayload();

    this.http.post<VacancyDto>(this.baseUrl, payload).subscribe({
      next: () => {
        this.snackbar.open('Job published', 'Dismiss', { duration: 3000 });
        this.form.reset({
          requisitionId: '',
          publicTitle: '',
          department: '',
          location: '',
          employmentType: 'Full-time',
          workMode: 'On-site',
          salaryVisible: false,
          salaryRangeText: '',
          numberOfPositions: 1,
          publicDescription: '',
          responsibilities: '',
          requirements: '',
          postingChannels: 'Careers site\nLinkedIn',
          pipelineStages: 'Applied\nScreening\nInterview\nOffer',
          hiringTeam: '',
          applicationUrl: ''
        });
        this.load(this.querySignal());
      },
      error: () => this.loading.set(false)
    });
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.render(query);
  }

  refresh(): void {
    this.load(this.querySignal());
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    this.http.get<ReadonlyArray<VacancyDto>>(this.baseUrl).subscribe({
      next: (response) => {
        const rows = response.map((vacancy) => this.toTableRow(vacancy));
        this.allVacancies.set(rows);
        this.render(query);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private buildPayload(): CreateVacancyRequest {
    const formValue = this.form.getRawValue();
    const numberOfPositions = Number(formValue.numberOfPositions) || 1;
    return {
      requisitionId: formValue.requisitionId.trim(),
      publicTitle: formValue.publicTitle,
      department: formValue.department,
      location: formValue.location,
      employmentType: formValue.employmentType,
      workMode: formValue.workMode,
      salaryVisible: formValue.salaryVisible,
      salaryRangeText: formValue.salaryRangeText ?? '',
      numberOfPositions,
      publicDescription: formValue.publicDescription,
      responsibilities: this.toCollection(formValue.responsibilities),
      requirements: this.toCollection(formValue.requirements),
      postingChannels: this.toCollection(formValue.postingChannels),
      pipelineStages: this.toCollection(formValue.pipelineStages),
      hiringTeam: this.toCollection(formValue.hiringTeam),
      applicationUrl: formValue.applicationUrl ?? ''
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

  private render(query: DataTableQuery): void {
    const filtered = this.filterRows(query.search);
    const sorted = this.sortRows(filtered, query.sortField, query.sortDirection);
    const paged = this.paginate(sorted, query.pageIndex, query.pageSize);

    this.vacancies.set(paged);
    this.total.set(filtered.length);
  }

  private filterRows(search?: string): VacancyTableRow[] {
    if (!search) {
      return [...this.allVacancies()];
    }

    const term = search.toLowerCase();
    return this.allVacancies().filter((row) =>
      [
        row.title,
        row.department,
        row.location,
        row.workMode,
        row.employmentType,
        row.status,
        row.postingChannels
      ]
        .filter(Boolean)
        .some((value) => value.toLowerCase().includes(term))
    );
  }

  private sortRows(
    rows: VacancyTableRow[],
    sortField?: string,
    sortDirection?: 'asc' | 'desc'
  ): VacancyTableRow[] {
    if (!sortField || !sortDirection) {
      return rows;
    }

    return [...rows].sort((a, b) => {
      const sortKey = sortField as keyof VacancyTableRow;
      const aValue = a[sortKey];
      const bValue = b[sortKey];

      if (typeof aValue === 'number' && typeof bValue === 'number') {
        return sortDirection === 'asc' ? aValue - bValue : bValue - aValue;
      }

      const aText = String(aValue ?? '').toLowerCase();
      const bText = String(bValue ?? '').toLowerCase();
      return sortDirection === 'asc' ? aText.localeCompare(bText) : bText.localeCompare(aText);
    });
  }

  private paginate(rows: VacancyTableRow[], pageIndex: number, pageSize: number): VacancyTableRow[] {
    const start = pageIndex * pageSize;
    return rows.slice(start, start + pageSize);
  }

  private toTableRow(vacancy: VacancyDto): VacancyTableRow {
    const salaryText = vacancy.salaryVisible
      ? vacancy.salaryRangeText || 'Visible'
      : 'Hidden';

    const publishedText = vacancy.publishedAtUtc
      ? new Date(vacancy.publishedAtUtc).toLocaleDateString()
      : 'Draft';

    return {
      id: vacancy.id,
      title: vacancy.publicTitle,
      department: vacancy.department,
      location: vacancy.location,
      workMode: vacancy.workMode,
      employmentType: vacancy.employmentType,
      salary: salaryText,
      positions: vacancy.numberOfPositions,
      status: vacancy.status,
      publishedAt: publishedText,
      postingChannels: vacancy.postingChannels.join(', ')
    };
  }
}
