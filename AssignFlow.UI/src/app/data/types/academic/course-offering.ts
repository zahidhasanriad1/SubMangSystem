export interface CourseOffering {
  courseOfferingId: string;
  classRoomId: string;
  className: string;
  section: string;
  academicYear: number;
  subjectId: string;
  subjectCode: string;
  subjectName: string;
  isActive: boolean;
  teacherCount: number;
  studentCount: number;
}
