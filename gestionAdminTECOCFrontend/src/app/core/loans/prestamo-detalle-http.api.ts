import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PrestamoDetalle } from '../models/prestamo-detalle.models';
import { PrestamoDetalleApi } from './prestamo-detalle-api';

@Injectable()
export class PrestamoDetalleHttpApi extends PrestamoDetalleApi {
  private readonly http = inject(HttpClient);

  getById(id: string): Observable<PrestamoDetalle> {
    return this.http.get<PrestamoDetalle>(`${environment.apiBaseUrl}/api/v1/Prestamo/${id}`);
  }
}
