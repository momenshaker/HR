import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';
import { Plan, PlanEntitlement } from './subscriptions.types';

export interface PlanPayload {
  readonly code: string;
  readonly name: string;
  readonly description: string;
  readonly price: number;
  readonly billingInterval: string;
  readonly entitlements: readonly PlanEntitlement[];
}

@Injectable({ providedIn: 'root' })
export class PlanApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/plans`;

  list(): Observable<readonly Plan[]> {
    return this.http
      .get<PaginatedResponse<Plan>>(this.baseUrl)
      .pipe(map((response) => response.items));
  }

  create(payload: PlanPayload): Observable<Plan> {
    return this.http.post<Plan>(this.baseUrl, payload);
  }

  update(id: string, payload: PlanPayload): Observable<Plan> {
    return this.http.put<Plan>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
