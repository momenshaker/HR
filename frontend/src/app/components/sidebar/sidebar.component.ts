import { Component, HostBinding, Input, OnInit, OnDestroy, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AuthStore } from '@core/auth/auth.store';
import { UserRole } from '@core/auth/auth.models';

export interface SidebarMenuItem {
  path?: string;
  title: string;
  icon: string;
  class?: string;
  children?: SidebarMenuItem[];
  roles?: readonly UserRole[];
}

export const SIDEBAR_ITEMS: SidebarMenuItem[] = [
  {
    path: '/dashboard',
    title: 'Dashboard',
    icon: 'dashboard',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/organizations',
    title: 'Organizations',
    icon: 'domain',
    class: '',
    roles: ['Admin', 'HR']
  },
  {
    path: '/lookups',
    title: 'Lookups',
    icon: 'list_alt',
    class: '',
    roles: ['Admin', 'HR']
  },
  {
    path: '/departments',
    title: 'Departments',
    icon: 'schedule',
    class: '',
    roles: ['HR', 'Manager']
  },
  {
    path: '/employees',
    title: 'Employees',
    icon: 'people',
    class: '',
    roles: ['Admin', 'HR', 'Manager']
  },
  {
    path: '/attendance',
    title: 'Attendance',
    icon: 'schedule',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/leave',
    title: 'Leave',
    icon: 'beach_access',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/payroll',
    title: 'Payroll',
    icon: 'paid',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/subscriptions',
    title: 'Subscriptions',
    icon: 'subscriptions',
    class: '',
    roles: ['Admin', 'HR']
  },
  {
    path: '/plans',
    title: 'Plans',
    icon: 'card_membership',
    class: '',
    roles: ['Admin', 'HR']
  },
  {
    path: '/performance',
    title: 'Performance',
    icon: 'analytics',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/training',
    title: 'Training',
    icon: 'co_present',
    class: '',
    roles: ['Admin', 'HR', 'Manager', 'Employee']
  },
  {
    path: '/recruitment',
    title: 'Recruitment',
    icon: 'work',
    class: '',
    roles: ['Admin', 'HR', 'Manager']
  },
  {
    path: '/timesheets',
    title: 'Timesheets',
    icon: 'calendar_view_month',
    class: '',
    roles: ['Admin', 'HR', 'Manager']
  },
  {
    path: '/notifications',
    title: 'Notifications',
    icon: 'notifications',
    class: '',
    roles: ['Admin', 'HR', 'Manager']
  }
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit, OnDestroy {
  menuItems: SidebarMenuItem[] = [];
  private readonly authStore = inject(AuthStore);
  private readonly menuTracker = effect(() => {
    const roles = this.authStore.roles();
    this.menuItems = SIDEBAR_ITEMS.filter((item) => this.hasAccess(item, roles));
  });
  private expandedGroups = new Set<string>();
  private routerSubscription?: Subscription;
  @HostBinding('class.collapsed') collapsed = false;

  @Input()
  set compact(value: boolean) {
    this.collapsed = value;
  }

  constructor(private router: Router) { }

  private hasAccess(menuItem: SidebarMenuItem, roles: readonly UserRole[]): boolean {
    const allowed = menuItem.roles ?? ['Admin', 'HR', 'Manager', 'Employee'];
    return allowed.some((role) => roles.includes(role));
  }

  ngOnInit() {
    this.menuItems = SIDEBAR_ITEMS;
    this.routerSubscription = this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe(event => this.syncExpandedGroups(event.urlAfterRedirects));
  }

  ngOnDestroy() {
    this.routerSubscription?.unsubscribe();
  }

  toggleGroup(menuItem: SidebarMenuItem) {
    if (!menuItem.children?.length) {
      return;
    }
    const key = menuItem.title;
    if (this.expandedGroups.has(key)) {
      this.expandedGroups.delete(key);
    } else {
      this.expandedGroups.add(key);
    }
  }

  isGroupExpanded(menuItem: SidebarMenuItem) {
    return menuItem.children?.length ? this.expandedGroups.has(menuItem.title) : false;
  }

  isMobileMenu() {
    return (typeof window !== 'undefined') ? window.innerWidth <= 991 : false;
  }

  private syncExpandedGroups(url: string) {
    this.expandedGroups.clear();
    const normalizedUrl = url.split('?')[0] ?? '';
    for (const menuItem of this.menuItems) {
      if (
        menuItem.children?.some((child) => {
          const childPath = child.path;
          if (typeof childPath !== 'string') {
            return false;
          }
          return this.isUrlMatching(childPath!, normalizedUrl);
        })
      ) {
        this.expandedGroups.add(menuItem.title);
        break;
      }
    }
  }

  private isUrlMatching(menuPath: string | undefined, url: string) {
    if (!menuPath) {
      return false;
    }
    return url === menuPath || url.startsWith(`${menuPath}/`);
  }
}
