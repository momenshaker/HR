import { HttpInterceptorFn } from '@angular/common/http';
import { authTokenInterceptor } from './auth-token.interceptor';
import { refreshTokenInterceptor } from './refresh-token.interceptor';
import { errorInterceptor } from './error.interceptor';
import { idempotencyInterceptor } from './idempotency.interceptor';

export const appHttpInterceptors: HttpInterceptorFn[] = [
  authTokenInterceptor,
  refreshTokenInterceptor,
  idempotencyInterceptor,
  errorInterceptor
];
