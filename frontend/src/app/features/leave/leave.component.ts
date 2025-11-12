import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { EntityCrudFactory } from '@core/data-access';

interface LeaveRequest {
  id: string;
  employeeName: string;
  type: string;
  startDate: string;
  endDate: string;
  status: string;
}

@Component({
  selector: 'app-leave-requests-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    DataTableComponent
  ],
  templateUrl: './leave.component.html',
  styleUrls: ['./leave.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LeaveRequestsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<any, any, LeaveRequest>('leave/requests');

  readonly form = this.fb.nonNullable.group({
    typeId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    reason: ['']
  });

  readonly loading = signal(false);
  readonly requests = signal<ReadonlyArray<LeaveRequest>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    employeeName: 'Employee',
    type: 'Type',
    startDate: 'Start',
    endDate: 'End',
    status: 'Status'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.service.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.snackbar.open('Leave request submitted', 'Dismiss', { duration: 3000 });
        this.form.reset({ typeId: '', startDate: '', endDate: '', reason: '' });
        this.load(this.querySignal());
      },
      error: () => this.loading.set(false)
    });
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
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
          this.requests.set(response.data);
          this.total.set(response.meta?.totalItems ?? response.data.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }
}
