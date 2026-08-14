import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../auth/auth.store';

export const roleGuard: CanActivateFn = (route) => {
  const allowedRoles = route.data['roles'] as string[] | undefined;
  return !allowedRoles || allowedRoles.includes(inject(AuthStore).role())
    ? true
    : inject(Router).createUrlTree(['/dashboard']);
};
