import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthApi } from './core/auth/auth-api';
import { AuthHttpApi } from './core/auth/auth-http.api';
import { AuthMockApi } from './mocks/auth/auth-mock.api';
import { DashboardApi } from './dashboard/dashboard-api';
import { DashboardMockApi } from './mocks/dashboard/dashboard-mock.api';
import { UsersApi } from './users/users-api';
import { UsersHttpApi } from './core/users/users-http.api';
import { UserRegistrationApi } from './core/users/user-registration-api';
import { UserRegistrationHttpApi } from './core/users/user-registration-http.api';
import { environment } from '../environments/environment';

const authProvider = environment.useMockApi
  ? { provide: AuthApi, useClass: AuthMockApi }
  : { provide: AuthApi, useClass: AuthHttpApi };
import { GroupsApi } from './groups/groups-api';
import { GroupsHttpApi } from './core/groups/groups-http.api';
import { PagedUsersApi } from './core/users/paged-users-api';
import { PagedUsersHttpApi } from './core/users/paged-users-http.api';
import { LoansApi } from './loans/loans-api';
import { LoansMockApi } from './mocks/loans/loans-mock.api';
import { ImplementosApi } from './core/loans/implementos-api';
import { ImplementosHttpApi } from './core/loans/implementos-http.api';
import { ImplementoPrestadoApi } from './core/loans/implemento-prestado-api';
import { ImplementoPrestadoHttpApi } from './core/loans/implemento-prestado-http.api';
import { PrestamoDetalleApi } from './core/loans/prestamo-detalle-api';
import { PrestamoDetalleHttpApi } from './core/loans/prestamo-detalle-http.api';
import { AttendanceApi } from './attendance/attendance-api';
import { AttendanceMockApi } from './mocks/attendance/attendance-mock.api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    authProvider,
    { provide: DashboardApi, useClass: DashboardMockApi },
    { provide: UsersApi, useClass: UsersMockApi },
    // Swap these for real HttpClient-backed implementations once the backend is ready.
    { provide: AuthApi, useClass: AuthHttpApi },
    { provide: DashboardApi, useClass: DashboardMockApi },
    { provide: UsersApi, useClass: UsersHttpApi },
    { provide: UserRegistrationApi, useClass: UserRegistrationHttpApi },
    { provide: PagedUsersApi, useClass: PagedUsersHttpApi },
    { provide: ImplementosApi, useClass: ImplementosHttpApi },
    { provide: ImplementoPrestadoApi, useClass: ImplementoPrestadoHttpApi },
    { provide: PrestamoDetalleApi, useClass: PrestamoDetalleHttpApi },
    { provide: LoansApi, useClass: LoansMockApi },
    { provide: AttendanceApi, useClass: AttendanceMockApi },
  ],
};
