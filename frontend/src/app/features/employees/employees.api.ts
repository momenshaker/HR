import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';

export interface PositionSummaryDto {
  readonly id: string;
  readonly title: string;
  readonly jobCode: string;
  readonly organizationUnitId: string;
  readonly reportsToPositionId?: string | null;
  readonly occupiedByEmployeeId?: string | null;
  readonly grade: string;
  readonly employmentType: string;
  readonly effectiveFrom?: string | null;
  readonly effectiveTo?: string | null;
  readonly isCriticalRole: boolean;
  readonly isVacant: boolean;
}

export interface EmployeeSummaryDto {
  readonly id: string;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: string;
  readonly jobTitle: string;
  readonly phoneNumber: string;
  readonly employmentType: string;
  readonly primaryDepartmentId: string;
  readonly primaryDepartmentName: string;
  readonly departmentIds: readonly string[];
  readonly employmentStartDate: string;
  readonly employmentEndDate?: string | null;
  readonly dateOfBirth?: string | null;
}

export interface EmployeeHierarchyNode {
  readonly position: PositionSummaryDto;
  readonly employee?: EmployeeSummaryDto | null;
  readonly directReports: EmployeeHierarchyNode[];
}

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);

  getHierarchy(): Observable<EmployeeHierarchyNode[]> {
    return this.http.get<EmployeeHierarchyNode[]>(`${this.config.apiBaseUrl}/employees/hierarchy`);
  }
}
