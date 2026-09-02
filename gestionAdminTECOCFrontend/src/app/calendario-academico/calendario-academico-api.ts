import { Observable } from 'rxjs';
import {
  CreateEventoAcademicoRequest,
  EventoAcademico,
  UpdateEventoAcademicoRequest,
} from './calendario-academico.models';

export abstract class CalendarioAcademicoApi {
  abstract getEventos(): Observable<EventoAcademico[]>;
  abstract createEvento(request: CreateEventoAcademicoRequest): Observable<EventoAcademico>;
  abstract updateEvento(request: UpdateEventoAcademicoRequest): Observable<EventoAcademico>;
  abstract deleteEvento(eventoAcademicoId: string): Observable<void>;
}
