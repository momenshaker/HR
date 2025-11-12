import { inject } from '@angular/core';
import { CanMatchFn, Router, Route, UrlSegment } from '@angular/router';
import { AuthStore } from './auth.store';
import { UserRole } from './auth.models';

export function roleGuard(requiredRoles: ReadonlyArray<UserRole>): CanMatchFn {
  return (_route: Route, _segments: UrlSegment[]) => {
    const router = inject(Router);
    const authStore = inject(AuthStore);
    const hasRole = authStore.roles().some((role) => requiredRoles.includes(role));

    return hasRole ? true : router.createUrlTree(['/forbidden']);
  };
}
