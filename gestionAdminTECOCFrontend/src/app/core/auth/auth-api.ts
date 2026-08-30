import { Observable } from 'rxjs';
import { AuthUser, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.models';

/**
 * Contract for authentication data access. Swap the provider in app.config.ts
 * (MockAuthApi -> a real HttpClient-backed implementation) without touching AuthService.
 */
export abstract class AuthApi {
  abstract login(request: LoginRequest): Observable<LoginResponse>;
  abstract register(request: RegisterRequest): Observable<AuthUser>;
  abstract getCurrentUser(token: string): Observable<AuthUser>;
}
