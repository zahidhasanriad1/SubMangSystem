import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { AssignmentStatus } from '../enums/assignment-status';
import { Assignment } from '../types/assignments/assignment';
import { AssignmentFilter } from '../types/assignments/assignment-filter';
import { CreateAssignment } from '../types/assignments/create-assignment';
import { UpdateAssignment } from '../types/assignments/update-assignment';
import { PagedResult } from '../types/common/paged-result';

@Injectable({ providedIn: 'root' })
export class AssignmentService {
  private readonly api = inject(ApiClient);

  getAssignments(filter: AssignmentFilter): Observable<PagedResult<Assignment>> {
    return this.api.get<PagedResult<Assignment>>('assignments', { ...filter });
  }

  create(model: CreateAssignment): Observable<Assignment> { return this.api.post<Assignment>('assignments', model); }
  update(id: string, model: UpdateAssignment): Observable<Assignment> { return this.api.put<Assignment>(`assignments/${id}`, model); }
  changeStatus(id: string, status: AssignmentStatus): Observable<Assignment> { return this.api.patch<Assignment>(`assignments/${id}/status`, { status }); }
  delete(id: string): Observable<boolean> { return this.api.delete<boolean>(`assignments/${id}`); }
}
