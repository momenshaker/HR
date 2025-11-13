import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { tap, Observable, map, of, catchError, throwError, finalize } from 'rxjs';
import { APP_CONFIG } from '../config/app-config.token';
import { AppConfig } from '../config/app-config.model';
import { AuthStore } from './auth.store';
import { AuthTokens, AuthUser, LoginRequest, ProblemDetails } from './auth.models';
import { TokenStorageService } from '../services/token-storage.service';
import { extractProblemMessage, normalizeProblemDetails } from '../errors/problem-details';
import {
  decodeJwtPayload,
  extractRoles,
  getEmailClaimKeys,
  getIdClaimKeys,
  getNameClaimKeys,
  getJwtClaim,
  EMPLOYEE_ID_CLAIM
} from './jwt.utils';

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user?: AuthUser;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly store = inject(AuthStore);
  private readonly tokenStorage = inject(TokenStorageService);

  initialize(): void {
    const tokens = this.tokenStorage.tokens;
    if (tokens) {
      this.store.setTokens(tokens);
      this.loadProfile().subscribe();
    }
  }

  login(request: LoginRequest): Observable<AuthUser | null> {
    this.store.setLoading(true);
    return this.http.post<LoginResponse>(`${this.config.apiBaseUrl}/auth/login`, request).pipe(
      map((response) => {
        const tokens: AuthTokens = {
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
          expiresIn: response.expiresIn
        };
        const user = response.user ?? this.createUserFromAccessToken(tokens.accessToken);
        return { tokens, user };
      }),
      tap(({ tokens, user }) => {
        this.tokenStorage.save(tokens);
        this.store.setTokens(tokens);
        this.store.setUser(user ?? null);
      }),
      map(({ user }) => user ?? null),
      catchError((error) => this.handleError(error)),
      finalize(() => this.store.setLoading(false))
    );
  }

  refresh(): Observable<AuthTokens | null> {
    const refreshToken = this.tokenStorage.refreshToken;
    if (!refreshToken) {
      return of(null);
    }

    return this.http
      .post<AuthTokens>(`${this.config.apiBaseUrl}/auth/refresh`, { refreshToken })
      .pipe(
        tap((tokens) => {
          this.tokenStorage.save(tokens);
          this.store.setTokens(tokens);
        }),
        catchError((error) => this.handleError(error))
      );
  }

  loadProfile(): Observable<AuthUser | null> {
    return this.http.get<AuthUser>(`${this.config.apiBaseUrl}/auth/me`).pipe(
      tap((user) => this.store.setUser(user)),
      catchError((error) => {
        if (error.status === 401) {
          this.clearSession();
          return of(null);
        }

        return this.handleError(error);
      })
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.config.apiBaseUrl}/auth/logout`, {}).pipe(
      tap(() => this.clearSession()),
      catchError((error) => this.handleError(error))
    );
  }

  private clearSession(): void {
    this.tokenStorage.clear();
    this.store.reset();
  }

  private handleError(error: any): Observable<never> {
    const normalized =
      normalizeProblemDetails(error?.error as ProblemDetails | Record<string, unknown>) ??
      normalizeProblemDetails(error) ??
      { title: 'Unexpected error' };
    const message = extractProblemMessage(normalized);
    this.store.setError(message, true);
    return throwError(() => normalized);
  }

  private createUserFromAccessToken(accessToken: string): AuthUser | null {
    const payload = decodeJwtPayload(accessToken);
    if (!payload) {
      return null;
    }

    const roles = extractRoles(payload);
    const id = getJwtClaim(payload, getIdClaimKeys()) ?? '';
    const fullName = getJwtClaim(payload, getNameClaimKeys()) ?? '';
    const email = getJwtClaim(payload, getEmailClaimKeys()) ?? '';
    const employeeId = getJwtClaim(payload, [EMPLOYEE_ID_CLAIM]);

    return {
      id,
      fullName,
      email,
      roles,
      employeeId: employeeId ?? undefined
    };
  }
}
