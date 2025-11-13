import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmationDialogData {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <div class="confirmation-card">
      <header class="confirmation-card__header">
        <div>
          <h2>{{ data.title }}</h2>
        </div>
      </header>
      <section class="confirmation-card__body">
        <p>{{ data.message }}</p>
      </section>
      <footer class="confirmation-card__actions">
        <button mat-button class="btn btn-warning" mat-dialog-close>
          {{ data.cancelLabel ?? 'Cancel' }}
        </button>
        <button mat-flat-button class="btn btn-primary" (click)="onConfirm()">
          {{ data.confirmLabel ?? 'Confirm' }}
        </button>
      </footer>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [
    `
      .confirmation-card {
        display: flex;
        flex-direction: column;
        width: min(520px, 100%);
        background: #fff;
        border-radius: 12px;
        padding: 0;
        box-shadow: 0 16px 48px rgba(0, 0, 0, 0.15);
      }

      .confirmation-card__header {
        padding: 1.25rem 1.5rem;
        border-bottom: 1px solid #eaeaea;
      }

      .confirmation-card__body {
        padding: 1.25rem 1.5rem;
        font-size: 0.95rem;
        color: rgba(0, 0, 0, 0.8);
      }

      .confirmation-card__actions {
        display: flex;
        justify-content: flex-end;
        gap: 0.75rem;
        padding: 1rem 1.25rem 1.25rem;
      }

      .confirmation-card__actions button {
        min-width: 110px;
      }
    `
  ]
})
export class ConfirmationDialogComponent {
  constructor(
    private readonly dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) readonly data: ConfirmationDialogData
  ) {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
