import { Injectable } from '@angular/core';
import { AuthUser } from '../models/auth.models';

const TOKEN_KEY = 'tecoc.auth.token';
const USER_KEY = 'tecoc.auth.user';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      // Sesión corrupta (ej. de una versión anterior del mock): se descarta
      // en vez de tumbar el arranque de la app.
      localStorage.removeItem(USER_KEY);
      return null;
    }
  }

  setSession(token: string, user: AuthUser): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  clear(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }
}
