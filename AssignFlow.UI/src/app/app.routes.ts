import { Routes } from '@angular/router';
import { AppRole } from './data/enums/app-role';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Sign in | AssignFlow',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login.component').then((component) => component.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell.component').then((component) => component.ShellComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        title: 'Overview | AssignFlow',
        loadComponent: () => import('./features/dashboard/dashboard.component').then((component) => component.DashboardComponent)
      },
      {
        path: 'users',
        title: 'People | AssignFlow',
        canActivate: [roleGuard],
        data: { roles: [AppRole.Admin] },
        loadComponent: () => import('./features/users/users.component').then((component) => component.UsersComponent)
      },
      {
        path: 'academic',
        title: 'Academics | AssignFlow',
        loadComponent: () => import('./features/academic/academic.component').then((component) => component.AcademicComponent)
      },
      {
        path: 'assignments',
        title: 'Assignments | AssignFlow',
        loadComponent: () => import('./features/assignments/assignments.component').then((component) => component.AssignmentsComponent)
      },
      {
        path: 'submissions',
        title: 'Submissions | AssignFlow',
        loadComponent: () => import('./features/submissions/submissions.component').then((component) => component.SubmissionsComponent)
      },
      {
        path: 'settings',
        title: 'Settings | AssignFlow',
        canActivate: [roleGuard],
        data: { roles: [AppRole.Admin] },
        loadComponent: () => import('./features/settings/settings.component').then((component) => component.SettingsComponent)
      }
    ]
  },
  {
    path: '**',
    title: 'Page not found | AssignFlow',
    loadComponent: () => import('./features/not-found/not-found.component').then((component) => component.NotFoundComponent)
  }
];
