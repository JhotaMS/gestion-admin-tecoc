import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { AttendanceApi } from '../../attendance/attendance-api';
import {
  AttendanceSnapshot,
  AttendanceStudent,
  ClassSession,
  NewSessionRequest,
  SaveAttendanceRequest,
  UpdateSessionRequest,
} from '../../attendance/attendance.models';

const students: AttendanceStudent[] = [
  { id: 'E-014', name: 'Ana Sofía Rojas' },
  { id: 'E-015', name: 'Camilo Peña' },
  { id: 'E-016', name: 'Daniela Cárdenas' },
  { id: 'E-017', name: 'Emilio Suárez' },
  { id: 'E-018', name: 'Felipa Torres' },
  { id: 'E-019', name: 'Gabriel Niño' },
];

let sessions: ClassSession[] = [
  { id: '2026-08-08', label: 'Viernes', day: '8 ago', duration: 4, hours: [4, 4, 3, 2, 0, 4] },
  { id: '2026-08-15', label: 'Viernes', day: '15 ago', duration: 4, hours: [4, 1, 4, 4, 3, 0] },
  { id: '2026-08-22', label: 'Viernes', day: '22 ago', duration: 4, hours: [4, 4, 4, 3, 0, 4] },
  { id: '2026-08-29', label: 'Jornada Especial', day: '29 ago', duration: 8, hours: [0, 8, 8, 6, 4, 8] },
];

@Injectable()
export class AttendanceMockApi extends AttendanceApi {
  getSnapshot(): Observable<AttendanceSnapshot> {
    return of({
      groupName: '8°B',
      subjectName: 'Matemáticas',
      students: [...students],
      sessions: sessions.map((session) => ({ ...session, hours: [...session.hours] })),
    }).pipe(delay(300));
  }

  saveAttendance(request: SaveAttendanceRequest): Observable<ClassSession> {
    const target = sessions.find((session) => session.id === request.sessionId);
    if (!target) {
      return throwError(() => new Error('Fecha de clase no encontrada.'));
    }

    const updated: ClassSession = { ...target, duration: request.duration, hours: [...request.hours] };
    sessions = sessions.map((session) => (session.id === request.sessionId ? updated : session));

    return of(updated).pipe(delay(300));
  }

  addSession(request: NewSessionRequest): Observable<ClassSession> {
    const created: ClassSession = {
      id: `session-${Date.now()}`,
      label: request.label,
      day: request.day,
      duration: request.duration,
      hours: students.map(() => 0),
    };

    sessions = [...sessions, created];

    return of(created).pipe(delay(300));
  }

  updateSession(request: UpdateSessionRequest): Observable<ClassSession> {
    const target = sessions.find((session) => session.id === request.sessionId);
    if (!target) {
      return throwError(() => new Error('Fecha de clase no encontrada.'));
    }

    const updated: ClassSession = {
      ...target,
      label: request.label,
      day: request.day,
      duration: request.duration,
      hours: target.hours.map((hours) => Math.min(hours, request.duration)),
    };
    sessions = sessions.map((session) => (session.id === request.sessionId ? updated : session));

    return of(updated).pipe(delay(300));
  }

  deleteSession(sessionId: string): Observable<void> {
    sessions = sessions.filter((session) => session.id !== sessionId);
    return of(undefined).pipe(delay(300));
  }
}
