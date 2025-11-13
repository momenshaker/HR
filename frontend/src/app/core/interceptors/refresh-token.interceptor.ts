import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const tokenStorage = inject(TokenStorageService);
  const isAuthEndpoint = req.url.includes('/auth/');

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint && tokenStorage.refreshToken) {
        return authService.refresh().pipe(
          switchMap((tokens) => {
            if (!tokens) {
              return throwError(() => error);
            }

            const cloned = req.clone({
              setHeaders: {
                Authorization: `Bearer ${tokens.accessToken}`
              }
            });
            return next(cloned);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
