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
import { UsersMockApi } from './mocks/users/users-mock.api';
import { UserRegistrationApi } from './core/users/user-registration-api';
import { UserRegistrationHttpApi } from './core/users/user-registration-http.api';
import { environment } from '../environments/environment';

const authProvider = environment.useMockApi
  ? { provide: AuthApi, useClass: AuthMockApi }
  : { provide: AuthApi, useClass: AuthHttpApi };

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    authProvider,
    { provide: DashboardApi, useClass: DashboardMockApi },
    { provide: UsersApi, useClass: UsersMockApi },
    { provide: UserRegistrationApi, useClass: UserRegistrationHttpApi },
  ],
};
