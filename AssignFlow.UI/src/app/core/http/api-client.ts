import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../data/types/common/api-response';

type QueryValue = string | number | boolean | null | undefined;

@Injectable({ providedIn: 'root' })
export class ApiClient {
  private readonly http = inject(HttpClient);

  get<T>(path: string, query: Record<string, QueryValue> = {}): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(this.url(path), { params: this.toParams(query) })
      .pipe(map((response) => response.data));
  }

  post<T>(path: string, body: unknown): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.url(path), body).pipe(map((response) => response.data));
  }

  put<T>(path: string, body: unknown = {}): Observable<T> {
    return this.http.put<ApiResponse<T>>(this.url(path), body).pipe(map((response) => response.data));
  }

  patch<T>(path: string, body: unknown): Observable<T> {
    return this.http.patch<ApiResponse<T>>(this.url(path), body).pipe(map((response) => response.data));
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(this.url(path)).pipe(map((response) => response.data));
  }

  private url(path: string): string {
    return `${environment.apiUrl}/${path}`;
  }

  private toParams(query: Record<string, QueryValue>): HttpParams {
    // Empty filters are intentionally omitted so ASP.NET Core receives clean optional query parameters.
    return Object.entries(query).reduce((params, [key, value]) => {
      return value === undefined || value === null || value === '' ? params : params.set(key, value);
    }, new HttpParams());
  }
}
