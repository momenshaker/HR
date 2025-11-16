import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormActionsComponent } from '@shared/components/form-actions/form-actions.component';
import { LookupStore } from '@core/lookups/lookup.store';

export interface OrganizationFormValue {
  name: string;
  code: string;
  description: string;
  industry: string;
  region: string;
  headquartersAddress: string;
  timeZone: string;
  primaryContactEmail: string;
  websiteUrl: string;
  isActive: boolean;
}

@Component({
  selector: 'app-organization-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatCheckboxModule, MatInputModule, MatSelectModule, FormActionsComponent],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit organization' : 'Create organization' }}</h2>
     <form [formGroup]="form" (ngSubmit)="submit()" class="example-full-width">
      <mat-dialog-content>
           <div class="row">
        <div class="col-md-6 item-full-width"> <mat-form-field appearance="outline" class="example-full-width">
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
</div>     <div class="row">
        <div class="col-md-6">
        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description"></textarea>
        </mat-form-field>
        </div>
        <div class="col-md-6">
        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Industry</mat-label>
          <mat-select formControlName="industry">
            <mat-option *ngFor="let option of industries()" [value]="option">
              {{ option }}
            </mat-option>
          </mat-select>
        </mat-form-field>
</div></div>
     <div class="row">
        <div class="col-md-6">
        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Region</mat-label>
          <mat-select formControlName="region">
            <mat-option *ngFor="let option of regions()" [value]="option">
              {{ option }}
            </mat-option>
          </mat-select>
        </mat-form-field>
        </div>
        <div class="col-md-6">
         <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Website</mat-label>
          <input matInput formControlName="websiteUrl" type="url" />
        </mat-form-field>
      
</div></div>
     <div class="row">
        <div class="col-md-6">
        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Time zone</mat-label>
          <mat-select formControlName="timeZone">
            <mat-option *ngFor="let option of timeZones()" [value]="option">
              {{ option }}
            </mat-option>
          </mat-select>
        </mat-form-field>
</div>        <div class="col-md-6">
        <mat-form-field appearance="outline" class="example-full-width">
          <mat-label>Primary contact email</mat-label>
          <input matInput formControlName="primaryContactEmail" type="email" />
          <mat-error *ngIf="form.controls.primaryContactEmail.hasError('email')">Enter a valid email</mat-error>
        </mat-form-field>
</div></div>
     <div class="row">
        <div class="col-md-12">
         <mat-form-field appearance="outline" class="example-full-width">
        
          <mat-label>Headquarters address</mat-label>
          <textarea matInput formControlName="headquartersAddress"></textarea>
        </mat-form-field>
</div></div>
        <mat-checkbox formControlName="isActive">Active organization</mat-checkbox>
        <app-form-actions submitLabel="Save" [onCancel]="close"></app-form-actions>
    </mat-dialog-content>  </form>
   
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [
    `
      .example-full-width {
        width: 100%;
      }

      .item-full-width mat-form-field {
        width: 100%;
      }
    `
  ]
})
export class OrganizationFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<OrganizationFormComponent>);
  private readonly lookupStore = inject(LookupStore);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    code: ['', Validators.required],
    description: [''],
    industry: [''],
    region: [''],
    headquartersAddress: [''],
    timeZone: [''],
    primaryContactEmail: ['', Validators.email],
    websiteUrl: [''],
    isActive: [true]
  });

  readonly industries = this.lookupStore.industries;
  readonly regions = this.lookupStore.regions;
  readonly timeZones = this.lookupStore.timeZones;

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
