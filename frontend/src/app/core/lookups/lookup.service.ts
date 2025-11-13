import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';
import { LookupCollection, LookupValue, LookupValuePayload } from './lookup.types';

@Injectable({ providedIn: 'root' })
export class LookupApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly resourceUrl = `${this.config.apiBaseUrl}/api/v1/lookups`;

  list(etag?: string | null): Observable<HttpResponse<LookupCollection>> {
    let headers = new HttpHeaders();
    if (etag) {
      headers = headers.set('If-None-Match', etag);
    }
    return this.http.get<LookupCollection>(this.resourceUrl, { observe: 'response', headers });
  }

  create(payload: LookupValuePayload): Observable<LookupValue> {
    return this.http.post<LookupValue>(this.resourceUrl, payload);
  }

  update(id: string, payload: LookupValuePayload): Observable<LookupValue> {
    return this.http.put<LookupValue>(`${this.resourceUrl}/${id}`, payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.resourceUrl}/${id}`);
  }
}
