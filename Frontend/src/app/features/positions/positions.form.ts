import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';
import { EmployeeOption, OrganizationUnitSummary, PositionSummary } from './positions.models';

export interface PositionFormValue {
  title: string;
  jobCode: string;
  organizationUnitId: string;
  reportsToPositionId?: string;
  occupiedByEmployeeId?: string;
  grade: string;
  employmentType: string;
  effectiveFrom?: string;
  effectiveTo?: string;
  isCriticalRole: boolean;
  isVacant: boolean;
}

export interface PositionFormDialogData {
  value?: PositionFormValue;
  organizationUnits: readonly OrganizationUnitSummary[];
  positions: readonly PositionSummary[];
  employees: readonly EmployeeOption[];
  defaultOrganizationUnitId?: string;
}

@Component({
  selector: 'app-position-form',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    FormActionsComponent
  ],
  template: `
    <h2 mat-dialog-title>{{ data.value ? 'Edit position' : 'Create position' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()" class="position-form">
      <mat-dialog-content>
        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Title</mat-label>
              <input matInput formControlName="title" />
              <mat-error *ngIf="form.controls.title.hasError('required')">Title is required</mat-error>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Job code</mat-label>
              <input matInput formControlName="jobCode" />
              <mat-error *ngIf="form.controls.jobCode.hasError('required')">Job code is required</mat-error>
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-12">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Organization unit</mat-label>
              <mat-select formControlName="organizationUnitId">
                <mat-option *ngFor="let unit of organizationUnits" [value]="unit.id">
                  {{ unit.name }}
                </mat-option>
              </mat-select>
              <mat-error *ngIf="form.controls.organizationUnitId.hasError('required')">
                Organization unit is required
              </mat-error>
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Reports to</mat-label>
              <mat-select formControlName="reportsToPositionId">
                <mat-option value="">None</mat-option>
                <mat-option *ngFor="let option of positions" [value]="option.id">
                  {{ option.title }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Occupied by</mat-label>
              <mat-select formControlName="occupiedByEmployeeId">
                <mat-option value="">None</mat-option>
                <mat-option *ngFor="let employee of employees" [value]="employee.id">
                  {{ employee.firstName }} {{ employee.lastName }}{{ employee.jobTitle ? ' - ' + employee.jobTitle : '' }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Grade</mat-label>
              <input matInput formControlName="grade" />
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Employment type</mat-label>
              <input matInput formControlName="employmentType" />
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Effective from</mat-label>
              <input matInput type="date" formControlName="effectiveFrom" />
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Effective to</mat-label>
              <input matInput type="date" formControlName="effectiveTo" />
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-checkbox formControlName="isVacant">Vacant</mat-checkbox>
          </div>
          <div class="col-md-6">
            <mat-checkbox formControlName="isCriticalRole">Critical role</mat-checkbox>
          </div>
        </div>

        <app-form-actions submitLabel="Save" [onCancel]="close"></app-form-actions>
      </mat-dialog-content>
    </form>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PositionFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<PositionFormComponent>);

  readonly form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    jobCode: ['', Validators.required],
    organizationUnitId: ['', Validators.required],
    reportsToPositionId: [''],
    occupiedByEmployeeId: [''],
    grade: [''],
    employmentType: [''],
    effectiveFrom: [''],
    effectiveTo: [''],
    isCriticalRole: [false],
    isVacant: [true]
  });

  readonly organizationUnits = this.data.organizationUnits;
  readonly positions = this.data.positions;
  readonly employees = this.data.employees;

  constructor(@Inject(MAT_DIALOG_DATA) public readonly data: PositionFormDialogData) {
    if (data.value) {
      this.form.patchValue({
        ...data.value,
        reportsToPositionId: data.value.reportsToPositionId ?? '',
        occupiedByEmployeeId: data.value.occupiedByEmployeeId ?? '',
        effectiveFrom: data.value.effectiveFrom ?? '',
        effectiveTo: data.value.effectiveTo ?? ''
      });
    } else if (data.defaultOrganizationUnitId) {
      this.form.controls.organizationUnitId.setValue(data.defaultOrganizationUnitId);
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
