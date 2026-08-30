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

 login(request: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(
      `${environment.apiBaseUrl}/v1/Auth/login`,
      request,
    );
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.httpClient.post<AuthUser>(
      `${environment.apiBaseUrl}/v1/user/register`,
      request,
    );
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    return this.httpClient.get<AuthUser>(`${environment.apiBaseUrl}/v1/auth/me`, {
      headers: { Authorization: `Bearer ${token}` },
    });

  }
}
