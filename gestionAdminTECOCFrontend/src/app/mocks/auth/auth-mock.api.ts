import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { AuthApi } from '../../core/auth/auth-api';
import { AuthUser, LoginRequest, LoginResponse } from '../../core/models/auth.models';

interface MockAccount {
  username: string;
  password: string;
  user: AuthUser;
}

const MOCK_ACCOUNTS: MockAccount[] = [
  {
    username: 'admin',
    password: 'admin123',
    user: {
      id: 'usr-001',
      name: 'Administrador TECOC',
      email: 'admin@tecoc.local',
      role: 'admin',
    },
  },
];

@Injectable()
export class AuthMockApi extends AuthApi {
  login(request: LoginRequest): Observable<LoginResponse> {
    const account = MOCK_ACCOUNTS.find(
      (a) => a.username === request.username && a.password === request.password
    );

    if (!account) {
      return throwError(() => new Error('Usuario o contraseña incorrectos')).pipe(delay(500));
    }

    const response: LoginResponse = {
      token: `mock-jwt.${account.user.id}.${Date.now()}`,
      expiresInSeconds: 3600,
      user: account.user,
    };

    return of(response).pipe(delay(600));
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    const account = MOCK_ACCOUNTS.find((a) => token.includes(a.user.id));
    if (!account) {
      return throwError(() => new Error('Sesión inválida'));
    }
    return of(account.user).pipe(delay(200));
  }
}
