import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';

export interface PositionSummaryDto {
  readonly id: string;
  readonly title: string;
  readonly jobCode: string;
  readonly organizationUnitId: string;
  readonly reportsToPositionId?: string | null;
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
  readonly positionId?: string | null;
  readonly reportsToEmployeeId?: string | null;
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

  getPositions(): Observable<PositionSummaryDto[]> {
    return this.http.get<PositionSummaryDto[]>(`${this.config.apiBaseUrl}/positions`);
  }

  getEmployees(pageSize = 200): Observable<EmployeeSummaryDto[]> {
    const params = new HttpParams().set('page', 1).set('pageSize', pageSize);
    return this.http
      .get<PaginatedResponse<EmployeeSummaryDto>>(`${this.config.apiBaseUrl}/employees`, { params })
      .pipe(map((response) => response.items));
  }
}
