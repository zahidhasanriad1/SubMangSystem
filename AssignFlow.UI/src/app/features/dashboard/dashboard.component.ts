import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { AuthStore } from '../../core/auth/auth.store';
import { AppRole } from '../../data/enums/app-role';
import { DashboardService } from '../../data/services/dashboard.service';
import { DashboardSummary } from '../../data/types/dashboard/dashboard-summary';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe, RouterLink, ButtonModule, SkeletonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent {
  readonly auth = inject(AuthStore);
  readonly dashboardOpenedAt = new Date().toISOString();
  readonly loading = signal(true);
  readonly summary = signal<DashboardSummary>({
    users: 0,
    courses: 0,
    assignments: 0,
    publishedAssignments: 0,
    submissions: 0,
    pendingReviews: 0
  });
  readonly cards = computed(() => {
    const data = this.summary();
    return [
      { label: this.auth.role() === AppRole.Admin ? 'Total users' : 'My courses', value: this.auth.role() === AppRole.Admin ? data.users : data.courses, icon: 'pi pi-users', tone: 'teal' },
      { label: 'Assignments', value: data.assignments, icon: 'pi pi-file-edit', tone: 'blue' },
      { label: 'Published', value: data.publishedAssignments, icon: 'pi pi-send', tone: 'amber' },
      { label: 'Submissions', value: data.submissions, icon: 'pi pi-inbox', tone: 'violet' },
      { label: 'Awaiting review', value: data.pendingReviews, icon: 'pi pi-clock', tone: 'rose' }
    ];
  });
  readonly publishingRate = computed(() => {
    const data = this.summary();
    return data.assignments ? Math.round((data.publishedAssignments / data.assignments) * 100) : 0;
  });
  readonly loginAt = computed(() => this.auth.loginAt() ?? this.dashboardOpenedAt);

  private readonly dashboardService = inject(DashboardService);

  constructor() {
    this.dashboardService.getSummary().subscribe({
      next: (summary) => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
