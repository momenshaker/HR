import { Routes } from '@angular/router';
import { AUTH_ROUTES } from './features/auth/auth.routes';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { ForbiddenPageComponent } from './pages/forbidden-page.component';
import { NotFoundPageComponent } from './pages/not-found-page.component';

export const APP_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'auth', children: AUTH_ROUTES },
  {
    path: 'dashboard',
    canMatch: [authGuard],
    loadChildren: () => import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES)
  },
  {
    path: 'organizations',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/organizations/organizations.routes').then((m) => m.ORGANIZATIONS_ROUTES)
  },
  {
    path: 'lookups',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/lookups/lookups.routes').then((m) => m.LOOKUPS_ROUTES)
  },
  {
    path: 'departments',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/departments/departments.routes').then((m) => m.DEPARTMENTS_ROUTES)
  },
  {
    path: 'employees',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/employees/employees.routes').then((m) => m.EMPLOYEES_ROUTES)
  },
  {
    path: 'attendance',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/attendance/attendance.routes').then((m) => m.ATTENDANCE_ROUTES)
  },
  {
    path: 'timesheets',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/timesheets/timesheets.routes').then((m) => m.TIMESHEETS_ROUTES)
  },
  {
    path: 'leave',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager', 'Employee'])],
    loadChildren: () => import('./features/leave/leave.routes').then((m) => m.LEAVE_ROUTES)
  },
  {
    path: 'subscriptions',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/subscriptions/subscriptions.routes').then((m) => m.SUBSCRIPTIONS_ROUTES)
  },
  {
    path: 'plans',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/plans/plans.routes').then((m) => m.PLANS_ROUTES)
  },
  {
    path: 'payroll',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/payroll/payroll.routes').then((m) => m.PAYROLL_ROUTES)
  },
  {
    path: 'performance',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/performance/performance.routes').then((m) => m.PERFORMANCE_ROUTES)
  },
  {
    path: 'recruitment',
    canMatch: [authGuard, roleGuard(['Admin', 'HR'])],
    loadChildren: () => import('./features/recruitment/recruitment.routes').then((m) => m.RECRUITMENT_ROUTES)
  },
  {
    path: 'notifications',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/notifications/notifications.routes').then((m) => m.NOTIFICATIONS_ROUTES)
  },
  {
    path: 'training',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () => import('./features/training/training.routes').then((m) => m.TRAINING_ROUTES)
  },
  {
    path: 'announcements',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager'])],
    loadChildren: () =>
      import('./features/announcements/announcements.routes').then((m) => m.ANNOUNCEMENTS_ROUTES)
  },
  {
    path: 'self-service',
    canMatch: [authGuard, roleGuard(['Admin', 'HR', 'Manager', 'Employee'])],
    loadChildren: () => import('./features/self-service/self-service.routes').then((m) => m.SELF_SERVICE_ROUTES)
  },
  { path: 'forbidden', component: ForbiddenPageComponent },
  { path: '**', component: NotFoundPageComponent }
];
