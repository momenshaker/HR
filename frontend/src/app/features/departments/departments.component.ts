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
import { EntityCrudFactory } from '@core/data-access';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { DepartmentFormComponent, DepartmentFormValue } from './departments.form';

interface DepartmentSummary {
  id: string;
  name: string;
  code: string;
  organizationName?: string;
  managerName?: string;
  businessUnit?: string;
  costCenterCode?: string;
  operatingHours?: string;
  budgetOwner?: string;
}

type SortOption = undefined | 'name' | '-name' | 'code' | '-code';

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
  private readonly service = inject(EntityCrudFactory).create<DepartmentFormValue, DepartmentFormValue, DepartmentSummary>(
    'departments'
  );

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<DepartmentSummary>>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);

  readonly pageSizeOptions = [10, 25, 50];
  readonly displayedColumns = [
    'name',
    'code',
    'organizationName',
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
  organizationFilter = '';
  businessUnitFilter = '';
  sort: SortOption = undefined;

  readonly filters = computed(() => ({
    search: this.search,
    organization: this.organizationFilter,
    businessUnit: this.businessUnitFilter,
    sort: this.sort
  }));

  ngOnInit(): void {
    this.load();
  }

  applyFilters(): void {
    this.pageIndex.set(0);
    this.load();
  }

  clearFilters(): void {
    this.search = '';
    this.organizationFilter = '';
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
      .open(DepartmentFormComponent, { width: '720px' })
      .afterClosed()
      .subscribe((value?: DepartmentFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        this.service.create(value).subscribe({
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
        this.service.update(item.id, value).subscribe({
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
        this.service.delete(item.id).subscribe({
          next: () => {
            this.snackbar.open('Department removed', 'Dismiss', { duration: 3000 });
            this.reload();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private reload(): void {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    const { search, organization, businessUnit, sort } = this.filters();
    const sortField = sort?.replace(/^-/, '') ?? undefined;
    const direction = sort ? (sort.startsWith('-') ? 'desc' : 'asc') : undefined;
    const filters: Record<string, string> = {};
    if (organization) {
      filters['organizationName'] = organization;
    }
    if (businessUnit) {
      filters['businessUnit'] = businessUnit;
    }

    this.service
      .list({
        page: this.pageIndex() + 1,
        pageSize: this.pageSize(),
        search: search || undefined,
        sort: sortField,
        direction,
        filters
      })
      .subscribe({
        next: (response) => {
          const normalized = response as unknown;
          const payload =
            Array.isArray(normalized)
              ? normalized
              : 'data' in (normalized as Record<string, unknown>)
              ? (normalized as { data?: DepartmentSummary[] }).data ?? []
              : [];
          const totalItems =
            Array.isArray(normalized)
              ? normalized.length
              : 'meta' in (normalized as Record<string, unknown>)
              ? (normalized as { meta?: { totalItems?: number } }).meta?.totalItems ?? payload.length
              : payload.length;

          this.items.set(payload);
          this.totalCount.set(totalItems);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }
}
