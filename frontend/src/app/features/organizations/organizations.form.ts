import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';

export interface OrganizationFormValue {
  name: string;
  code: string;
  address?: string;
}

@Component({
  selector: 'app-organization-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatInputModule, FormActionsComponent],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit organization' : 'Create organization' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <mat-form-field appearance="outline">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" />
          <mat-error *ngIf="form.controls.name.hasError('required')">Name is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Code</mat-label>
          <input matInput formControlName="code" />
          <mat-error *ngIf="form.controls.code.hasError('required')">Code is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Address</mat-label>
          <textarea matInput formControlName="address"></textarea>
        </mat-form-field>
        <app-form-actions submitLabel="Save" [onCancel]="close"></app-form-actions>
      </form>
    </mat-dialog-content>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrganizationFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<OrganizationFormComponent>);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    code: ['', Validators.required],
    address: ['']
  });

  constructor(@Inject(MAT_DIALOG_DATA) readonly data: OrganizationFormValue | null) {
    if (data) {
      this.form.patchValue(data);
    }
  }

  get value(): OrganizationFormValue {
    return this.form.getRawValue();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.value);
  }

  readonly close = () => this.dialogRef.close();
}
