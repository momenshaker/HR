import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, UntypedFormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { LookupValue, LookupValuePayload } from '@core/lookups/lookup.types';

export interface LookupValueDialogData {
  title: string;
  categories: readonly string[];
  defaultCategory?: string;
  nextSortOrder?: number;
  value?: LookupValue;
}

@Component({
  selector: 'app-lookup-value-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule
  ],
  templateUrl: './lookup-value-dialog.component.html',
  styleUrls: ['./lookup-value-dialog.component.scss']
})
export class LookupValueDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<LookupValueDialogComponent, LookupValuePayload | undefined>);
  private readonly fb = inject(UntypedFormBuilder);

  readonly data = inject<LookupValueDialogData>(MAT_DIALOG_DATA);
  readonly form = this.fb.group({
    category: [this.data.value?.category ?? this.data.defaultCategory ?? '', [Validators.required, Validators.maxLength(100)]],
    code: [this.data.value?.code ?? '', [Validators.required, Validators.maxLength(100)]],
    displayName: [this.data.value?.displayName ?? '', [Validators.required, Validators.maxLength(200)]],
    description: [this.data.value?.description ?? '', [Validators.maxLength(512)]],
    sortOrder: [this.data.value?.sortOrder ?? this.data.nextSortOrder ?? 1, [Validators.min(1)]],
    isActive: [this.data.value?.isActive ?? true]
  });

  readonly categories = this.data.categories;

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const normalizedSortOrder = Number(value.sortOrder);
    const payload: LookupValuePayload = {
      category: value.category?.trim() ?? '',
      code: value.code?.trim() ?? '',
      displayName: value.displayName?.trim() ?? '',
      description: value.description?.trim() ? value.description.trim() : null,
      sortOrder: Number.isNaN(normalizedSortOrder) || normalizedSortOrder <= 0 ? undefined : normalizedSortOrder,
      isActive: value.isActive ?? true
    };

    this.dialogRef.close(payload);
  }
}
