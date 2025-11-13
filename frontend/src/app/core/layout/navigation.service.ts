import { computed, inject, Injectable, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AuthStore } from '../auth/auth.store';
import { NavigationItem } from './navigation.model';

@Injectable({ providedIn: 'root' })
export class NavigationService {
  private readonly router = inject(Router);
  private readonly authStore = inject(AuthStore);

  private readonly allItems: NavigationItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard', roles: ['Admin', 'HR', 'Manager', 'Employee'] },
    { label: 'Organizations', icon: 'domain', route: '/organizations', roles: ['Admin', 'HR'] },
    { label: 'Departments', icon: 'account_tree', route: '/departments', roles: ['Admin', 'HR', 'Manager'] },
    { label: 'Employees', icon: 'people', route: '/employees', roles: ['Admin', 'HR', 'Manager'] },
    { label: 'Attendance', icon: 'schedule', route: '/attendance', roles: ['Admin', 'HR', 'Manager'] },
    { label: 'Leave', icon: 'beach_access', route: '/leave', roles: ['Admin', 'HR', 'Manager', 'Employee'] },
    { label: 'Self Service', icon: 'person', route: '/self-service', roles: ['Admin', 'HR', 'Manager', 'Employee'] },
    { label: 'Payroll', icon: 'paid', route: '/payroll', roles: ['Admin', 'HR'] },
    { label: 'Recruitment', icon: 'work', route: '/recruitment', roles: ['Admin', 'HR'] },
    { label: 'Notifications', icon: 'notifications', route: '/notifications', roles: ['Admin', 'HR', 'Manager'] }
  ];

  readonly navigationItems = computed(() => this.filterByRole(this.allItems));

  private readonly breadcrumbsState = signal<string[]>(['Dashboard']);
  readonly breadcrumbs = computed(() => this.breadcrumbsState());

  constructor() {
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd)).subscribe((event) => {
      const [path] = event.urlAfterRedirects.split('?');
      this.updateBreadcrumbs(path ?? '/');
    });
  }

  private filterByRole(items: NavigationItem[]): NavigationItem[] {
    const roles = this.authStore.roles();
    if (roles.length === 0) {
      return items;
    }

    const normalizedRoles = roles.map((role) => role.toLowerCase());
    return items
      .filter(
        (item) =>
          !item.roles ||
          item.roles.some((role) => normalizedRoles.includes(role.toLowerCase()))
      )
      .map((item) => ({
        ...item,
        children: item.children ? this.filterByRole(item.children) : undefined
      }));
  }

  private updateBreadcrumbs(url: string): void {
    const segments = url
      .split('/')
      .filter(Boolean)
      .map((segment) => segment.replace(/-/g, ' '))
      .map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1));
    const filtered = segments.filter((segment, index) => !(index === 0 && segment.toLowerCase() === 'dashboard'));
    this.breadcrumbsState.set(['Dashboard', ...filtered]);
  }
}
