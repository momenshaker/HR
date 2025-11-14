import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

export interface LeaveActionDialogData {
  title: string;
  message: string;
  confirmLabel: string;
  placeholder?: string;
}

@Component({
  selector: 'app-leave-action-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, ReactiveFormsModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content>
      <p>{{ data.message }}</p>
      <mat-form-field appearance="outline" class="dialog-field">
        <mat-label>{{ data.placeholder ?? 'Reason' }}</mat-label>
        <textarea matInput rows="3" [formControl]="form.controls.reason"></textarea>
        <mat-error *ngIf="form.controls.reason.hasError('required')">Reason is required</mat-error>
      </mat-form-field>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-stroked-button type="button" mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary" type="button" (click)="submit()" [disabled]="form.invalid">
        {{ data.confirmLabel }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .dialog-field {
        width: 100%;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LeaveActionDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<LeaveActionDialogComponent, string | undefined>);
  readonly form = inject(FormBuilder).nonNullable.group({
    reason: ['', Validators.required]
  });
  readonly data = inject<LeaveActionDialogData>(MAT_DIALOG_DATA);

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.form.controls.reason.value.trim());
  }
}
