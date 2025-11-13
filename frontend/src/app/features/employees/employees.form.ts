import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { EntityCrudFactory } from '@core/data-access';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';

export interface EmployeeProfileDocumentInput {
  fileName: string;
  storagePath: string;
  description?: string;
  contentType?: string;
  uploadedAtUtc?: string;
}

export interface EmployeeFormValue {
  firstName: string;
  lastName: string;
  email: string;
  jobTitle?: string;
  phoneNumber?: string;
  employmentType?: 'FullTime' | 'PartTime' | 'Contractor';
  employmentStartDate?: string;
  employmentEndDate?: string;
  departmentAssignment: {
    primaryDepartmentId: string;
    secondaryDepartmentIds: string[];
  };
  profileDocuments: EmployeeProfileDocumentInput[];
}

interface DepartmentSummary {
  id: string;
  name: string;
}

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    FormActionsComponent
  ],
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
          <mat-label>Phone</mat-label>
          <input matInput type="tel" formControlName="phoneNumber" />
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
          <mat-label>Primary department</mat-label>
          <mat-select formControlName="departmentId">
            <mat-option *ngFor="let department of departments" [value]="department.id">
              {{ department.name }}
            </mat-option>
          </mat-select>
          <mat-error *ngIf="form.controls.departmentId.hasError('required')">Department is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Start date</mat-label>
          <input matInput type="date" formControlName="startDate" />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>End date</mat-label>
          <input matInput type="date" formControlName="endDate" />
        </mat-form-field>

        <section class="documents" formArrayName="profileDocuments">
          <h3>Profile documents</h3>
          <div
            *ngFor="let group of profileDocumentControls.controls; let i = index"
            [formGroupName]="i"
            class="document-row"
          >
            <mat-form-field appearance="outline">
              <mat-label>File name</mat-label>
              <input matInput formControlName="fileName" />
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Storage path / URL</mat-label>
              <input matInput formControlName="storagePath" />
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Description (optional)</mat-label>
              <input matInput formControlName="description" />
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Content type</mat-label>
              <input matInput formControlName="contentType" />
            </mat-form-field>

            <button mat-icon-button type="button" color="warn" (click)="removeDocument(i)">
              <mat-icon>delete</mat-icon>
            </button>
          </div>
          <button mat-mini-button type="button" (click)="addDocument()">
            <mat-icon>add</mat-icon>
            Add document
          </button>
        </section>

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

      .documents {
        border: 1px solid var(--ion-color-surface-border, rgba(0, 0, 0, 0.12));
        border-radius: 4px;
        padding: 1rem;
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
      }

      .document-row {
        display: grid;
        gap: 0.75rem;
        grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
        align-items: center;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<EmployeeFormComponent>);
  private readonly departmentRequester = inject(EntityCrudFactory).create<never, never, DepartmentSummary>('departments');

  departments: DepartmentSummary[] = [];
  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    jobTitle: [''],
    employmentType: ['FullTime'],
    departmentId: ['', Validators.required],
    startDate: [''],
    endDate: [''],
    profileDocuments: this.fb.array<FormGroup>([])
  });

  constructor(@Inject(MAT_DIALOG_DATA) readonly data: Partial<EmployeeFormValue> | null) {
    this.loadDepartments();
    if (data) {
      this.patchData(data);
    }
  }

  get profileDocumentControls(): FormArray {
    return this.form.controls.profileDocuments as FormArray;
  }

  addDocument(document?: EmployeeProfileDocumentInput): void {
    this.profileDocumentControls.push(this.buildDocumentGroup(document));
  }

  removeDocument(index: number): void {
    this.profileDocumentControls.removeAt(index);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const documents = (raw.profileDocuments as EmployeeProfileDocumentInput[])
      .map((document) => ({
        fileName: document.fileName?.trim() ?? '',
        storagePath: document.storagePath?.trim() ?? '',
        description: document.description?.trim(),
        contentType: document.contentType?.trim(),
        uploadedAtUtc: document.uploadedAtUtc ?? new Date().toISOString()
      }))
      .filter((document) => document.fileName && document.storagePath);

    this.dialogRef.close({
      firstName: raw.firstName.trim(),
      lastName: raw.lastName.trim(),
      email: raw.email.trim(),
      jobTitle: raw.jobTitle?.trim(),
      phoneNumber: raw.phoneNumber?.trim(),
      employmentType: raw.employmentType,
      employmentStartDate: raw.startDate,
      employmentEndDate: raw.endDate,
      departmentAssignment: {
        primaryDepartmentId: raw.departmentId,
        secondaryDepartmentIds: []
      },
      profileDocuments: documents
    });
  }

  readonly close = () => this.dialogRef.close();

  private loadDepartments(): void {
    this.departmentRequester.list({ pageSize: 250 }).subscribe({
      next: (response) => {
        this.departments.splice(0, this.departments.length, ...(response.data ?? []));
      },
      error: () => {
        // swallow errors; departments are optional for UX
      }
    });
  }

  private patchData(data: Partial<EmployeeFormValue>): void {
    this.form.patchValue({
      firstName: data.firstName ?? '',
      lastName: data.lastName ?? '',
      email: data.email ?? '',
      phoneNumber: data.phoneNumber ?? '',
      jobTitle: data.jobTitle ?? '',
      employmentType: data.employmentType ?? 'FullTime',
      startDate: data.employmentStartDate ?? '',
      endDate: data.employmentEndDate ?? ''
    });

    if (data.departmentAssignment?.primaryDepartmentId) {
      this.form.controls.departmentId.setValue(data.departmentAssignment.primaryDepartmentId);
    }

    this.profileDocumentControls.clear();
    (data.profileDocuments ?? []).forEach((document) => this.addDocument(document));
  }

  private buildDocumentGroup(document?: EmployeeProfileDocumentInput): FormGroup {
    return this.fb.group({
      fileName: [document?.fileName ?? '', Validators.required],
      storagePath: [document?.storagePath ?? '', Validators.required],
      description: [document?.description ?? ''],
      contentType: [document?.contentType ?? ''],
      uploadedAtUtc: [document?.uploadedAtUtc ?? '']
    });
  }
}
