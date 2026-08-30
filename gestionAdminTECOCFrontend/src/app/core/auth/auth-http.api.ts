import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthApi } from './auth-api';
import { AuthUser, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable()
export class AuthHttpApi extends AuthApi {
  private readonly httpClient = inject(HttpClient);

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, request);
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.httpClient.post<AuthUser>(`${environment.apiBaseUrl}/user/register`, request);
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    return this.httpClient.get<AuthUser>(`${environment.apiBaseUrl}/auth/me`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  }
}
