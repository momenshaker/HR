import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { take } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AuthStore } from './auth.store';
import { UserRole } from './auth.models';
import { AuthUser } from './auth.models';
import { PaginatedResponse } from '../data-access';

interface EmployeeDepartmentDto {
  organizationId: string;
}

export interface ScopedOrganization {
  id: string;
  name: string;
  code: string;
}

@Injectable({ providedIn: 'root' })
export class OrganizationContextService {
  private static readonly storageKey = 'hr:organizationId';
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly authStore = inject(AuthStore);
  private readonly storage = window.localStorage;
  private readonly restrictedRoles: ReadonlyArray<UserRole> = ['HR', 'Manager', 'Employee'];
  private readonly organizationDetails = signal<ScopedOrganization | null>(null);
  private lastRequestedEmployeeId: string | null = null;

  readonly organizationId = signal<string | null>(this.loadStoredValue());
  readonly organization = computed(() => this.organizationDetails());
  readonly isScopedRole = computed(() => this.authStore.roles().some((role) => this.restrictedRoles.includes(role)));
  readonly shouldUseScopedOrganization = computed(
    () => this.isScopedRole() && Boolean(this.organizationId())
  );

  constructor() {
    const storedOrgId = this.organizationId();
    if (storedOrgId) {
      this.loadOrganizationDetails(storedOrgId);
    }
  }

  updateFromUser(user: AuthUser | null): void {
    const hasScopedRole = user?.roles.some((role) => this.restrictedRoles.includes(role)) ?? false;
    const employeeId = user?.employeeId ?? null;

    if (!hasScopedRole || !employeeId) {
      this.lastRequestedEmployeeId = null;
      this.setOrganizationId(null);
      return;
    }

    if (this.lastRequestedEmployeeId === employeeId) {
      return;
    }

    this.lastRequestedEmployeeId = employeeId;
    this.fetchOrganizationId(employeeId);
  }

  private fetchOrganizationId(employeeId: string): void {
    this.http
      .get<PaginatedResponse<EmployeeDepartmentDto>>(`${this.config.apiBaseUrl}/employees/${employeeId}/departments`)
      .pipe(take(1))
      .subscribe({
        next: (departments) => {
          const organizationId =
            departments.items.map((department) => department.organizationId).find(Boolean) ?? null;
          this.setOrganizationId(organizationId);
        },
        error: () => this.setOrganizationId(null)
      });
  }

  private loadOrganizationDetails(organizationId: string): void {
    this.http
      .get<ScopedOrganization>(`${this.config.apiBaseUrl}/organizations/${organizationId}`)
      .pipe(take(1))
      .subscribe({
        next: (organization) => this.organizationDetails.set(organization),
        error: () => this.organizationDetails.set(null)
      });
  }

  private setOrganizationId(value: string | null): void {
    if (value) {
      this.storage.setItem(OrganizationContextService.storageKey, value);
      this.organizationId.set(value);
      this.loadOrganizationDetails(value);
      return;
    }

    this.storage.removeItem(OrganizationContextService.storageKey);
    this.organizationId.set(null);
    this.organizationDetails.set(null);
  }

  private loadStoredValue(): string | null {
    return this.storage.getItem(OrganizationContextService.storageKey);
  }
}
