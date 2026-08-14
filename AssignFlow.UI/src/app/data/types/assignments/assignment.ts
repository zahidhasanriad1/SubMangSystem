import { AssignmentStatus } from '../../enums/assignment-status';

export interface Assignment {
  assignmentId: string;
  courseOfferingId: string;
  courseName: string;
  subjectCode: string;
  title: string;
  description: string;
  deadlineUtc: string;
  maximumMarks: number;
  allowResubmission: boolean;
  status: AssignmentStatus;
  publishedAtUtc: string | null;
  createdByUserId: string;
  teacherName: string;
  submissionCount: number;
  hasSubmitted: boolean;
  createdAtUtc: string;
}
