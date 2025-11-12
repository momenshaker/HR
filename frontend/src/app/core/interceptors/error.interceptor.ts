import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthStore } from '../auth/auth.store';
import { ProblemDetails } from '../auth/auth.models';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authStore = inject(AuthStore);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        authStore.setError('Unable to reach the server. Please check your network connection.', true);
        return throwError(() => error);
      }

      const problem = error.error as ProblemDetails | undefined;
      const message = problem?.detail ?? problem?.title ?? 'An unexpected error occurred';
      authStore.setError(message, true);

      return throwError(() => error);
    })
  );
};
