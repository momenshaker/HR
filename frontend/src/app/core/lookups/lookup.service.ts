import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';
import { PaginatedResponse } from '@core/data-access/paginated-response.model';
import { LookupCollection, LookupValue, LookupValuePayload } from './lookup.types';

@Injectable({ providedIn: 'root' })
export class LookupApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly resourceUrl = `${this.config.apiBaseUrl}/api/v1/lookups`;

  list(etag?: string | null): Observable<HttpResponse<PaginatedResponse<LookupCollection>>> {
    let headers = new HttpHeaders();
    if (etag) {
      headers = headers.set('If-None-Match', etag);
    }
    return this.http.get<PaginatedResponse<LookupCollection>>(this.resourceUrl, { observe: 'response', headers });
  }

  create(payload: LookupValuePayload): Observable<LookupValue> {
    return this.http
      .post<PaginatedResponse<LookupValue>>(this.resourceUrl, payload)
      .pipe(map((response) => this.extractSingle(response)));
  }

  update(id: string, payload: LookupValuePayload): Observable<LookupValue> {
    return this.http
      .put<PaginatedResponse<LookupValue>>(`${this.resourceUrl}/${id}`, payload)
      .pipe(map((response) => this.extractSingle(response)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.resourceUrl}/${id}`);
  }

  getByCategory(category: string): Observable<readonly LookupValue[]> {
    return this.http
      .get<PaginatedResponse<LookupValue>>(`${this.resourceUrl}/category/${category}`)
      .pipe(map((response) => response.items));
  }

  getById(id: string): Observable<LookupValue> {
    return this.http
      .get<PaginatedResponse<LookupValue>>(`${this.resourceUrl}/value/${id}`)
      .pipe(map((response) => this.extractSingle(response)));
  }

  private extractSingle<T>(response: PaginatedResponse<T>): T {
    if (response.items.length === 0) {
      throw new Error('Lookup API returned an empty payload.');
    }

    const item = response.items[0];
    if (item === undefined) {
      throw new Error('Lookup API returned an empty payload.');
    }

    return item;
  }
}
