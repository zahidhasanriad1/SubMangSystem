import { SubmissionStatus } from '../../enums/submission-status';

export interface SubmissionFilter {
  page: number;
  pageSize: number;
  search?: string;
  assignmentId?: string;
  status?: SubmissionStatus;
}
