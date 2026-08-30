import { DocumentTypeCode } from '../core/models/user-registration.models';

export type AttendanceStatus = 'completa' | 'parcial' | 'ausente';

export interface AttendanceStudent {
  id: string;
  fullName: string;
  documentType: DocumentTypeCode;
  documentNumber: string;
  email: string;
}

export interface ClassSession {
  id: string;
  label: string;
  day: string;
  duration: number;
  hours: number[];
}

export interface AttendanceSnapshot {
  groupName: string;
  subjectName: string;
  students: AttendanceStudent[];
  sessions: ClassSession[];
}

export interface SaveAttendanceRequest {
  sessionId: string;
  duration: number;
  hours: number[];
}

export interface NewSessionRequest {
  label: string;
  day: string;
  duration: number;
}

export interface UpdateSessionRequest {
  sessionId: string;
  label: string;
  day: string;
  duration: number;
}
