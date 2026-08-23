import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateUserRequest, CreateUserResponse } from '../models/user-registration.models';
import { UserRegistrationApi } from './user-registration-api';

interface ApiErrorBody {
  message?: string;
  errors?: Record<string, string[]>;
}

@Injectable()
export class UserRegistrationHttpApi extends UserRegistrationApi {
  private readonly http = inject(HttpClient);

  createUser(request: CreateUserRequest): Observable<CreateUserResponse> {
    return this.http
      .post<CreateUserResponse>(`${environment.apiBaseUrl}/v1/User`, request)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(this.extractMessage(error)))));
  }

  private extractMessage(error: HttpErrorResponse): string {
    // status 0 = la petición nunca llegó a recibir respuesta del servidor
    // (backend caído, proxy sin poder conectar, CORS). Se distingue del caso
    // en que el backend sí respondió pero con un error de negocio/validación.
    if (error.status === 0) {
      return 'No se pudo conectar con el servidor de registro. Verifica que el backend esté corriendo (localhost:5189) e inténtalo de nuevo.';
    }

    const body = error.error as ApiErrorBody | undefined;

    if (body?.errors) {
      const fieldMessages = Object.values(body.errors).flat();
      if (fieldMessages.length) {
        return fieldMessages.join(' ');
      }
    }

    return body?.message || 'No fue posible registrar la identificación. Intenta nuevamente.';
  }
}
