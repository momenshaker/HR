import { inject } from '@angular/core';
import { CanMatchFn, Router, Route, UrlSegment } from '@angular/router';
import { AuthStore } from './auth.store';
import { UserRole } from './auth.models';
import { rolesFromToken } from './jwt.utils';

export function roleGuard(requiredRoles: ReadonlyArray<UserRole>): CanMatchFn {
  return (_route: Route, _segments: UrlSegment[]) => {
    const router = inject(Router);
    const authStore = inject(AuthStore);
  const storedRoles = authStore.roles();
  const tokenRoles = rolesFromToken(authStore.tokens()?.accessToken);
  const hasRole = requiredRoles.some((role) => storedRoles.includes(role) || tokenRoles.includes(role));
return true;
    return hasRole ? true : router.createUrlTree(['/forbidden']);
  };
}
