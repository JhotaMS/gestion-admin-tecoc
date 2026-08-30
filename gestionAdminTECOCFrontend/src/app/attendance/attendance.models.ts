export type AttendanceStatus = 'completa' | 'parcial' | 'ausente';

export interface AttendanceStudent {
  id: string;
  fullName: string;
  // Texto legible (ej. "Cédula de ciudadanía"), tal como lo entrega el backend real —
  // no el código corto, para que se muestre igual que en el resto de la app.
  documentType: string;
  documentNumber: string;
  email: string;
}

export interface ClassSession {
  id: string;
  label: string;
  day: string;
  duration: number;
  // Horas asistidas por estudiante, indexadas por su id (no por posición), para que
  // sigan siendo válidas sin importar qué página de estudiantes esté visible.
  hours: Record<string, number>;
}

export interface AttendanceSnapshot {
  groupName: string;
  subjectName: string;
  sessions: ClassSession[];
}

export interface SaveAttendanceRequest {
  sessionId: string;
  duration: number;
  hours: Record<string, number>;
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
