import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, signal, WritableSignal, computed } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';

export interface DataTableQuery {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortDirection?: 'asc' | 'desc';
  search?: string;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    ReactiveFormsModule
  ],
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent {
  @Input({ required: true }) displayedColumns: string[] = [];
  @Input({ required: true }) columns: Record<string, string> = {};
  @Input() data: ReadonlyArray<unknown> = [];
  @Input() total = 0;
  @Input() loading = false;
  @Input() pageSizeOptions: number[] = [10, 25, 50];
  @Input() actionsTemplate?: any;

  @Output() queryChange = new EventEmitter<DataTableQuery>();

  readonly filterControl = new FormControl('', { nonNullable: true });
  private readonly querySignal: WritableSignal<DataTableQuery> = signal({ pageIndex: 0, pageSize: 10 });
  readonly query = computed(() => this.querySignal());

  constructor() {
    this.filterControl.valueChanges.pipe(debounceTime(300)).subscribe((value) => {
      this.emitQuery({ search: value, pageIndex: 0 });
    });
  }

  onPageChange(event: PageEvent): void {
    this.emitQuery({ pageIndex: event.pageIndex, pageSize: event.pageSize });
  }

  onSortChange(sort: Sort): void {
    this.emitQuery({
      sortField: sort.active,
      sortDirection: sort.direction === '' ? undefined : (sort.direction as 'asc' | 'desc')
    });
  }

  private emitQuery(partial: Partial<DataTableQuery>): void {
    const query = { ...this.querySignal(), ...partial };
    this.querySignal.set(query);
    this.queryChange.emit(query);
  }

  getCellValue(row: T, column: string): unknown {
    return (row as Record<string, unknown>)[column];
  }
}
