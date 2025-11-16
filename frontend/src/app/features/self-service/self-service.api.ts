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
  LeaveType,
  SalarySlip,
  SelfServiceAccount,
  SelfServiceAccountPayload,
  SelfServiceAccountUpdatePayload,
  TrainingCourse
} from './self-service.models';

@Injectable({ providedIn: 'root' })
export class SelfServiceApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);

  private base(employeeId: string): string {
    return `${this.config.apiBaseUrl}/employees/${employeeId}/self-service`;
  }

  getLeaveRequests(employeeId: string) {
    return this.http.get<LeaveRequest[]>(`${this.base(employeeId)}/leave-requests`);
  }

  submitLeaveRequest(employeeId: string, payload: LeaveRequestPayload) {
    return this.http.post<LeaveRequest>(`${this.base(employeeId)}/leave-requests`, payload);
  }

  getLeaveTypes() {
    return this.http.get<LeaveType[]>(`${this.config.apiBaseUrl}/leave/types`);
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
