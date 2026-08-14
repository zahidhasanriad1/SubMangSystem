import { AssignmentStatus } from '../../enums/assignment-status';

export interface AssignmentFilter {
  page: number;
  pageSize: number;
  search?: string;
  courseOfferingId?: string;
  status?: AssignmentStatus;
}
