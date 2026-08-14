import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { AuthStore } from '../../core/auth/auth.store';
import { ToastService } from '../../core/services/toast.service';
import { AppRole } from '../../data/enums/app-role';
import { SubmissionStatus } from '../../data/enums/submission-status';
import { SubmissionService } from '../../data/services/submission.service';
import { SelectOption } from '../../data/types/common/select-option';
import { GradeSubmission } from '../../data/types/submissions/grade-submission';
import { Submission } from '../../data/types/submissions/submission';

@Component({
  selector: 'app-submissions',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    InputNumberModule,
    InputTextModule,
    SelectModule,
    TableModule,
    TagModule,
    TextareaModule,
    TooltipModule
  ],
  templateUrl: './submissions.component.html',
  styleUrl: './submissions.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SubmissionsComponent {
  private readonly submissionService = inject(SubmissionService);
  private readonly toast = inject(ToastService);
  private readonly formBuilder = inject(FormBuilder);
  readonly auth = inject(AuthStore);

  readonly submissions = signal<Submission[]>([]);
  readonly totalRecords = signal(0);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly reviewVisible = signal(false);
  readonly selectedSubmission = signal<Submission | null>(null);
  readonly isTeacher = computed(() => this.auth.role() === AppRole.Teacher);
  readonly SubmissionStatus = SubmissionStatus;
  readonly search = this.formBuilder.nonNullable.control('');
  readonly statusFilter = this.formBuilder.control<SubmissionStatus | null>(null);
  readonly statusOptions: SelectOption<SubmissionStatus>[] = [
    { label: 'Submitted', value: SubmissionStatus.Submitted },
    { label: 'Under review', value: SubmissionStatus.UnderReview },
    { label: 'Graded', value: SubmissionStatus.Graded },
    { label: 'Returned', value: SubmissionStatus.Returned }
  ];
  readonly gradeForm = this.formBuilder.nonNullable.group({
    marks: [0, [Validators.required, Validators.min(0)]],
    feedback: ['', [Validators.required, Validators.maxLength(5000)]]
  });

  private page = 1;
  private pageSize = 10;

  constructor() {
    this.search.valueChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed()).subscribe(() => this.resetAndLoad());
    this.statusFilter.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => this.resetAndLoad());
  }

  load(event?: { first?: number | null; rows?: number | null }): void {
    if (event) {
      this.pageSize = event.rows ?? this.pageSize;
      this.page = Math.floor((event.first ?? 0) / this.pageSize) + 1;
    }
    this.loadSubmissions();
  }

  openReview(item: Submission): void {
    this.selectedSubmission.set(item);
    this.gradeForm.controls.marks.setValidators([Validators.required, Validators.min(0), Validators.max(item.maximumMarks)]);
    this.gradeForm.reset({ marks: item.marks ?? 0, feedback: item.feedback ?? '' });
    this.reviewVisible.set(true);
  }

  grade(): void {
    const submission = this.selectedSubmission();
    if (!submission || this.gradeForm.invalid) {
      this.gradeForm.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.submissionService.grade(submission.submissionId, this.gradeForm.getRawValue() satisfies GradeSubmission)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.toast.success('Feedback and marks published.');
        this.reviewVisible.set(false);
        this.loadSubmissions();
      });
  }

  changeStatus(status: SubmissionStatus): void {
    const submission = this.selectedSubmission();
    if (!submission) return;
    this.saving.set(true);
    this.submissionService.changeStatus(submission.submissionId, status)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe(() => {
        this.toast.success('Submission status updated.');
        this.reviewVisible.set(false);
        this.loadSubmissions();
      });
  }

  statusLabel(status: SubmissionStatus): string {
    return this.statusOptions.find((option) => option.value === status)?.label ?? 'Unknown';
  }

  statusSeverity(status: SubmissionStatus): 'secondary' | 'info' | 'success' | 'warn' {
    if (status === SubmissionStatus.Graded) return 'success';
    if (status === SubmissionStatus.UnderReview) return 'info';
    if (status === SubmissionStatus.Returned) return 'warn';
    return 'secondary';
  }

  private resetAndLoad(): void {
    this.page = 1;
    this.loadSubmissions();
  }

  private loadSubmissions(): void {
    this.loading.set(true);
    this.submissionService.getSubmissions({
      page: this.page,
      pageSize: this.pageSize,
      search: this.search.value.trim() || undefined,
      status: this.statusFilter.value ?? undefined
    }).pipe(finalize(() => this.loading.set(false))).subscribe((result) => {
      this.submissions.set(result.items);
      this.totalRecords.set(result.totalCount);
    });
  }
}
