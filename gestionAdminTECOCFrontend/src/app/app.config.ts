import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthApi } from './core/auth/auth-api';
import { AuthMockApi } from './mocks/auth/auth-mock.api';
import { DashboardApi } from './dashboard/dashboard-api';
import { DashboardMockApi } from './mocks/dashboard/dashboard-mock.api';
import { UsersApi } from './users/users-api';
import { UsersMockApi } from './mocks/users/users-mock.api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    // Swap these for real HttpClient-backed implementations once the backend is ready.
    { provide: AuthApi, useClass: AuthMockApi },
    { provide: DashboardApi, useClass: DashboardMockApi },
    { provide: UsersApi, useClass: UsersMockApi },
  ],
};
