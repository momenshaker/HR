import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { EntityCrudFactory } from '@core/data-access';
import { DepartmentFormComponent, DepartmentFormValue } from './departments.form';

interface DepartmentSummary {
  id: string;
  name: string;
  code: string;
  organizationName?: string;
  managerName?: string;
}

@Component({
  selector: 'app-departments-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, MatButtonModule, MatIconModule, MatDialogModule],
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
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    name: 'Name',
    code: 'Code',
    organizationName: 'Organization',
    managerName: 'Manager',
    actions: 'Actions'
  } as const;

  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.load(this.querySignal());
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  create(): void {
    this.dialog
      .open(DepartmentFormComponent, {
        width: '480px'
      })
      .afterClosed()
      .subscribe((value?: DepartmentFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        this.service.create(value).subscribe({
          next: () => {
            this.snackbar.open('Department created', 'Dismiss', { duration: 3000 });
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  edit(item: DepartmentSummary): void {
    this.dialog
      .open(DepartmentFormComponent, {
        width: '480px',
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
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  remove(item: DepartmentSummary): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete department',
          message: `Delete ${item.name}?`
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
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
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
          this.items.set(response.data);
          this.total.set(response.meta?.totalItems ?? response.data.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }
}
