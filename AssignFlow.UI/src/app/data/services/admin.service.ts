import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { PagedResult } from '../types/common/paged-result';
import { CreateUser } from '../types/users/create-user';
import { UpdateUser } from '../types/users/update-user';
import { User } from '../types/users/user';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly api = inject(ApiClient);

  getUsers(page: number, pageSize: number, search = ''): Observable<PagedResult<User>> {
    return this.api.get<PagedResult<User>>('admin/users', { page, pageSize, search });
  }

  createUser(model: CreateUser): Observable<User> {
    return this.api.post<User>('admin/users', model);
  }

  updateUser(userId: string, model: UpdateUser): Observable<User> {
    return this.api.put<User>(`admin/users/${userId}`, model);
  }
}
