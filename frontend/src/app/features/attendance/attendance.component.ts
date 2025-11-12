import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { DateRangePickerComponent } from '@shared/components/date-range-picker/date-range-picker.component';
import { EntityCrudFactory } from '@core/data-access';

interface AttendanceRecord {
  id: string;
  employeeName: string;
  date: string;
  status: string;
  checkIn?: string;
  checkOut?: string;
}

@Component({
  selector: 'app-attendance-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatIconModule, DataTableComponent, DateRangePickerComponent],
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AttendancePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<never, never, AttendanceRecord>('attendance');

  readonly filterForm = this.fb.nonNullable.group({
    start: [''],
    end: ['']
  });

  readonly loading = signal(false);
  readonly records = signal<ReadonlyArray<AttendanceRecord>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });

  readonly columns = {
    employeeName: 'Employee',
    date: 'Date',
    status: 'Status',
    checkIn: 'Check-in',
    checkOut: 'Check-out'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.load(this.querySignal());
    this.filterForm.valueChanges.subscribe(() => this.onFiltersChanged());
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  onFiltersChanged(): void {
    this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
    this.load(this.querySignal());
  }

  refresh(): void {
    this.snackbar.open('Attendance refreshed', 'Dismiss', { duration: 2000 });
    this.load(this.querySignal());
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    const { start, end } = this.filterForm.getRawValue();
    this.service
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection,
        filters: {
          start,
          end
        }
      })
      .subscribe({
        next: (response) => {
          this.records.set(response.data);
          this.total.set(response.meta?.totalItems ?? response.data.length);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }
}
