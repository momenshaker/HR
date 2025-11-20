import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';
import {
  AttendanceRecord,
  ClockInPayload,
  ClockOutPayload,
  DelegatedAuthority,
  EmployeeOrganizationSnapshot,
  LeaveRequest,
  LeaveRequestPayload,
  SalarySlip,
  SelfServiceAccount,
  SelfServiceAccountPayload,
  SelfServiceAccountUpdatePayload,
  TrainingCourse
} from './self-service.models';
import { PaginatedResponse } from '@app/core/data-access';

@Injectable({ providedIn: 'root' })
export class SelfServiceApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);

  private base(employeeId: string): string {
    return `${this.config.apiBaseUrl}/EmployeeSelfService/${employeeId}`;
  }

  getLeaveRequests(employeeId: string) {
    return this.http.get<PaginatedResponse<LeaveRequest>>(`${this.base(employeeId)}/leave-requests`);
  }

  submitLeaveRequest(employeeId: string, payload: LeaveRequestPayload) {
    return this.http.post<LeaveRequest>(`${this.base(employeeId)}/leave-requests`, payload);
  }

  clockIn(employeeId: string, payload: ClockInPayload) {
    return this.http.post<AttendanceRecord>(`${this.base(employeeId)}/attendance/clock-in`, payload);
  }

  clockOut(employeeId: string, attendanceRecordId: string, payload: ClockOutPayload) {
    return this.http.post<AttendanceRecord>(
      `${this.base(employeeId)}/attendance/${attendanceRecordId}/clock-out`,
      payload
    );
  }

  getSalarySlips(employeeId: string) {
    return this.http.get<SalarySlip[]>(`${this.base(employeeId)}/salary-slips`);
  }

  getTrainingCourses(employeeId: string) {
    return this.http.get<TrainingCourse[]>(`${this.base(employeeId)}/training-courses`);
  }

  getOrganizationSnapshot(employeeId: string) {
    return this.http.get<EmployeeOrganizationSnapshot>(`${this.base(employeeId)}/organization`);
  }

  getDelegatedAuthorities(employeeId: string) {
    return this.http.get<DelegatedAuthority[]>(`${this.base(employeeId)}/delegated-authorities`);
  }

  getAccount(employeeId: string) {
    return this.http.get<SelfServiceAccount>(`${this.base(employeeId)}/account`);
  }

  getAttendanceRecords() {
    return this.http.get<PaginatedResponse<AttendanceRecord>>(`${this.config.apiBaseUrl}/attendanceRecords`);
  }

  getAttendanceHistory(employeeId: string) {
    return this.http.get<PaginatedResponse<AttendanceRecord>>(`${this.base(employeeId)}/attendance-records`);
  }

  createAccount(employeeId: string, payload: SelfServiceAccountPayload) {
    return this.http.post<SelfServiceAccount>(`${this.base(employeeId)}/account`, payload);
  }

  updateAccount(employeeId: string, payload: SelfServiceAccountUpdatePayload) {
    return this.http.put<SelfServiceAccount>(`${this.base(employeeId)}/account`, payload);
  }

  deleteAccount(employeeId: string) {
    return this.http.delete<void>(`${this.base(employeeId)}/account`);
  }
}
