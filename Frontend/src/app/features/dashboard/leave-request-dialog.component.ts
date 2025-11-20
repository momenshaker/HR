import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { LeaveApiService, LeaveType } from '../leave/leave.api';

@Component({
  selector: 'app-leave-request-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>Submit leave request</h2>
    <form [formGroup]="form" (ngSubmit)="submit()" class="leave-dialog">
      <mat-form-field appearance="outline">
        <mat-label>Leave type</mat-label>
        <mat-select formControlName="typeId">
          <mat-option *ngFor="let type of leaveTypes()" [value]="type.id">
            {{ type.name }}
          </mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Start date</mat-label>
        <input matInput [matDatepicker]="startPicker" formControlName="startDate" />
        <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
        <mat-datepicker #startPicker></mat-datepicker>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>End date</mat-label>
        <input matInput [matDatepicker]="endPicker" formControlName="endDate" />
        <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
        <mat-datepicker #endPicker></mat-datepicker>
      </mat-form-field>
      <mat-form-field appearance="outline" class="leave-dialog__reason">
        <mat-label>Reason</mat-label>
        <textarea matInput rows="3" formControlName="reason"></textarea>
      </mat-form-field>
      <div class="leave-dialog__actions">
        <button mat-stroked-button type="button" (click)="close()">Cancel</button>
        <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid">Send</button>
      </div>
    </form>
  `,
  styles: [
    `
      .leave-dialog {
      padding:20px;
        display: grid;
        gap: 1rem;
      }

      .leave-dialog__reason {
        grid-column: 1 / -1;
      }

      .leave-dialog__actions {
        display: flex;
        justify-content: flex-end;
        gap: 0.5rem;
      }
    `
  ]
})
export class LeaveRequestDialogComponent implements OnInit {
  private readonly dialogRef = inject(MatDialogRef<LeaveRequestDialogComponent>);
  private readonly fb = inject(FormBuilder);
  private readonly leaveApi = inject(LeaveApiService);

  readonly leaveTypes = signal<readonly LeaveType[]>([]);

  readonly form = this.fb.nonNullable.group({
    typeId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    reason: ['', Validators.maxLength(500)]
  });

  submit(): void {
    if (this.form.invalid) {
      return;
    }
    this.dialogRef.close(this.form.getRawValue());
  }

  close(): void {
    this.dialogRef.close(null);
  }

  ngOnInit(): void {
    this.leaveApi.getTypes().subscribe({
      next: (types) => this.leaveTypes.set(types),
      error: () => {
        // if the request fails we keep the dialog usable with an empty list
      }
    });
  }
}
