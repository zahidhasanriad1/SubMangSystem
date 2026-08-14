import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, EMPTY } from 'rxjs';
import { AuthStore } from '../auth/auth.store';
import { ToastService } from '../services/toast.service';

export const apiInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  const toast = inject(ToastService);
  const token = auth.token;
  const authorizedRequest = token
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      // API problem details are surfaced consistently while authentication failures terminate the local session.
      const detail = typeof error.error?.title === 'string'
        ? error.error.title
        : 'The request could not be completed. Please try again.';

      if (error.status === 401 && !request.url.endsWith('/auth/login')) {
        auth.clear();
        void router.navigate(['/login']);
      }
      toast.error(detail);
      return EMPTY;
    })
  );
};
