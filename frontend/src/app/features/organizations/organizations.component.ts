import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { EntityCrudFactory } from '@core/data-access';
import { OrganizationFormComponent, OrganizationFormValue } from './organizations.form';

export interface OrganizationSummary {
  id: string;
  name: string;
  code: string;
  address?: string;
  createdAt?: string;
}

@Component({
  selector: 'app-organizations-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './organizations.component.html',
  styleUrls: ['./organizations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrganizationsPageComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<OrganizationFormValue, OrganizationFormValue, OrganizationSummary>(
    'organizations'
  );

  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });
  readonly query = this.querySignal.asReadonly();

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<OrganizationSummary>>([]);
  readonly total = signal(0);

  readonly columns = {
    name: 'Name',
    code: 'Code',
    address: 'Address',
    createdAt: 'Created',
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
      .open(OrganizationFormComponent, {
        width: '480px'
      })
      .afterClosed()
      .subscribe((value?: OrganizationFormValue) => {
        if (value) {
          this.loading.set(true);
          this.service.create(value).subscribe({
            next: () => {
              this.snackbar.open('Organization created', 'Dismiss', { duration: 3000 });
              this.load(this.querySignal());
            },
            error: () => this.loading.set(false)
          });
        }
      });
  }

  edit(item: OrganizationSummary): void {
    this.dialog
      .open(OrganizationFormComponent, {
        width: '480px',
        data: item
      })
      .afterClosed()
      .subscribe((value?: OrganizationFormValue) => {
        if (value) {
          this.loading.set(true);
          this.service.update(item.id, value).subscribe({
            next: () => {
              this.snackbar.open('Organization updated', 'Dismiss', { duration: 3000 });
              this.load(this.querySignal());
            },
            error: () => this.loading.set(false)
          });
        }
      });
  }

  remove(item: OrganizationSummary): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete organization',
          message: `Are you sure you want to remove ${item.name}?`
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (confirmed) {
          this.loading.set(true);
          this.service.delete(item.id).subscribe({
            next: () => {
              this.snackbar.open('Organization removed', 'Dismiss', { duration: 3000 });
              this.load(this.querySignal());
            },
            error: () => this.loading.set(false)
          });
        }
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
