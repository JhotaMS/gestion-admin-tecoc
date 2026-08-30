import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ImplementosDisponiblesResponse } from '../models/implemento-disponible.models';
import { ImplementosDisponiblesApi } from './implementos-disponibles-api';

@Injectable()
export class ImplementosDisponiblesHttpApi extends ImplementosDisponiblesApi {
  private readonly http = inject(HttpClient);

  getDisponibles(): Observable<ImplementosDisponiblesResponse> {
    return this.http.get<ImplementosDisponiblesResponse>(
      `${environment.apiBaseUrl}/api/implementos/disponibles`,
    );
  }
}
