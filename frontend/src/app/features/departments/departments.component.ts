import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';
import { DepartmentFormComponent, DepartmentFormValue } from './departments.form';

interface DepartmentSummary {
  id: string;
  name: string;
  code: string;
  organizationId: string;
  managerName?: string;
  businessUnit?: string;
  costCenterCode?: string;
  operatingHours?: string;
  budgetOwner?: string;
}

type SortOption = undefined | 'name' | '-name' | 'code' | '-code';

interface ApiResponse<T> {
  data: T;
  meta?: {
    totalItems?: number;
  };
}

@Component({
  selector: 'app-departments-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule
  ],
  templateUrl: './departments.component.html',
  styleUrls: ['./departments.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentsPageComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/departments`;

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<DepartmentSummary>>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);

  readonly pageSizeOptions = [10, 25, 50];
  readonly displayedColumns = [
    'name',
    'code',
    'managerName',
    'businessUnit',
    'costCenterCode',
    'operatingHours',
    'budgetOwner',
    'actions'
  ] as const;

  readonly sortOptions: Array<{ label: string; value?: SortOption }> = [
    { label: 'Default', value: undefined },
    { label: 'Name A→Z', value: 'name' },
    { label: 'Name Z→A', value: '-name' },
    { label: 'Code A→Z', value: 'code' },
    { label: 'Code Z→A', value: '-code' }
  ];

  search = '';
  organizationId = '';
  businessUnitFilter = '';
  sort: SortOption = undefined;

  readonly filters = computed(() => ({
    search: this.search,
    businessUnit: this.businessUnitFilter,
    sort: this.sort
  }));

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.organizationId = params.get('organizationId') ?? '';
      this.pageIndex.set(0);
      this.load();
    });
  }

  applyFilters(): void {
    this.pageIndex.set(0);
    this.load();
  }

  clearFilters(): void {
    this.search = '';
    this.businessUnitFilter = '';
    this.sort = undefined;
    this.applyFilters();
  }

  onFilterChanged(): void {
    this.pageIndex.set(0);
  }

  onPage(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.load();
  }

  openCreate(): void {
    this.dialog
      .open(DepartmentFormComponent, { width: '720px', data: { organizationId: this.organizationId } })
      .afterClosed()
      .subscribe((value?: DepartmentFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        if (!this.ensureOrganizationId()) {
          this.loading.set(false);
          return;
        }
        const params = this.organizationParams();
        this.http.post<DepartmentSummary>(this.baseUrl, value, { params }).subscribe({
          next: () => {
            this.snackbar.open('Department created', 'Dismiss', { duration: 3000 });
            this.reload();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  openEdit(item: DepartmentSummary): void {
    this.dialog
      .open(DepartmentFormComponent, {
        width: '520px',
        data: item
      })
      .afterClosed()
      .subscribe((value?: DepartmentFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        if (!this.ensureOrganizationId()) {
          this.loading.set(false);
          return;
        }
        const params = this.organizationParams();
        this.http.put<DepartmentSummary>(`${this.baseUrl}/${item.id}`, value, { params }).subscribe({
          next: () => {
            this.snackbar.open('Department updated', 'Dismiss', { duration: 3000 });
            this.reload();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  openDelete(item: DepartmentSummary): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete department',
          message: `Are you sure you want to remove ${item.name}?`
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }
        this.loading.set(true);
        if (!this.ensureOrganizationId()) {
          this.loading.set(false);
          return;
        }
        const params = this.organizationParams();
        this.http.delete<void>(`${this.baseUrl}/${item.id}`, { params }).subscribe({
          next: () => {
            this.snackbar.open('Department removed', 'Dismiss', { duration: 3000 });
            this.reload();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  viewEmployees(item: DepartmentSummary): void {
    this.router.navigate(['/employees'], { queryParams: { departmentId: item.id } });
  }

  private reload(): void {
    this.load();
  }

  private load(): void {
    if (!this.organizationId) {
      this.items.set([]);
      this.totalCount.set(0);
      return;
    }

    this.loading.set(true);
    const { search, businessUnit, sort } = this.filters();
    const sortField = sort?.replace(/^-/, '') ?? undefined;
    const direction = sort ? (sort.startsWith('-') ? 'desc' : 'asc') : undefined;
    const params = this.buildHttpParams({
      page: String(this.pageIndex() + 1),
      pageSize: String(this.pageSize()),
      search,
      sort: sortField,
      direction,
      businessUnit
    });

    this.http.get<PaginatedResponse<DepartmentSummary>>(this.baseUrl, { params }).subscribe({
      next: (response) => {
        this.items.set(response.items);
        this.totalCount.set(response.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private buildHttpParams(pairs: {
    page: string;
    pageSize: string;
    search?: string;
    sort?: string;
    direction?: 'asc' | 'desc';
    businessUnit?: string;
  }): HttpParams {
    let params = new HttpParams().set('organizationId', this.organizationId);
    params = params.set('page', pairs.page);
    params = params.set('pageSize', pairs.pageSize);
    if (pairs.search) {
      params = params.set('search', pairs.search);
    }
    if (pairs.sort) {
      params = params.set('sort', pairs.sort);
    }
    if (pairs.direction) {
      params = params.set('direction', pairs.direction);
    }
    if (pairs.businessUnit) {
      params = params.set('businessUnit', pairs.businessUnit);
    }
    return params;
  }

  private organizationParams(): HttpParams {
    return new HttpParams().set('organizationId', this.organizationId);
  }
  private ensureOrganizationId(): boolean {
    if (!this.organizationId) {
      this.snackbar.open('Select an organization before managing departments.', 'Dismiss', { duration: 3000 });
      return false;
    }
    return true;
  }
}
