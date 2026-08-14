import { SubmissionStatus } from '../../enums/submission-status';

export interface Submission {
  submissionId: string;
  assignmentId: string;
  assignmentTitle: string;
  studentUserId: string;
  studentName: string;
  studentEmail: string;
  answer: string;
  submittedAtUtc: string;
  status: SubmissionStatus;
  marks: number | null;
  feedback: string | null;
  gradedAtUtc: string | null;
  maximumMarks: number;
}
