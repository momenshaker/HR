import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResponse } from '@app/core/data-access';

export interface HeadcountItem {
  readonly departmentId: string;
  readonly departmentName: string;
  readonly count: number;
}

export interface PayrollRunTotals {
  readonly runId: string;
  readonly periodStart: string;
  readonly periodEnd: string;
  readonly totalGross: number;
  readonly totalNet: number;
}

export interface DepartmentPayrollTotals {
  readonly departmentId: string;
  readonly departmentName: string;
  readonly totalGross: number;
  readonly totalNet: number;
}

export interface PayrollTotalsResponse {
  readonly runs: readonly PayrollRunTotals[];
  readonly byDepartment: readonly DepartmentPayrollTotals[];
}

export interface VacancySummary {
  readonly status: string;
  readonly numberOfPositions: number;
}

export interface TrainingCompliance {
  readonly organizationId: string;
  readonly mandatoryCourseCount: number;
  readonly observedEmployeeCount: number;
  readonly compliantEmployeeCount: number;
  readonly complianceRate: number;
}

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly analyticsUrl = `${this.config.apiBaseUrl}/analytics`;
  private readonly vacanciesUrl = `${this.config.apiBaseUrl}/vacancies`;

  getHeadcount(organizationId: string): Observable<PaginatedResponse<HeadcountItem>> {
    const params = new HttpParams().set('orgId', organizationId);
    return this.http.get<PaginatedResponse<HeadcountItem>>(`${this.analyticsUrl}/headcount`, { params });
  }

  getPayrollTotals(organizationId: string, from: string, to: string): Observable<PaginatedResponse<PayrollTotalsResponse>> {
    let params = new HttpParams().set('orgId', organizationId);
    params = params.set('from', from);
    params = params.set('to', to);
    return this.http.get<PaginatedResponse<PayrollTotalsResponse>>(`${this.analyticsUrl}/payroll-totals`, { params });
  }

  getVacancies(): Observable<PaginatedResponse<VacancySummary>> {
    return this.http.get<PaginatedResponse<VacancySummary>>(this.vacanciesUrl);
  }

  getTrainingCompliance(organizationId: string): Observable<PaginatedResponse<TrainingCompliance>> {
    const params = new HttpParams().set('orgId', organizationId);
    return this.http.get<PaginatedResponse<TrainingCompliance>>(`${this.analyticsUrl}/training-compliance`, { params });
  }
}
