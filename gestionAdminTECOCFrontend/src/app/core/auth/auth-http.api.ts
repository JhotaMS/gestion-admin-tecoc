import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthUser, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.models';
import { AuthApi } from './auth-api';

interface LoginApiResponse {
  userId: string;
  email: string;
}

@Injectable()
export class AuthHttpApi extends AuthApi {
  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<LoginResponse> {
    const payload = {
      email: request.username,
      password: request.password,
    };

    return this.http
      .post<LoginApiResponse>(`${environment.apiBaseUrl}/v1/auth/login`, payload)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          const msg = error.error?.message || error.message || 'Usuario o contraseña incorrectos';
          return throwError(() => new Error(msg));
        }),
        map((response) => ({
          token: `api-token-${response.userId}`,
          expiresInSeconds: 3600,
          user: {
            id: response.userId,
            name: response.email.split('@')[0],
            email: response.email,
            role: 'user',
          } satisfies AuthUser,
        })),
      );
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return throwError(() => new Error('El registro de autenticación no está habilitado con la API real. Usa el registro de usuarios.'));
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    return throwError(() => new Error(`No se pudo recuperar el usuario actual para el token ${token}.`));
  }
}
