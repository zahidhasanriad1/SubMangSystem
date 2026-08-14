import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../core/http/api-client';
import { ClassRoom } from '../types/academic/class-room';
import { CourseOffering } from '../types/academic/course-offering';
import { Subject } from '../types/academic/subject';
import { UpsertClassRoom } from '../types/academic/upsert-class-room';
import { UpsertCourseOffering } from '../types/academic/upsert-course-offering';
import { UpsertSubject } from '../types/academic/upsert-subject';

@Injectable({ providedIn: 'root' })
export class AcademicService {
  private readonly api = inject(ApiClient);

  getClassRooms(): Observable<ClassRoom[]> { return this.api.get<ClassRoom[]>('academic/classes'); }
  createClassRoom(model: UpsertClassRoom): Observable<ClassRoom> { return this.api.post<ClassRoom>('academic/classes', model); }
  updateClassRoom(id: string, model: UpsertClassRoom): Observable<ClassRoom> { return this.api.put<ClassRoom>(`academic/classes/${id}`, model); }
  deleteClassRoom(id: string): Observable<boolean> { return this.api.delete<boolean>(`academic/classes/${id}`); }
  getSubjects(): Observable<Subject[]> { return this.api.get<Subject[]>('academic/subjects'); }
  createSubject(model: UpsertSubject): Observable<Subject> { return this.api.post<Subject>('academic/subjects', model); }
  updateSubject(id: string, model: UpsertSubject): Observable<Subject> { return this.api.put<Subject>(`academic/subjects/${id}`, model); }
  deleteSubject(id: string): Observable<boolean> { return this.api.delete<boolean>(`academic/subjects/${id}`); }
  getCourseOfferings(): Observable<CourseOffering[]> { return this.api.get<CourseOffering[]>('academic/course-offerings'); }
  createCourseOffering(model: UpsertCourseOffering): Observable<CourseOffering> { return this.api.post<CourseOffering>('academic/course-offerings', model); }
  updateCourseOffering(id: string, model: UpsertCourseOffering): Observable<CourseOffering> { return this.api.put<CourseOffering>(`academic/course-offerings/${id}`, model); }
  deleteCourseOffering(id: string): Observable<boolean> { return this.api.delete<boolean>(`academic/course-offerings/${id}`); }
  assignTeacher(courseId: string, teacherId: string): Observable<boolean> { return this.api.put<boolean>(`academic/course-offerings/${courseId}/teachers/${teacherId}`); }
  removeTeacher(courseId: string, teacherId: string): Observable<boolean> { return this.api.delete<boolean>(`academic/course-offerings/${courseId}/teachers/${teacherId}`); }
  enrollStudent(courseId: string, studentId: string): Observable<boolean> { return this.api.put<boolean>(`academic/course-offerings/${courseId}/students/${studentId}`); }
  removeStudent(courseId: string, studentId: string): Observable<boolean> { return this.api.delete<boolean>(`academic/course-offerings/${courseId}/students/${studentId}`); }
}
