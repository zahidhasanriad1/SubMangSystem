import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';
import { AuthStore } from '../../core/auth/auth.store';
import { ToastService } from '../../core/services/toast.service';
import { AppRole } from '../../data/enums/app-role';
import { AssignmentStatus } from '../../data/enums/assignment-status';
import { AcademicService } from '../../data/services/academic.service';
import { AssignmentService } from '../../data/services/assignment.service';
import { SubmissionService } from '../../data/services/submission.service';
import { CourseOffering } from '../../data/types/academic/course-offering';
import { Assignment } from '../../data/types/assignments/assignment';
import { CreateAssignment } from '../../data/types/assignments/create-assignment';
import { UpdateAssignment } from '../../data/types/assignments/update-assignment';
import { SelectOption } from '../../data/types/common/select-option';

@Component({
  selector: 'app-assignments',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    ButtonModule,
    ConfirmDialogModule,
    DatePickerModule,
    DialogModule,
    InputNumberModule,
    InputTextModule,
    SelectModule,
    TableModule,
    TagModule,
    TextareaModule,
    ToggleSwitchModule,
    TooltipModule
  ],
  templateUrl: './assignments.component.html',
  styleUrl: './assignments.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssignmentsComponent {
  private readonly assignmentService = inject(AssignmentService);
  private readonly academicService = inject(AcademicService);
  private readonly submissionService = inject(SubmissionService);
  private readonly toast = inject(ToastService);
  private readonly confirmation = inject(ConfirmationService);
  private readonly formBuilder = inject(FormBuilder);
  readonly auth = inject(AuthStore);

  readonly assignments = signal<Assignment[]>([]);
  readonly courses = signal<CourseOffering[]>([]);
  readonly totalRecords = signal(0);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly editorVisible = signal(false);
  readonly detailsVisible = signal(false);
  readonly submitVisible = signal(false);
  readonly editingAssignment = signal<Assignment | null>(null);
  readonly detailAssignment = signal<Assignment | null>(null);
  readonly submissionAssignment = signal<Assignment | null>(null);
  readonly isTeacher = computed(() => this.auth.role() === AppRole.Teacher);
  readonly isStudent = computed(() => this.auth.role() === AppRole.Student);
  readonly AssignmentStatus = AssignmentStatus;
  readonly minimumDeadline = new Date();
  readonly search = this.formBuilder.nonNullable.control('');
  readonly statusFilter = this.formBuilder.control<AssignmentStatus | null>(null);
  readonly statusOptions: SelectOption<AssignmentStatus>[] = [
    { label: 'Draft', value: AssignmentStatus.Draft },
    { label: 'Published', value: AssignmentStatus.Published },
    { label: 'Archived', value: AssignmentStatus.Archived }
  ];
  readonly form = this.formBuilder.group({
    courseOfferingId: ['', Validators.required],
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(5000)]],
    deadline: [null as Date | null, Validators.required],
    maximumMarks: [100, [Validators.required, Validators.min(1), Validators.max(10000)]],
    allowResubmission: [true],
    status: [AssignmentStatus.Draft, Validators.required]
  });
  readonly submitForm = this.formBuilder.nonNullable.group({
    answer: ['', [Validators.required, Validators.maxLength(20000)]]
  });

  private page = 1;
  private pageSize = 10;

  constructor() {
    this.academicService.getCourseOfferings().subscribe((courses) => this.courses.set(courses));
    this.search.valueChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed()).subscribe(() => this.resetAndLoad());
    this.statusFilter.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => this.resetAndLoad());
  }

  load(event?: { first?: number | null; rows?: number | null }): void {
    if (event) {
      this.pageSize = event.rows ?? this.pageSize;
      this.page = Math.floor((event.first ?? 0) / this.pageSize) + 1;
    }
    this.loadAssignments();
  }

  openCreate(): void {
    this.editingAssignment.set(null);
    this.form.controls.courseOfferingId.enable();
    this.form.reset({
      courseOfferingId: '',
      title: '',
      description: '',
      deadline: null,
      maximumMarks: 100,
      allowResubmission: true,
      status: AssignmentStatus.Draft
    });
    this.editorVisible.set(true);
  }

  openEdit(item: Assignment): void {
    this.editingAssignment.set(item);
    this.form.controls.courseOfferingId.disable();
    this.form.patchValue({
      courseOfferingId: item.courseOfferingId,
      title: item.title,
      description: item.description,
      deadline: new Date(item.deadlineUtc),
      maximumMarks: item.maximumMarks,
      allowResubmission: item.allowResubmission,
      status: item.status
    });
    this.editorVisible.set(true);
  }

  openDetails(item: Assignment): void {
    this.detailAssignment.set(item);
    this.detailsVisible.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    if (!value.deadline) return;

    const current = this.editingAssignment();
    const request = current
      ? this.assignmentService.update(current.assignmentId, {
          title: value.title ?? '',
          description: value.description ?? '',
          deadlineUtc: value.deadline.toISOString(),
          maximumMarks: value.maximumMarks ?? 0,
          allowResubmission: value.allowResubmission ?? false
        } satisfies UpdateAssignment)
      : this.assignmentService.create({
          courseOfferingId: value.courseOfferingId ?? '',
          title: value.title ?? '',
          description: value.description ?? '',
          deadlineUtc: value.deadline.toISOString(),
          maximumMarks: value.maximumMarks ?? 0,
          allowResubmission: value.allowResubmission ?? false,
          status: value.status ?? AssignmentStatus.Draft
        } satisfies CreateAssignment);

    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.toast.success(current ? 'Assignment updated.' : 'Assignment created.');
      this.editorVisible.set(false);
      this.loadAssignments();
    });
  }

  changeStatus(item: Assignment, status: AssignmentStatus): void {
    this.assignmentService.changeStatus(item.assignmentId, status).subscribe(() => {
      this.toast.success(`Assignment ${this.statusLabel(status).toLowerCase()}.`);
      this.loadAssignments();
    });
  }

  confirmDelete(item: Assignment): void {
    this.confirmation.confirm({
      header: 'Delete draft assignment?',
      message: `“${item.title}” will be permanently removed.`,
      icon: 'pi pi-exclamation-triangle',
      acceptButtonProps: { label: 'Delete', severity: 'danger' },
      rejectButtonProps: { label: 'Cancel', severity: 'secondary', outlined: true },
      accept: () => this.assignmentService.delete(item.assignmentId).subscribe(() => {
        this.toast.success('Draft assignment deleted.');
        this.loadAssignments();
      })
    });
  }

  openSubmit(item: Assignment): void {
    this.submissionAssignment.set(item);
    this.submitForm.reset({ answer: '' });
    this.submitVisible.set(true);

    if (item.hasSubmitted) {
      this.submissionService.getSubmissions({ page: 1, pageSize: 1, assignmentId: item.assignmentId })
        .subscribe((result) => {
          if (this.submissionAssignment()?.assignmentId === item.assignmentId)
            this.submitForm.setValue({ answer: result.items[0]?.answer ?? '' });
        });
    }
  }

  submitAnswer(): void {
    const assignment = this.submissionAssignment();
    if (!assignment || this.submitForm.invalid) {
      this.submitForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.submissionService.submit(assignment.assignmentId, this.submitForm.controls.answer.value)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.toast.success('Submission saved successfully.');
        this.submitVisible.set(false);
        this.loadAssignments();
      });
  }

  statusLabel(status: AssignmentStatus): string {
    return this.statusOptions.find((option) => option.value === status)?.label ?? 'Unknown';
  }

  statusSeverity(status: AssignmentStatus): 'secondary' | 'success' | 'warn' {
    return status === AssignmentStatus.Published ? 'success' : status === AssignmentStatus.Archived ? 'warn' : 'secondary';
  }

  isOverdue(item: Assignment): boolean {
    return item.status === AssignmentStatus.Published && Date.parse(item.deadlineUtc) <= Date.now();
  }

  private resetAndLoad(): void {
    this.page = 1;
    this.loadAssignments();
  }

  private loadAssignments(): void {
    this.loading.set(true);
    this.assignmentService.getAssignments({
      page: this.page,
      pageSize: this.pageSize,
      search: this.search.value.trim() || undefined,
      status: this.statusFilter.value ?? undefined
    }).pipe(finalize(() => this.loading.set(false))).subscribe((result) => {
      this.assignments.set(result.items);
      this.totalRecords.set(result.totalCount);
    });
  }
}
