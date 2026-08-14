import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { DashboardSummary } from '../types/dashboard/dashboard-summary';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly api = inject(ApiClient);

  getSummary(): Observable<DashboardSummary> {
    return this.api.get<DashboardSummary>('dashboard/summary');
  }
}
