import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { ApiClient } from '../http/api-client';
import { AuthResponse } from '../../data/types/auth/auth-response';
import { CurrentUser } from '../../data/types/auth/current-user';
import { LoginRequest } from '../../data/types/auth/login-request';

const TOKEN_KEY = 'assignflow_token';
const USER_KEY = 'assignflow_user';
const EXPIRY_KEY = 'assignflow_expiry';
const LOGIN_AT_KEY = 'assignflow_login_at';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly api = inject(ApiClient);
  private readonly router = inject(Router);
  private readonly userState = signal<CurrentUser | null>(this.readUser());
  private readonly loginAtState = signal<string | null>(sessionStorage.getItem(LOGIN_AT_KEY));

  readonly user = this.userState.asReadonly();
  readonly loginAt = this.loginAtState.asReadonly();
  readonly role = computed(() => this.userState()?.role ?? '');
  readonly initials = computed(() => {
    const name = this.userState()?.fullName ?? '';
    return name.split(' ').filter(Boolean).slice(0, 2).map((part) => part[0]).join('').toUpperCase();
  });

  get token(): string | null {
    // Session storage limits credentials to the current browser session and avoids long-lived token persistence.
    const expiry = sessionStorage.getItem(EXPIRY_KEY);
    if (expiry && Date.parse(expiry) <= Date.now()) {
      this.clear();
      return null;
    }
    return sessionStorage.getItem(TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    // Guards evaluate current session state on every navigation, including immediately after login persistence.
    return !!this.userState() && !!this.token;
  }

  login(model: LoginRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/login', model).pipe(tap((response) => this.persist(response)));
  }

  logout(): void {
    this.clear();
    void this.router.navigate(['/login']);
  }

  clear(): void {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(USER_KEY);
    sessionStorage.removeItem(EXPIRY_KEY);
    sessionStorage.removeItem(LOGIN_AT_KEY);
    this.userState.set(null);
    this.loginAtState.set(null);
  }

  private persist(response: AuthResponse): void {
    const loginAt = new Date().toISOString();
    sessionStorage.setItem(TOKEN_KEY, response.accessToken);
    sessionStorage.setItem(USER_KEY, JSON.stringify(response.user));
    sessionStorage.setItem(EXPIRY_KEY, response.expiresAtUtc);
    sessionStorage.setItem(LOGIN_AT_KEY, loginAt);
    this.userState.set(response.user);
    this.loginAtState.set(loginAt);
  }

  private readUser(): CurrentUser | null {
    try {
      const value = sessionStorage.getItem(USER_KEY);
      return value ? JSON.parse(value) as CurrentUser : null;
    } catch {
      return null;
    }
  }
}
