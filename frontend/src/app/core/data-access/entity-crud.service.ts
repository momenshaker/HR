import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';
import { PaginatedResponse } from './paginated-response.model';

export interface QueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sort?: string;
  direction?: 'asc' | 'desc';
  filters?: Record<string, string | number | boolean | undefined>;
}

class EntityCrudRequester<TCreate, TUpdate, TResponse> {
  constructor(
    private readonly http: HttpClient,
    private readonly config: AppConfig,
    private readonly resource: string
  ) {}

  list(query: QueryParams = {}): Observable<PaginatedResponse<TResponse>> {
    const params = this.toHttpParams(query);
    return this.http.get<PaginatedResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}`, { params });
  }

  getById(id: string): Observable<TResponse> {
    return this.http
      .get<PaginatedResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}/${id}`)
      .pipe(map((response) => this.extractSingle(response)));
  }

  create(payload: TCreate): Observable<TResponse> {
    return this.http
      .post<PaginatedResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}`, payload)
      .pipe(map((response) => this.extractSingle(response)));
  }

  update(id: string, payload: TUpdate): Observable<TResponse> {
    return this.http
      .put<PaginatedResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}/${id}`, payload)
      .pipe(map((response) => this.extractSingle(response)));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.config.apiBaseUrl}/${this.resource}/${id}`);
  }

  private toHttpParams(query: QueryParams): HttpParams {
    let params = new HttpParams();
    if (query.page !== undefined) {
      params = params.set('page', query.page);
    }
    if (query.pageSize !== undefined) {
      params = params.set('pageSize', query.pageSize);
    }
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.sort) {
      params = params.set('sort', query.sort);
    }
    if (query.direction) {
      params = params.set('direction', query.direction);
    }
    if (query.filters) {
      for (const [key, value] of Object.entries(query.filters)) {
        if (value !== undefined && value !== null) {
          params = params.set(key, String(value));
        }
      }
    }
    return params;
  }

  private extractSingle<T>(response: PaginatedResponse<T>): T {
    if (response.items.length === 0) {
      throw new Error(`The response for ${this.resource} did not include an item.`);
    }

    const item = response.items[0];
    if (item === undefined) {
      throw new Error(`The response for ${this.resource} contained an undefined item.`);
    }

    return item;
  }
}

@Injectable({ providedIn: 'root' })
export class EntityCrudFactory {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);

  create<TCreate, TUpdate, TResponse>(resource: string): EntityCrudRequester<TCreate, TUpdate, TResponse> {
    return new EntityCrudRequester<TCreate, TUpdate, TResponse>(this.http, this.config, resource);
  }
}

export type EntityCrudService<TCreate, TUpdate, TResponse> = EntityCrudRequester<TCreate, TUpdate, TResponse>;
