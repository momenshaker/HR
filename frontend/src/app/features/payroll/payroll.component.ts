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

interface PayrollRun {
  id: string;
  period: string;
  status: string;
  processedOn: string;
  totalEmployees: number;
}

@Component({
  selector: 'app-payroll-page',
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
  templateUrl: './payroll.component.html',
  styleUrls: ['./payroll.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PayrollPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<any, any, PayrollRun>('payroll/runs');

  readonly form = this.fb.nonNullable.group({
    period: ['', Validators.required],
    payDate: ['', Validators.required],
    bankFile: ['']
  });

  readonly loading = signal(false);
  readonly runs = signal<ReadonlyArray<PayrollRun>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    period: 'Period',
    status: 'Status',
    processedOn: 'Processed on',
    totalEmployees: 'Employees'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  generate(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.service.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.snackbar.open('Payroll run created', 'Dismiss', { duration: 3000 });
        this.form.reset({ period: '', payDate: '', bankFile: '' });
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
          this.runs.set(response.data);
          this.total.set(response.meta?.totalItems ?? response.data.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }
}
