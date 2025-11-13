import { HttpInterceptorFn } from '@angular/common/http';

const generateIdempotencyKey = (): string =>
  globalThis.crypto?.randomUUID?.() ??
  `${Math.random().toString(36).slice(2)}-${Date.now().toString(36)}`;

export const idempotencyInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'POST') {
    const cloned = req.clone({
      setHeaders: {
        'Idempotency-Key': generateIdempotencyKey()
      }
    });
    return next(cloned);
  }

  return next(req);
};
