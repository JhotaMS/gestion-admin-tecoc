import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { AttendanceApi } from '../../attendance/attendance-api';
import {
  AttendanceSnapshot,
  ClassSession,
  NewSessionRequest,
  SaveAttendanceRequest,
  UpdateSessionRequest,
} from '../../attendance/attendance.models';

// Los estudiantes ya no son mock: vienen del listado paginado real de usuarios
// (PagedUsersApi). Las horas por sesión quedan en {} hasta que se registren para
// el id de un estudiante real.
let sessions: ClassSession[] = [
  { id: '2026-08-08', label: 'Viernes', day: '8 ago', duration: 4, hours: {} },
  { id: '2026-08-15', label: 'Viernes', day: '15 ago', duration: 4, hours: {} },
  { id: '2026-08-22', label: 'Viernes', day: '22 ago', duration: 4, hours: {} },
  { id: '2026-08-29', label: 'Jornada Especial', day: '29 ago', duration: 8, hours: {} },
];

@Injectable()
export class AttendanceMockApi extends AttendanceApi {
  getSnapshot(): Observable<AttendanceSnapshot> {
    return of({
      groupName: '8°B',
      subjectName: 'Matemáticas',
      sessions: sessions.map((session) => ({ ...session, hours: { ...session.hours } })),
    }).pipe(delay(300));
  }

  saveAttendance(request: SaveAttendanceRequest): Observable<ClassSession> {
    const target = sessions.find((session) => session.id === request.sessionId);
    if (!target) {
      return throwError(() => new Error('Fecha de clase no encontrada.'));
    }

    const updated: ClassSession = { ...target, duration: request.duration, hours: { ...request.hours } };
    sessions = sessions.map((session) => (session.id === request.sessionId ? updated : session));

    return of(updated).pipe(delay(300));
  }

  addSession(request: NewSessionRequest): Observable<ClassSession> {
    const created: ClassSession = {
      id: `session-${Date.now()}`,
      label: request.label,
      day: request.day,
      duration: request.duration,
      hours: {},
    };

    sessions = [...sessions, created];

    return of(created).pipe(delay(300));
  }

  updateSession(request: UpdateSessionRequest): Observable<ClassSession> {
    const target = sessions.find((session) => session.id === request.sessionId);
    if (!target) {
      return throwError(() => new Error('Fecha de clase no encontrada.'));
    }

    const clampedHours = Object.fromEntries(
      Object.entries(target.hours).map(([studentId, hours]) => [studentId, Math.min(hours, request.duration)]),
    );
    const updated: ClassSession = {
      ...target,
      label: request.label,
      day: request.day,
      duration: request.duration,
      hours: clampedHours,
    };
    sessions = sessions.map((session) => (session.id === request.sessionId ? updated : session));

    return of(updated).pipe(delay(300));
  }

  deleteSession(sessionId: string): Observable<void> {
    sessions = sessions.filter((session) => session.id !== sessionId);
    return of(undefined).pipe(delay(300));
  }
}
