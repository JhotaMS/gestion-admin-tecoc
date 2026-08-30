import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AttendanceApi } from './attendance-api';
import { AttendanceStatus, AttendanceStudent, ClassSession } from './attendance.models';

interface StudentRow {
  student: AttendanceStudent;
  hours: number;
  duration: number;
  status: AttendanceStatus;
  bars: { filled: boolean }[];
}

const STATUS_LABELS: Record<AttendanceStatus, string> = {
  completa: 'Completa',
  parcial: 'Parcial',
  ausente: 'Ausente',
};

function statusFor(hours: number, duration: number): AttendanceStatus {
  if (hours === 0) return 'ausente';
  if (hours < duration) return 'parcial';
  return 'completa';
}

function cloneSessions(sessions: ClassSession[]): ClassSession[] {
  return sessions.map((session) => ({ ...session, hours: [...session.hours] }));
}

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './attendance.component.html',
})
export class AttendanceComponent implements OnInit {
  private readonly attendanceApi = inject(AttendanceApi);
  private lastSaved: ClassSession[] = [];

  readonly durationOptions = [4, 8];

  readonly loading = signal(true);
  readonly groupName = signal('');
  readonly subjectName = signal('');
  readonly students = signal<AttendanceStudent[]>([]);
  readonly sessions = signal<ClassSession[]>([]);
  readonly selectedSessionId = signal<string | null>(null);
  readonly saveNote = signal<string | null>(null);
  readonly saving = signal(false);

  readonly editModalOpen = signal(false);
  readonly editLabel = signal('');
  readonly editDay = signal('');
  readonly editDuration = signal(4);
  readonly editSubmitting = signal(false);
  readonly editSubmitted = signal(false);

  readonly confirmingDelete = signal(false);
  readonly deleting = signal(false);

  readonly selectedSession = computed<ClassSession | null>(
    () => this.sessions().find((session) => session.id === this.selectedSessionId()) ?? null,
  );

  readonly rows = computed<StudentRow[]>(() => {
    const session = this.selectedSession();
    if (!session) return [];

    return this.students().map((student, index) => {
      const hours = session.hours[index] ?? 0;
      const status = statusFor(hours, session.duration);
      const bars = Array.from({ length: session.duration }, (_, hour) => ({ filled: hour < hours }));
      return { student, hours, duration: session.duration, status, bars };
    });
  });

  readonly summary = computed(() => {
    const rows = this.rows();
    return {
      completa: rows.filter((row) => row.status === 'completa').length,
      parcial: rows.filter((row) => row.status === 'parcial').length,
      ausente: rows.filter((row) => row.status === 'ausente').length,
      totalHours: rows.reduce((sum, row) => sum + row.hours, 0),
    };
  });

  ngOnInit(): void {
    this.attendanceApi.getSnapshot().subscribe((snapshot) => {
      this.groupName.set(snapshot.groupName);
      this.subjectName.set(snapshot.subjectName);
      this.students.set(snapshot.students);
      this.sessions.set(snapshot.sessions);
      this.lastSaved = cloneSessions(snapshot.sessions);
      this.selectedSessionId.set(snapshot.sessions.at(-1)?.id ?? null);
      this.loading.set(false);
    });
  }

