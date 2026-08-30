import { Observable } from 'rxjs';
import {
  AttendanceSnapshot,
  ClassSession,
  NewSessionRequest,
  SaveAttendanceRequest,
  UpdateSessionRequest,
} from './attendance.models';

export abstract class AttendanceApi {
  abstract getSnapshot(): Observable<AttendanceSnapshot>;
  abstract saveAttendance(request: SaveAttendanceRequest): Observable<ClassSession>;
  abstract addSession(request: NewSessionRequest): Observable<ClassSession>;
  abstract updateSession(request: UpdateSessionRequest): Observable<ClassSession>;
  abstract deleteSession(sessionId: string): Observable<void>;
}
