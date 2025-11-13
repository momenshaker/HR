import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';
import { ProblemDetails } from '../errors/problem-details';

export interface ApiResponse<T> {
  data: T;
  meta?: {
    totalItems?: number;
    totalPages?: number;
    currentPage?: number;
    pageSize?: number;
  };
  errors?: ProblemDetails;
}

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

  list(query: QueryParams = {}): Observable<ApiResponse<TResponse[]>> {
    const params = this.toHttpParams(query);
    return this.http.get<ApiResponse<TResponse[]>>(`${this.config.apiBaseUrl}/${this.resource}`, { params });
  }

  getById(id: string): Observable<TResponse> {
    return this.http
      .get<ApiResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}/${id}`)
      .pipe(map((response) => response.data));
  }

  create(payload: TCreate): Observable<TResponse> {
    return this.http
      .post<ApiResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}`, payload)
      .pipe(map((response) => response.data));
  }

  update(id: string, payload: TUpdate): Observable<TResponse> {
    return this.http
      .put<ApiResponse<TResponse>>(`${this.config.apiBaseUrl}/${this.resource}/${id}`, payload)
      .pipe(map((response) => response.data));
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
