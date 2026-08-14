import { AssignmentStatus } from '../../enums/assignment-status';

export interface CreateAssignment {
  courseOfferingId: string;
  title: string;
  description: string;
  deadlineUtc: string;
  maximumMarks: number;
  allowResubmission: boolean;
  status: AssignmentStatus;
}
