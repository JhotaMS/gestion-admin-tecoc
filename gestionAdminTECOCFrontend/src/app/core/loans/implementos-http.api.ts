import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ImplementoOption } from '../models/implemento-prestado.models';
import { ImplementosApi } from './implementos-api';

interface ImplementoDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion: string;
}

interface GetAllImplementosResponseDto {
  implementos: ImplementoDto[];
}

@Injectable()
export class ImplementosHttpApi extends ImplementosApi {
  private readonly http = inject(HttpClient);

  getAll(): Observable<ImplementoOption[]> {
    return this.http
      .get<GetAllImplementosResponseDto>(`${environment.apiBaseUrl}/v1/Implementos/disponibles`)
      .pipe(map((response) => response.implementos));
  }
}
