import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-date-range-picker',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDatepickerModule, MatNativeDateModule, MatFormFieldModule, MatInputModule],
  template: `
    <form [formGroup]="form" class="date-range">
      <mat-form-field appearance="outline">
        <mat-label>Start date</mat-label>
        <input matInput [matDatepicker]="startPicker" formControlName="start" />
        <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
        <mat-datepicker #startPicker></mat-datepicker>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>End date</mat-label>
        <input matInput [matDatepicker]="endPicker" formControlName="end" />
        <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
        <mat-datepicker #endPicker></mat-datepicker>
      </mat-form-field>
    </form>
  `,
  styles: [
    `
      .date-range {
        display: flex;
        gap: 1rem;
        flex-wrap: wrap;
      }
      mat-form-field {
        flex: 1;
        min-width: 200px;
      }
    `
  ]
})
export class DateRangePickerComponent {
  @Input({ required: true }) form!: FormGroup;
  @Output() readonly changed = new EventEmitter<void>();
}
