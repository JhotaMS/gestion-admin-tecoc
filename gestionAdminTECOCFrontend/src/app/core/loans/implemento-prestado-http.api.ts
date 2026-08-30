import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateImplementoPrestadoRequest,
  ImplementoPrestadoResponse,
} from '../models/implemento-prestado.models';
import { ImplementoPrestadoApi } from './implemento-prestado-api';

interface ApiErrorBody {
  message?: string;
}

@Injectable()
export class ImplementoPrestadoHttpApi extends ImplementoPrestadoApi {
  private readonly http = inject(HttpClient);

  create(request: CreateImplementoPrestadoRequest): Observable<ImplementoPrestadoResponse> {
    return this.http
      .post<ImplementoPrestadoResponse>(`${environment.apiBaseUrl}/api/v1/ImplementosPrestados`, request)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(this.extractMessage(error)))));
  }

  private extractMessage(error: HttpErrorResponse): string {
    const body = error.error as ApiErrorBody | undefined;
    return body?.message || 'No fue posible registrar el préstamo. Intenta nuevamente.';
  }
}
