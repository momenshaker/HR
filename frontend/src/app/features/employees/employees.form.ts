import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';

export interface EmployeeFormValue {
  firstName: string;
  lastName: string;
  email: string;
  departmentId?: string;
  jobTitle?: string;
  employmentType?: string;
  startDate?: string;
}

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatInputModule, MatSelectModule, FormActionsComponent],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit employee' : 'Add employee' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="form" (ngSubmit)="submit()" class="form-grid">
        <mat-form-field appearance="outline">
          <mat-label>First name</mat-label>
          <input matInput formControlName="firstName" />
          <mat-error *ngIf="form.controls.firstName.hasError('required')">First name is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Last name</mat-label>
          <input matInput formControlName="lastName" />
          <mat-error *ngIf="form.controls.lastName.hasError('required')">Last name is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" />
          <mat-error *ngIf="form.controls.email.hasError('required')">Email is required</mat-error>
          <mat-error *ngIf="form.controls.email.hasError('email')">Provide a valid email</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Job title</mat-label>
          <input matInput formControlName="jobTitle" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Employment type</mat-label>
          <mat-select formControlName="employmentType">
            <mat-option value="FullTime">Full-time</mat-option>
            <mat-option value="PartTime">Part-time</mat-option>
            <mat-option value="Contractor">Contractor</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Start date</mat-label>
          <input matInput type="date" formControlName="startDate" />
        </mat-form-field>
        <app-form-actions submitLabel="Save" [onCancel]="close"></app-form-actions>
      </form>
    </mat-dialog-content>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        gap: 1rem;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<EmployeeFormComponent>);

  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    departmentId: [''],
    jobTitle: [''],
    employmentType: ['FullTime'],
    startDate: ['']
  });

  constructor(@Inject(MAT_DIALOG_DATA) readonly data: Partial<EmployeeFormValue> | null) {
    if (data) {
      this.form.patchValue(data);
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.form.getRawValue());
  }

  readonly close = () => this.dialogRef.close();
}
