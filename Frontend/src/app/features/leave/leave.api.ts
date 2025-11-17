import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';

export interface LeaveType {
  readonly id: string;
  readonly code: string;
  readonly name: string;
  readonly isPaid: boolean;
  readonly requiresApproval: boolean;
  readonly requiresAttachment: boolean;
  readonly annualAllowanceDays: number;
  readonly carryOverDays: number;
  readonly maxConsecutiveDays?: number | null;
  readonly color: string;
}

export interface LeaveBalance {
  readonly employeeId: string;
  readonly leaveTypeId: string;
  readonly year: number;
  readonly openingBalance: number;
  readonly accrued: number;
  readonly taken: number;
  readonly carriedForward: number;
  readonly reserved: number;
  readonly remaining: number;
}

export interface LeaveRequest {
  readonly id: string;
  readonly employeeId: string;
  readonly leaveTypeId: string;
  readonly leaveType: string;
  readonly startDate: string;
  readonly endDate: string;
  readonly numberOfDays: number;
  readonly status: string;
  readonly approverId?: string | null;
  readonly reason: string;
  readonly attachmentPath?: string | null;
  readonly submittedAtUtc: string;
  readonly approvedAtUtc?: string | null;
  readonly rejectedAtUtc?: string | null;
  readonly cancelledAtUtc?: string | null;
}

export interface LeaveRequestFilters {
  employeeId?: string | null;
  managerId?: string | null;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface LeaveRequestPayload {
  readonly employeeId: string;
  readonly leaveTypeId: string;
  readonly startDate: string;
  readonly endDate: string;
  readonly reason?: string;
  readonly attachmentPath?: string | null;
  readonly draft?: boolean;
}

export interface RejectPayload {
  readonly reason: string;
}

export interface LeaveBalanceAdjustmentPayload {
  readonly leaveTypeId: string;
  readonly remaining: number;
}

export interface SetLeaveBalancesPayload {
  readonly employeeId: string;
  readonly year: number;
  readonly balances: readonly LeaveBalanceAdjustmentPayload[];
}

@Injectable({ providedIn: 'root' })
export class LeaveApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/leave`;

  getTypes(): Observable<readonly LeaveType[]> {
    return this.http
      .get<PaginatedResponse<LeaveType>>(`${this.baseUrl}/types`)
      .pipe(map((response) => response.items));
  }

  getBalances(employeeId: string, year: number): Observable<readonly LeaveBalance[]> {
    const params = new HttpParams().set('employeeId', employeeId).set('year', String(year));
    return this.http
      .get<PaginatedResponse<LeaveBalance>>(`${this.baseUrl}/balances`, { params })
      .pipe(map((response) => response.items));
  }

  listRequests(filters: LeaveRequestFilters = {}): Observable<PaginatedResponse<LeaveRequest>> {
    let params = new HttpParams();
    if (filters.employeeId) {
      params = params.set('employeeId', filters.employeeId);
    }
    if (filters.managerId) {
      params = params.set('managerId', filters.managerId);
    }
    if (filters.status) {
      params = params.set('status', filters.status);
    }
    if (filters.page) {
      params = params.set('page', String(filters.page));
    }
    if (filters.pageSize) {
      params = params.set('pageSize', String(filters.pageSize));
    }
    return this.http.get<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests`, { params });
  }

  createRequest(payload: LeaveRequestPayload): Observable<LeaveRequest> {
    return this.http
      .post<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests`, payload)
      .pipe(map((response) => this.extractSingle(response)));
  }

  getRequestById(id: string): Observable<LeaveRequest> {
    return this.http
      .get<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests/${id}`)
      .pipe(map((response) => this.extractSingle(response)));
  }

  submitRequest(id: string, employeeId: string): Observable<LeaveRequest> {
    const params = new HttpParams().set('employeeId', employeeId);
    return this.http
      .post<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests/${id}:submit`, null, { params })
      .pipe(map((response) => this.extractSingle(response)));
  }

  approveRequest(id: string, managerId: string): Observable<LeaveRequest> {
    const params = new HttpParams().set('managerId', managerId);
    return this.http
      .post<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests/${id}:approve`, null, { params })
      .pipe(map((response) => this.extractSingle(response)));
  }

  rejectRequest(id: string, managerId: string, payload: RejectPayload): Observable<LeaveRequest> {
    const params = new HttpParams().set('managerId', managerId);
    return this.http
      .post<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests/${id}:reject`, payload.reason, { params })
      .pipe(map((response) => this.extractSingle(response)));
  }

  cancelRequest(id: string, employeeId: string): Observable<LeaveRequest> {
    const params = new HttpParams().set('employeeId', employeeId);
    return this.http
      .post<PaginatedResponse<LeaveRequest>>(`${this.baseUrl}/requests/${id}:cancel`, null, { params })
      .pipe(map((response) => this.extractSingle(response)));
  }

  setBalances(payload: SetLeaveBalancesPayload): Observable<readonly LeaveBalance[]> {
    return this.http
      .post<PaginatedResponse<LeaveBalance>>(`${this.baseUrl}/balances`, payload)
      .pipe(map((response) => response.items));
  }

  private extractSingle<T>(response: PaginatedResponse<T>): T {
    if (response.items.length === 0) {
      throw new Error('Leave API returned an empty response.');
    }

    const item = response.items[0];
    if (item === undefined) {
      throw new Error('Leave API returned an empty response.');
    }

    return item;
  }
}
