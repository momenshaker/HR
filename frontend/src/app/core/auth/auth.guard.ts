import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from './auth.store';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authStore = inject(AuthStore);

  if (authStore.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/auth/login']);
};
