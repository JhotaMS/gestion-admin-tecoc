import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CalendarioAcademicoApi } from '../../calendario-academico/calendario-academico-api';
import {
  CreateEventoAcademicoRequest,
  EventoAcademico,
  UpdateEventoAcademicoRequest,
} from '../../calendario-academico/calendario-academico.models';

interface EventoAcademicoDto {
  id: string;
  titulo: string;
  descripcion: string | null;
  fechaInicio: string;
  fechaFin: string | null;
  enabled: boolean;
}

interface GetAllEventosAcademicosResponseDto {
  eventos: EventoAcademicoDto[];
}

interface ApiErrorBody {
  statusCode?: number;
  message?: string;
}

const BASE_URL = `${environment.apiBaseUrl}/v1/CalendarioAcademico`;

@Injectable()
export class CalendarioAcademicoHttpApi extends CalendarioAcademicoApi {
  private readonly http = inject(HttpClient);

  getEventos(): Observable<EventoAcademico[]> {
    return this.http.get<GetAllEventosAcademicosResponseDto>(BASE_URL).pipe(
      map((response) => response.eventos.map(toEventoAcademico)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible cargar el calendario académico.'))),
      ),
    );
  }

  createEvento(request: CreateEventoAcademicoRequest): Observable<EventoAcademico> {
    return this.http.post<EventoAcademicoDto>(BASE_URL, request).pipe(
      map(toEventoAcademico),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible crear el evento.'))),
      ),
    );
  }

  updateEvento(request: UpdateEventoAcademicoRequest): Observable<EventoAcademico> {
    return this.http.put<EventoAcademicoDto>(`${BASE_URL}/${request.eventoAcademicoId}`, request).pipe(
      map(toEventoAcademico),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible actualizar el evento.'))),
      ),
    );
  }

  deleteEvento(eventoAcademicoId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${eventoAcademicoId}`).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible eliminar el evento.'))),
      ),
    );
  }
}

function toEventoAcademico(dto: EventoAcademicoDto): EventoAcademico {
  return {
    id: dto.id,
    titulo: dto.titulo,
    descripcion: dto.descripcion,
    fechaInicio: dto.fechaInicio,
    fechaFin: dto.fechaFin,
    enabled: dto.enabled,
  };
}

function messageFrom(error: HttpErrorResponse, fallback: string): string {
  const body = error.error as ApiErrorBody | undefined;
  return body?.message?.trim() || fallback;
}
