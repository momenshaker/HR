import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '@core/config/app-config.model';
import { APP_CONFIG } from '@core/config/app-config.token';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';
import {
  CreateSubscriptionPayload,
  Invoice,
  Subscription,
  UpdateSubscriptionPayload
} from './subscriptions.types';

@Injectable({ providedIn: 'root' })
export class SubscriptionsApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/subscriptions`;

  list(
    page = 0,
    pageSize = 25,
    status?: string
  ): Observable<PaginatedResponse<Subscription>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<PaginatedResponse<Subscription>>(this.baseUrl, { params });
  }

  create(payload: CreateSubscriptionPayload): Observable<Subscription> {
    return this.http.post<Subscription>(this.baseUrl, payload);
  }

  update(id: string, payload: UpdateSubscriptionPayload): Observable<Subscription> {
    return this.http.put<Subscription>(`${this.baseUrl}/${id}`, payload);
  }

  cancel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getLatestInvoice(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.baseUrl}/${id}/invoice`);
  }

  assignOrganization(id: string, organizationIds: readonly string[]): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/organizations`, { organizationIds });
  }
}
