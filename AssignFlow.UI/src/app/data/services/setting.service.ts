import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { SystemSetting } from '../types/settings/system-setting';
import { UpsertSetting } from '../types/settings/upsert-setting';

@Injectable({ providedIn: 'root' })
export class SettingService {
  private readonly api = inject(ApiClient);

  getSettings(): Observable<SystemSetting[]> { return this.api.get<SystemSetting[]>('admin/settings'); }
  upsert(key: string, model: UpsertSetting): Observable<SystemSetting> { return this.api.put<SystemSetting>(`admin/settings/${encodeURIComponent(key)}`, model); }
}
