import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthStore } from '../auth/auth.store';
import { extractProblemMessage, normalizeProblemDetails } from '../errors/problem-details';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authStore = inject(AuthStore);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        authStore.setError('Unable to reach the server. Please check your network connection.', true);
        return throwError(() => error);
      }

      const payload = error.error ?? error;
      const problem = normalizeProblemDetails(payload);
      const message = extractProblemMessage(problem);
      authStore.setError(message, true);

      return throwError(() => error);
    })
  );
};
