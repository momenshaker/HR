import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-logout-page',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule],
  template: `
    <div class="logout">
      <mat-spinner></mat-spinner>
      <p>Signing you out...</p>
    </div>
  `,
  styles: [
    `
      .logout {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 1rem;
        min-height: 300px;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LogoutPageComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/auth/login'])
    });
  }
}
