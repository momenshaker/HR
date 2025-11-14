import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  effect,
  inject,
  signal
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { LookupStore } from '@core/lookups/lookup.store';
import { LookupValue } from '@core/lookups/lookup.types';
import { LookupValueDialogComponent } from './lookup-value-dialog.component';

@Component({
  selector: 'app-lookups-page',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatListModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatTableModule
  ],
  templateUrl: './lookups.component.html',
  styleUrls: ['./lookups.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LookupsPageComponent implements OnInit {
  private readonly store = inject(LookupStore);
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);

  readonly categories = this.store.categories;
  readonly isLoading = this.store.isLoading;
  readonly selectedCategory = signal<string | null>(null);
  readonly values = computed(() => {
    const category = this.selectedCategory();
    if (!category) {
      return [];
    }
    return this.store.getValues(category);
  });

  readonly displayedColumns = ['displayName', 'code', 'sortOrder', 'isActive', 'updatedAtUtc', 'actions'] as const;
  readonly selectedValueId = signal<string | null>(null);
  readonly valueDetail = signal<LookupValue | null>(null);
  readonly valueLoading = signal(false);

  constructor() {
    effect(
      () => {
        const available = this.categories();
        const current = this.selectedCategory();
        if (!available.length) {
          if (current !== null) {
            this.selectedCategory.set(null);
          }
          return;
        }

        if (!current || !available.includes(current)) {
          this.selectedCategory.set(available[0] ?? null);
        }
      },
      { allowSignalWrites: true }
    );
    effect(() => {
      const category = this.selectedCategory();
      if (!category) {
        this.clearValueSelection();
        return;
      }
      this.loadCategoryValues(category);
    });
  }

  ngOnInit(): void {
    this.store
      .load()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: (error) => this.handleError('Failed to load lookup values.', error)
      });
  }

  selectCategory(category: string): void {
    this.selectedCategory.set(category);
  }

  refresh(): void {
    this.store
      .load(true)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.snackbar.open('Lookup values refreshed', 'Dismiss', { duration: 2500 }),
        error: (error) => this.handleError('Failed to refresh lookup values.', error)
      });
  }

  openCreate(): void {
    const defaultCategory = this.selectedCategory();
    const dialogRef = this.dialog.open(LookupValueDialogComponent, {
      width: '520px',
      data: {
        title: 'Add lookup value',
        categories: this.categories(),
        defaultCategory: defaultCategory ?? '',
        nextSortOrder: this.store.nextSortOrder(defaultCategory ?? '')
      }
    });

    dialogRef
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => {
        if (!result) {
          return;
        }
        this.store
          .create(result)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => {
              this.selectedCategory.set(result.category);
              this.snackbar.open('Lookup value created', 'Dismiss', { duration: 2500 });
            },
            error: (error) => this.handleError('Unable to create lookup value.', error)
          });
      });
  }

  openEdit(value: LookupValue): void {
    const dialogRef = this.dialog.open(LookupValueDialogComponent, {
      width: '520px',
      data: {
        title: 'Edit lookup value',
        categories: this.categories(),
        value
      }
    });

    dialogRef
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => {
        if (!result) {
          return;
        }
        this.store
          .update(value.id, result)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => {
              this.selectedCategory.set(result.category);
              this.snackbar.open('Lookup value updated', 'Dismiss', { duration: 2500 });
            },
            error: (error) => this.handleError('Unable to update lookup value.', error)
          });
      });
  }

  confirmDelete(value: LookupValue): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete lookup value',
          message: `Delete ${value.displayName}?`
        }
      })
      .afterClosed()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed: boolean) => {
        if (!confirmed) {
          return;
        }
        this.store
          .delete(value.id)
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: () => this.snackbar.open('Lookup value deleted', 'Dismiss', { duration: 2500 }),
            error: (error) => this.handleError('Unable to delete lookup value.', error)
          });
      });
  }

  viewDetails(value: LookupValue): void {
    if (this.valueLoading()) {
      return;
    }
    this.selectedValueId.set(value.id);
    this.valueLoading.set(true);
    this.store
      .fetchValue(value.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (detail) => {
          this.valueDetail.set(detail);
          this.valueLoading.set(false);
        },
        error: (error) => {
          this.valueLoading.set(false);
          this.handleError('Unable to load lookup value details.', error);
        }
      });
  }

  closeDetails(): void {
    this.clearValueSelection();
  }

  getCount(category: string): number {
    return this.store.getValues(category).length;
  }

  trackByCategory(_: number, category: string): string {
    return category;
  }

  trackByValue(_: number, value: LookupValue): string {
    return value.id;
  }

  private handleError(message: string, error: unknown): void {
    // eslint-disable-next-line no-console
    console.error(message, error);
    this.snackbar.open(message, 'Dismiss', { duration: 4000 });
  }

  private loadCategoryValues(category: string): void {
    this.clearValueSelection();
    this.store
      .loadCategory(category)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        error: (error) => this.handleError(`Failed to load ${category} lookup values.`, error)
      });
  }

  private clearValueSelection(): void {
    this.selectedValueId.set(null);
    this.valueDetail.set(null);
    this.valueLoading.set(false);
  }
}
