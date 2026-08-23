import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { AuthApi } from '../../core/auth/auth-api';
import { AuthUser, LoginRequest, LoginResponse, RegisterRequest } from '../../core/models/auth.models';

interface MockAccount {
  username: string;
  password: string;
  user: AuthUser;
}

const SEED_ACCOUNTS: MockAccount[] = [
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

// Cuentas creadas desde el formulario de registro. Se guardan en localStorage
// para que sobrevivan a recargas de página mientras no exista un backend real.
const REGISTERED_ACCOUNTS_KEY = 'tecoc.mockRegisteredAccounts';

function loadRegisteredAccounts(): MockAccount[] {
  try {
    const raw = localStorage.getItem(REGISTERED_ACCOUNTS_KEY);
    return raw ? (JSON.parse(raw) as MockAccount[]) : [];
  } catch {
    return [];
  }
}

function saveRegisteredAccounts(accounts: MockAccount[]): void {
  try {
    localStorage.setItem(REGISTERED_ACCOUNTS_KEY, JSON.stringify(accounts));
  } catch {
    // Almacenamiento no disponible (ej. navegación privada): la cuenta sigue
    // funcionando en memoria durante el resto de la sesión.
  }
}

@Injectable()
export class AuthMockApi extends AuthApi {
  private readonly registeredAccounts: MockAccount[] = loadRegisteredAccounts();

  private get accounts(): MockAccount[] {
    return [...SEED_ACCOUNTS, ...this.registeredAccounts];
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    const account = this.accounts.find(
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

  register(request: RegisterRequest): Observable<AuthUser> {
    if (this.accounts.some((a) => a.username === request.username)) {
      return throwError(() => new Error('Ese nombre de usuario ya está en uso.')).pipe(delay(500));
    }

    if (this.accounts.some((a) => a.user.email === request.email)) {
      return throwError(() => new Error('Ese correo ya está registrado.')).pipe(delay(500));
    }

    const user: AuthUser = {
      id: `usr-${Date.now()}`,
      name: request.fullName,
      email: request.email,
      role: 'user',
    };

    this.registeredAccounts.push({ username: request.username, password: request.password, user });
    saveRegisteredAccounts(this.registeredAccounts);

    return of(user).pipe(delay(700));
  }

  getCurrentUser(token: string): Observable<AuthUser> {
    const account = this.accounts.find((a) => token.includes(a.user.id));
    if (!account) {
      return throwError(() => new Error('Sesión inválida'));
    }
    return of(account.user).pipe(delay(200));
  }
}
