import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./account/auth/signin/signin.component').then((m) => m.SigninComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./account/auth/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: '',
    loadComponent: () => import('./layout/layout.component').then((m) => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'usuarios',
        loadComponent: () => import('./users/users.component').then((m) => m.UsersComponent),
      },
      {
        path: 'prestamos',
        loadComponent: () => import('./loans/loans.component').then((m) => m.LoansComponent),
      },
      {
        path: 'asistencia',
        loadComponent: () =>
          import('./attendance/attendance.component').then((m) => m.AttendanceComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
