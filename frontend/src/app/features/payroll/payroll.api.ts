import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

export type PayrollStatus = 'Draft' | 'Calculated' | 'UnderReview' | 'Approved' | 'Locked' | 'Paid';

export interface PayrollRun {
  readonly id: string;
  readonly organizationId: string;
  readonly periodStart: string;
  readonly periodEnd: string;
  readonly payDate: string;
  readonly status: PayrollStatus;
  readonly createdAtUtc: string;
  readonly approvedAtUtc?: string | null;
  readonly paidAtUtc?: string | null;
  readonly totalGrossPay: number;
  readonly totalNetPay: number;
  readonly notes: string;
}

export type PayrollCalculationType = 'FixedAmount' | 'PercentageOfBasic' | 'Formula';
export type PayrollComponentType = 'Earning' | 'Deduction';

export interface PayrollComponentAmount {
  readonly componentId: string;
  readonly name: string;
  readonly type: PayrollComponentType;
  readonly calculationType: PayrollCalculationType;
  readonly amount: number;
  readonly isTaxable: boolean;
  readonly isRecurring: boolean;
  readonly formula?: string | null;
}

export interface PayrollBreakdown {
  readonly earnings: readonly PayrollComponentAmount[];
  readonly deductions: readonly PayrollComponentAmount[];
}

export interface PayrollItem {
  readonly id: string;
  readonly runId: string;
  readonly employeeId: string;
  readonly gross: number;
  readonly deductions: number;
  readonly net: number;
  readonly currency: string;
  readonly earnings?: readonly PayrollComponentAmount[];
  readonly deductionComponents?: readonly PayrollComponentAmount[];
  readonly breakdownJson?: string | null;
}

export interface Payslip {
  readonly id: string;
  readonly employeeId: string;
  readonly periodId: string;
  readonly netPay: number;
  readonly pdfUrl?: string | null;
  readonly publishedToEmployee: boolean;
}

export interface CreatePayrollRunPayload {
  readonly organizationId: string;
  readonly periodStart: string;
  readonly periodEnd: string;
  readonly payDate: string;
  readonly notes?: string;
}

@Injectable({ providedIn: 'root' })
export class PayrollApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/payroll/runs`;

  listRuns(filters: { organizationId?: string; status?: PayrollStatus | '' } = {}): Observable<readonly PayrollRun[]> {
    let params = new HttpParams();
    if (filters.organizationId) {
      params = params.set('orgId', filters.organizationId);
    }
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    return this.http.get<readonly PayrollRun[]>(this.baseUrl, { params });
  }

  createRun(payload: CreatePayrollRunPayload): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(this.baseUrl, payload);
  }

  calculate(id: string): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(`${this.baseUrl}/${id}:calculate`, {});
  }

  moveToReview(id: string): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(`${this.baseUrl}/${id}:review`, {});
  }

  approve(id: string): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(`${this.baseUrl}/${id}:approve`, {});
  }

  lock(id: string): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(`${this.baseUrl}/${id}:lock`, {});
  }

  markPaid(id: string): Observable<PayrollRun> {
    return this.http.post<PayrollRun>(`${this.baseUrl}/${id}:paid`, {});
  }

  listItems(id: string): Observable<readonly PayrollItem[]> {
    return this.http.get<readonly PayrollItem[]>(`${this.baseUrl}/${id}/items`);
  }

  generatePayslips(id: string): Observable<readonly Payslip[]> {
    return this.http.post<readonly Payslip[]>(`${this.baseUrl}/${id}:payslips`, {});
  }
}
