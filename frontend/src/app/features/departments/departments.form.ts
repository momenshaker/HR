import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';
import { EntityCrudFactory } from '@core/data-access';
import { LookupStore } from '@core/lookups/lookup.store';

export interface DepartmentFormValue {
  name: string;
  code: string;
  organizationId?: string;
  parentDepartmentId?: string;
  managerId?: string;
  branch: string;
  location: string;
  businessUnit: string;
  costCenterCode: string;
  operatingHours: string;
  budgetOwner: string;
  description: string;
  isActive: boolean;
}

interface OrganizationSummary {
  id: string;
  name: string;
}

@Component({
  selector: 'app-department-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    FormActionsComponent
  ],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit department' : 'Create department' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()" class="example-full-width">
      <mat-dialog-content>
        <div class="row">
          <div class="col-md-6 item-full-width">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Name</mat-label>
              <input matInput formControlName="name" />
              <mat-error *ngIf="form.controls.name.hasError('required')">Name is required</mat-error>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Code</mat-label>
              <input matInput formControlName="code" />
              <mat-error *ngIf="form.controls.code.hasError('required')">Code is required</mat-error>
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Organization</mat-label>
              <mat-select formControlName="organizationId">
                <mat-option *ngFor="let org of organizations" [value]="org.id">{{ org.name }}</mat-option>
              </mat-select>
              <mat-error *ngIf="form.controls.organizationId.hasError('required')">Organization is required</mat-error>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Parent department</mat-label>
              <input matInput formControlName="parentDepartmentId" />
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Branch</mat-label>
              <mat-select formControlName="branch">
                <mat-option value="">None</mat-option>
                <mat-option *ngFor="let option of branchOptions()" [value]="option">
                  {{ option }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Location</mat-label>
              <input matInput formControlName="location" />
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Business unit</mat-label>
              <mat-select formControlName="businessUnit">
                <mat-option value="">None</mat-option>
                <mat-option *ngFor="let option of businessUnitOptions()" [value]="option">
                  {{ option }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Cost center</mat-label>
              <input matInput formControlName="costCenterCode" />
            </mat-form-field>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Operating hours</mat-label>
              <mat-select formControlName="operatingHours">
                <mat-option value="">None</mat-option>
                <mat-option *ngFor="let option of operatingHoursOptions()" [value]="option">
                  {{ option }}
                </mat-option>
              </mat-select>
            </mat-form-field>
          </div>
          <div class="col-md-6">
            <mat-form-field appearance="outline" class="example-full-width">
              <mat-label>Budget owner</mat-label>
              <input matInput formControlName="budgetOwner" />
            </mat-form-field>
          </div>
        </div>

        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description"></textarea>
        </mat-form-field>

        <mat-checkbox formControlName="isActive">Active</mat-checkbox>

        <app-form-actions submitLabel="Save" [onCancel]="close"></app-form-actions>
      </mat-dialog-content>
    </form>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<DepartmentFormComponent>);
  private readonly lookupStore = inject(LookupStore);
  private readonly organizationRequester = inject(EntityCrudFactory).create<never, never, OrganizationSummary>(
    'organizations'
  );

  organizations: OrganizationSummary[] = [];

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    code: ['', Validators.required],
    organizationId: ['', Validators.required],
    parentDepartmentId: [''],
    managerId: [''],
    branch: [''],
    location: [''],
    businessUnit: [''],
    costCenterCode: [''],
    operatingHours: [''],
    budgetOwner: [''],
    description: [''],
    isActive: [true]
  });

  readonly branchOptions = this.lookupStore.branches;
  readonly businessUnitOptions = this.lookupStore.businessUnits;
  readonly operatingHoursOptions = this.lookupStore.operatingHours;

  constructor(@Inject(MAT_DIALOG_DATA) readonly data: Partial<DepartmentFormValue> | null) {
    this.loadOrganizations();
    if (data) {
      this.form.patchValue(data);
    }
  }

  private loadOrganizations(): void {
    this.organizationRequester.list({ pageSize: 250 }).subscribe({
      next: (response) => {
        this.organizations.splice(0, this.organizations.length, ...(response.data ?? []));
      },
      error: () => {
        // ignore lookup failures for UX
      }
    });
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
