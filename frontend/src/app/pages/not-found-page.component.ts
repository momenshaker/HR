import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, RouterLink],
  template: `
    <section class="empty-state">
      <h1>404 – Page not found</h1>
      <p>The requested page could not be found.</p>
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
export class NotFoundPageComponent {}
