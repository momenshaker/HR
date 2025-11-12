import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { EntityCrudFactory } from '@core/data-access';
import { EmployeeFormComponent, EmployeeFormValue } from './employees.form';

interface EmployeeSummary {
  id: string;
  fullName: string;
  email: string;
  departmentName?: string;
  jobTitle?: string;
  status?: string;
}

@Component({
  selector: 'app-employees-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeesPageComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly router = inject(Router);
  private readonly service = inject(EntityCrudFactory).create<EmployeeFormValue, EmployeeFormValue, EmployeeSummary>('employees');

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<EmployeeSummary>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    fullName: 'Name',
    email: 'Email',
    departmentName: 'Department',
    jobTitle: 'Job Title',
    status: 'Status',
    actions: 'Actions'
  } as const;

  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.load(this.querySignal());
  }

  view(item: EmployeeSummary): void {
    this.router.navigate(['/employees', item.id]);
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  create(): void {
    this.dialog
      .open(EmployeeFormComponent, {
        width: '520px'
      })
      .afterClosed()
      .subscribe((value?: EmployeeFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        this.service.create(value).subscribe({
          next: () => {
            this.snackbar.open('Employee created', 'Dismiss', { duration: 3000 });
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  edit(item: EmployeeSummary): void {
    this.dialog
      .open(EmployeeFormComponent, {
        width: '520px',
        data: item
      })
      .afterClosed()
      .subscribe((value?: EmployeeFormValue) => {
        if (!value) {
          return;
        }
        this.loading.set(true);
        this.service.update(item.id, value).subscribe({
          next: () => {
            this.snackbar.open('Employee updated', 'Dismiss', { duration: 3000 });
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  remove(item: EmployeeSummary): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete employee',
          message: `Deactivate ${item.fullName}?`
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
            this.snackbar.open('Employee removed', 'Dismiss', { duration: 3000 });
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