  statusLabel(status: AttendanceStatus): string {
    return STATUS_LABELS[status];
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  selectSession(sessionId: string): void {
    this.selectedSessionId.set(sessionId);
    this.saveNote.set(null);
    this.confirmingDelete.set(false);
  }

  setDuration(duration: number): void {
    const sessionId = this.selectedSessionId();
    if (!sessionId) return;

    this.sessions.update((sessions) =>
      sessions.map((session) =>
        session.id === sessionId
          ? { ...session, duration, hours: session.hours.map((hours) => Math.min(hours, duration)) }
          : session,
      ),
    );
    this.saveNote.set('La jornada cambió y las horas superiores al máximo fueron ajustadas.');
  }

  setHours(index: number, value: string): void {
    const sessionId = this.selectedSessionId();
    const session = this.selectedSession();
    if (!sessionId || !session) return;

    const parsed = Number.parseInt(value, 10);
    const clamped = Number.isNaN(parsed) ? 0 : Math.min(Math.max(parsed, 0), session.duration);

    this.sessions.update((sessions) =>
      sessions.map((current) =>
        current.id === sessionId
          ? { ...current, hours: current.hours.map((hours, i) => (i === index ? clamped : hours)) }
          : current,
      ),
    );
    this.saveNote.set(null);
  }

  markComplete(index: number): void {
    const session = this.selectedSession();
    if (session) this.setHours(index, String(session.duration));
  }

  markAbsent(index: number): void {
    this.setHours(index, '0');
  }

  addSession(): void {
    const nextNumber = this.sessions().length + 1;
    this.attendanceApi
      .addSession({ label: 'Nueva', day: `Fecha ${nextNumber}`, duration: 4 })
      .subscribe((created) => {
        this.sessions.update((sessions) => [...sessions, created]);
        this.lastSaved = cloneSessions(this.sessions());
        this.selectedSessionId.set(created.id);
        this.saveNote.set('Nueva fecha creada con jornada de 4 horas.');
      });
  }

  save(): void {
    const session = this.selectedSession();
    if (!session) return;

    this.saving.set(true);
    this.attendanceApi
      .saveAttendance({ sessionId: session.id, duration: session.duration, hours: session.hours })
      .subscribe({
        next: () => {
          this.lastSaved = cloneSessions(this.sessions());
          this.saving.set(false);
          this.saveNote.set('Asistencia guardada correctamente.');
        },
        error: () => {
          this.saving.set(false);
          this.saveNote.set('No fue posible guardar la asistencia. Intenta nuevamente.');
        },
      });
  }

  undo(): void {
    this.sessions.set(cloneSessions(this.lastSaved));
    this.saveNote.set('Cambios restaurados al último guardado.');
  }

  openEditModal(): void {
    const session = this.selectedSession();
    if (!session) return;

    this.editLabel.set(session.label);
    this.editDay.set(session.day);
    this.editDuration.set(session.duration);
    this.editSubmitted.set(false);
    this.editModalOpen.set(true);
  }

  closeEditModal(): void {
    this.editModalOpen.set(false);
  }

  setEditDuration(duration: number): void {
    this.editDuration.set(duration);
  }

  saveEdit(): void {
    this.editSubmitted.set(true);

    const session = this.selectedSession();
    const label = this.editLabel().trim();
    const day = this.editDay().trim();
    if (!session || !label || !day) return;

    this.editSubmitting.set(true);
    this.attendanceApi
      .updateSession({ sessionId: session.id, label, day, duration: this.editDuration() })
      .subscribe({
        next: (updated) => {
          this.sessions.update((sessions) =>
            sessions.map((current) => (current.id === updated.id ? updated : current)),
          );
          this.lastSaved = cloneSessions(this.sessions());
          this.editSubmitting.set(false);
          this.editModalOpen.set(false);
          this.saveNote.set('Fecha de clase actualizada.');
        },
        error: () => {
          this.editSubmitting.set(false);
          this.saveNote.set('No fue posible actualizar la fecha. Intenta nuevamente.');
        },
      });
  }

  requestDelete(): void {
    this.confirmingDelete.set(true);
  }

  cancelDelete(): void {
    this.confirmingDelete.set(false);
  }

  confirmDelete(): void {
    const session = this.selectedSession();
    if (!session || this.sessions().length <= 1) return;

    this.deleting.set(true);
    this.attendanceApi.deleteSession(session.id).subscribe({
      next: () => {
        const remaining = this.sessions().filter((current) => current.id !== session.id);
        this.sessions.set(remaining);
        this.lastSaved = cloneSessions(remaining);
        this.selectedSessionId.set(remaining.at(-1)?.id ?? null);
        this.deleting.set(false);
        this.confirmingDelete.set(false);
        this.saveNote.set('Fecha de clase eliminada.');
      },
      error: () => {
        this.deleting.set(false);
        this.saveNote.set('No fue posible eliminar la fecha. Intenta nuevamente.');
      },
    });
  }
}
