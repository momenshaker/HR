import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from './core/auth/auth.service';
import { NavigationService } from './core/layout/navigation.service';
import { BreadcrumbsComponent } from './shared/components/breadcrumbs/breadcrumbs.component';
import { FooterComponent } from './components/footer/footer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    NavbarComponent,
    SidebarComponent,
    MatSnackBarModule,
    BreadcrumbsComponent,
    FooterComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  sidebarCompact = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    public navigationService: NavigationService
  ) {
    this.authService.initialize();
  }

  isLoginPage() {
    const path = this.router.url.split('?')[0];
    return path === '/auth/login' || path === '/auth/logout' || path === '/auth/onboarding';
  }

  toggleSidebarCompact() {
    this.sidebarCompact = !this.sidebarCompact;
  }
}
