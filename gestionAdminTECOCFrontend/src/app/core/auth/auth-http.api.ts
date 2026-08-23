import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthApi } from './auth-api';
import { AuthUser, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.models';

@Injectable()
export class AuthHttpApi extends AuthApi {
  private readonly httpClient = inject(HttpClient);
  private readonly apiUrl = '/api/v1';

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(`${this.apiUrl}/auth/login`, request);
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.httpClient.post<AuthUser>(`${this.apiUrl}/user/register`, request);
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    return this.httpClient.get<AuthUser>(`${this.apiUrl}/auth/me`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  }
}
