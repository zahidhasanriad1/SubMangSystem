import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, Observable } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';
import { AuthStore } from '../../core/auth/auth.store';
import { ToastService } from '../../core/services/toast.service';
import { AppRole } from '../../data/enums/app-role';
import { AcademicService } from '../../data/services/academic.service';
import { AdminService } from '../../data/services/admin.service';
import { ClassRoom } from '../../data/types/academic/class-room';
import { CourseOffering } from '../../data/types/academic/course-offering';
import { Subject } from '../../data/types/academic/subject';
import { UpsertClassRoom } from '../../data/types/academic/upsert-class-room';
import { UpsertCourseOffering } from '../../data/types/academic/upsert-course-offering';
import { UpsertSubject } from '../../data/types/academic/upsert-subject';
import { User } from '../../data/types/users/user';

type AcademicView = 'courses' | 'classes' | 'subjects';
type EditorType = 'course' | 'class' | 'subject';
type MembershipAction = 'assignTeacher' | 'removeTeacher' | 'enrollStudent' | 'removeStudent';

@Component({
  selector: 'app-academic',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    InputNumberModule,
    InputTextModule,
    SelectModule,
    TableModule,
    TagModule,
    ToggleSwitchModule,
    TooltipModule
  ],
  templateUrl: './academic.component.html',
  styleUrl: './academic.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AcademicComponent {
  private readonly academicService = inject(AcademicService);
  private readonly adminService = inject(AdminService);
  private readonly toast = inject(ToastService);
  private readonly formBuilder = inject(FormBuilder);
  readonly auth = inject(AuthStore);

  readonly activeView = signal<AcademicView>('courses');
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly editorVisible = signal(false);
  readonly accessVisible = signal(false);
  readonly editorType = signal<EditorType>('course');
  readonly editingCourse = signal<CourseOffering | null>(null);
  readonly editingClass = signal<ClassRoom | null>(null);
  readonly editingSubject = signal<Subject | null>(null);
  readonly selectedCourse = signal<CourseOffering | null>(null);
  readonly classRooms = signal<ClassRoom[]>([]);
  readonly subjects = signal<Subject[]>([]);
  readonly courses = signal<CourseOffering[]>([]);
  readonly teachers = signal<User[]>([]);
  readonly students = signal<User[]>([]);
  readonly isAdmin = computed(() => this.auth.role() === AppRole.Admin);

  readonly classForm = this.formBuilder.nonNullable.group({
    name: ['', Validators.required],
    section: ['', Validators.required],
    academicYear: [new Date().getFullYear(), [Validators.required, Validators.min(2000), Validators.max(2200)]],
    isActive: [true]
  });
  readonly subjectForm = this.formBuilder.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    isActive: [true]
  });
  readonly courseForm = this.formBuilder.nonNullable.group({
    classRoomId: ['', Validators.required],
    subjectId: ['', Validators.required],
    isActive: [true]
  });
  readonly accessForm = this.formBuilder.nonNullable.group({
    teacherId: [''],
    studentId: ['']
  });

  constructor() {
    this.loadAll();
    if (this.isAdmin()) {
      this.adminService.getUsers(1, 100).subscribe((result) => {
        this.teachers.set(result.items.filter((user) => user.role === AppRole.Teacher && user.isActive));
        this.students.set(result.items.filter((user) => user.role === AppRole.Student && user.isActive));
      });
    }
  }

  openCreate(): void {
    const type = this.activeView() === 'classes' ? 'class' : this.activeView() === 'subjects' ? 'subject' : 'course';
    this.editorType.set(type);
    this.editingCourse.set(null);
    this.editingClass.set(null);
    this.editingSubject.set(null);
    this.classForm.reset({ name: '', section: '', academicYear: new Date().getFullYear(), isActive: true });
    this.subjectForm.reset({ code: '', name: '', isActive: true });
    this.courseForm.reset({ classRoomId: '', subjectId: '', isActive: true });
    this.editorVisible.set(true);
  }

  openCourseEdit(item: CourseOffering): void {
    this.editorType.set('course');
    this.editingCourse.set(item);
    this.courseForm.setValue({ classRoomId: item.classRoomId, subjectId: item.subjectId, isActive: item.isActive });
    this.editorVisible.set(true);
  }

  openClassEdit(item: ClassRoom): void {
    this.editorType.set('class');
    this.editingClass.set(item);
    this.classForm.setValue({ name: item.name, section: item.section, academicYear: item.academicYear, isActive: item.isActive });
    this.editorVisible.set(true);
  }

  openSubjectEdit(item: Subject): void {
    this.editorType.set('subject');
    this.editingSubject.set(item);
    this.subjectForm.setValue({ code: item.code, name: item.name, isActive: item.isActive });
    this.editorVisible.set(true);
  }

  openAccess(course: CourseOffering): void {
    this.selectedCourse.set(course);
    this.accessForm.reset({ teacherId: '', studentId: '' });
    this.accessVisible.set(true);
  }

  saveEditor(): void {
    switch (this.editorType()) {
      case 'class': this.saveClass(); break;
      case 'subject': this.saveSubject(); break;
      case 'course': this.saveCourse(); break;
    }
  }

  updateMembership(action: MembershipAction): void {
    const course = this.selectedCourse();
    const memberId = action.includes('Teacher') ? this.accessForm.controls.teacherId.value : this.accessForm.controls.studentId.value;
    if (!course || !memberId) {
      this.toast.warn('Select a user before continuing.');
      return;
    }

    this.saving.set(true);
    this.academicService[action](course.courseOfferingId, memberId)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.toast.success('Course access updated.');
        this.loadAll();
      });
  }

  private saveClass(): void {
    if (this.classForm.invalid) { this.classForm.markAllAsTouched(); return; }
    const model = this.classForm.getRawValue() satisfies UpsertClassRoom;
    const current = this.editingClass();
    this.saveRequest(current
      ? this.academicService.updateClassRoom(current.classRoomId, model)
      : this.academicService.createClassRoom(model), 'Class details saved.');
  }

  private saveSubject(): void {
    if (this.subjectForm.invalid) { this.subjectForm.markAllAsTouched(); return; }
    const model = this.subjectForm.getRawValue() satisfies UpsertSubject;
    const current = this.editingSubject();
    this.saveRequest(current
      ? this.academicService.updateSubject(current.subjectId, model)
      : this.academicService.createSubject(model), 'Subject details saved.');
  }

  private saveCourse(): void {
    if (this.courseForm.invalid) { this.courseForm.markAllAsTouched(); return; }
    const model = this.courseForm.getRawValue() satisfies UpsertCourseOffering;
    const current = this.editingCourse();
    this.saveRequest(current
      ? this.academicService.updateCourseOffering(current.courseOfferingId, model)
      : this.academicService.createCourseOffering(model), 'Course offering saved.');
  }

  private saveRequest(request: Observable<unknown>, message: string): void {
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.toast.success(message);
      this.editorVisible.set(false);
      this.loadAll();
    });
  }

  private loadAll(): void {
    this.loading.set(true);
    forkJoin({
      classRooms: this.academicService.getClassRooms(),
      subjects: this.academicService.getSubjects(),
      courses: this.academicService.getCourseOfferings()
    }).pipe(finalize(() => this.loading.set(false))).subscribe((result) => {
      this.classRooms.set(result.classRooms);
      this.subjects.set(result.subjects);
      this.courses.set(result.courses);
    });
  }
}
