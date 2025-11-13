import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-form-actions',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  template: `
    <div class="form-actions">
      <button mat-stroked-button class="btn btn-warning" type="button" (click)="onCancel?.()">Cancel</button>
      <button mat-flat-button class="btn btn-primary" type="submit" [disabled]="disabled">{{ submitLabel }}</button>
    </div>
  `,
  styles: [
    `
      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 1rem;
        margin-top: 1.5rem;
      }
    `
  ]
})
export class FormActionsComponent {
  @Input() submitLabel = 'Save';
  @Input() disabled = false;
  @Input() onCancel?: () => void;
}
