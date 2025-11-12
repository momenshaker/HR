import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AsyncPipe, NgIf, NgFor } from '@angular/common';
import { NavigationService } from './core/layout/navigation.service';
import { AuthStore } from './core/auth/auth.store';
import { ThemeService } from './core/services/theme.service';
import { BreadcrumbsComponent } from './shared/components/breadcrumbs/breadcrumbs.component';
import { AuthService } from './core/auth/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatBadgeModule,
    MatTooltipModule,
    MatSnackBarModule,
    AsyncPipe,
    NgIf,
    NgFor,
    BreadcrumbsComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly navigationService = inject(NavigationService);
  private readonly authStore = inject(AuthStore);
  private readonly themeService = inject(ThemeService);
  private readonly authService = inject(AuthService);

  readonly navigationItems = this.navigationService.navigationItems;
  readonly breadcrumbs = this.navigationService.breadcrumbs;
  readonly user = this.authStore.user;
  readonly theme = this.themeService.theme;

  private readonly showShellSignal = signal(true);
  readonly showShell = this.showShellSignal.asReadonly();

  ngOnInit(): void {
    this.authService.initialize();
    this.updateShellVisibility(this.router.url);
    this.router.events.subscribe(() => this.updateShellVisibility(this.router.url));
  }

  toggleTheme(): void {
    this.themeService.toggle();
  }

  logout(): void {
    this.router.navigate(['/auth/logout']);
  }

  private updateShellVisibility(url: string): void {
    this.showShellSignal.set(!url.startsWith('/auth'));
  }
}
