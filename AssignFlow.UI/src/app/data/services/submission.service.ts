import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { SubmissionStatus } from '../enums/submission-status';
import { PagedResult } from '../types/common/paged-result';
import { GradeSubmission } from '../types/submissions/grade-submission';
import { Submission } from '../types/submissions/submission';
import { SubmissionFilter } from '../types/submissions/submission-filter';

@Injectable({ providedIn: 'root' })
export class SubmissionService {
  private readonly api = inject(ApiClient);

  getSubmissions(filter: SubmissionFilter): Observable<PagedResult<Submission>> {
    return this.api.get<PagedResult<Submission>>('submissions', { ...filter });
  }

  submit(assignmentId: string, answer: string): Observable<Submission> {
    return this.api.put<Submission>(`submissions/assignment/${assignmentId}`, { answer });
  }

  grade(submissionId: string, model: GradeSubmission): Observable<Submission> {
    return this.api.put<Submission>(`submissions/${submissionId}/grade`, model);
  }

  changeStatus(submissionId: string, status: SubmissionStatus): Observable<Submission> {
    return this.api.patch<Submission>(`submissions/${submissionId}/status`, { status });
  }
}
