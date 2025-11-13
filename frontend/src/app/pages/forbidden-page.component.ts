import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forbidden-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, RouterLink],
  template: `
    <section class="empty-state">
      <h1>403 – Access denied</h1>
      <p>You do not have permission to view this page.</p>
      <button mat-flat-button color="primary" routerLink="/dashboard">Go to dashboard</button>
    </section>
  `,
  styles: [
    `
      .empty-state {
        min-height: 60vh;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 1rem;
        text-align: center;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ForbiddenPageComponent {}
