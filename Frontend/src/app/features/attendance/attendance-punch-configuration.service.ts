import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

export interface PunchTypeOption {
  id: string;
  punchType: string;
  displayName: string;
  description: string;
  sortOrder: number;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class AttendancePunchConfigurationService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(APP_CONFIG).apiBaseUrl;

  getPunchTypes() {
    return this.http.get<readonly PunchTypeOption[]>(`${this.apiBaseUrl}/attendance-punch-configurations`);
  }

  savePunchType(payload: AttendancePunchConfigurationRequest) {
    const endpoint = payload.id
      ? `${this.apiBaseUrl}/attendance-punch-configurations/${payload.id}`
      : `${this.apiBaseUrl}/attendance-punch-configurations`;

    return this.http.request<PunchTypeOption>(payload.id ? 'PUT' : 'POST', endpoint, {
      body: payload
    });
  }
}

export interface AttendancePunchConfigurationRequest {
  id?: string;
  punchType: string;
  displayName: string;
  description?: string;
  sortOrder: number;
  isActive: boolean;
}
