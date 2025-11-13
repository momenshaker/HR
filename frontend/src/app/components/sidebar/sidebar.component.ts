import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

export interface SidebarMenuItem {
  path?: string;
  title: string;
  icon: string;
  class?: string;
  children?: SidebarMenuItem[];
}

export const SIDEBAR_ITEMS: SidebarMenuItem[] = [
  { path: '/dashboard', title: 'Dashboard', icon: 'dashboard', class: '' },
  { path: '/organizations', title: 'Organizations', icon: 'domain', class: '' },
  { path: '/departments', title: 'Departments', icon: 'account_tree', class: '' },
  { path: '/employees', title: 'Employees', icon: 'people', class: '' },
  { path: '/attendance', title: 'Attendance', icon: 'schedule', class: '' },
  { path: '/leave', title: 'Leave', icon: 'beach_access', class: '' },
  { path: '/self-service', title: 'Self Service', icon: 'person', class: '' },
  { path: '/payroll', title: 'Payroll', icon: 'paid', class: '' },
  { path: '/recruitment', title: 'Recruitment', icon: 'work', class: '' },
  { path: '/notifications', title: 'Notifications', icon: 'notifications', class: '' }
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, MatButtonModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit, OnDestroy {
  menuItems: SidebarMenuItem[] = [];
  private expandedGroups = new Set<string>();
  private routerSubscription?: Subscription;

  constructor(private router: Router) { }

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
