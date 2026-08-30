import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AttendanceApi } from './attendance-api';
import { AttendanceStatus, AttendanceStudent, ClassSession } from './attendance.models';
import { PagedUsersApi } from '../core/users/paged-users-api';

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
  return sessions.map((session) => ({ ...session, hours: { ...session.hours } }));
}

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './attendance.component.html',
})
export class AttendanceComponent implements OnInit {
  private readonly attendanceApi = inject(AttendanceApi);
  private readonly pagedUsersApi = inject(PagedUsersApi);
  private lastSaved: ClassSession[] = [];

  // Paginación real de estudiantes: al superar este tamaño de página se requiere
  // avanzar a la página 2 para ver el resto.
  readonly pageSize = 8;
  readonly studentsLoading = signal(true);
  readonly currentPage = signal(1);
  readonly totalPages = signal(0);
  readonly totalStudents = signal(0);

  readonly durationOptions = [4, 8];

  // Selección visual de contexto académico (Programa/Semestre/Materia). No filtra datos:
  // el backend real de asistencias todavía no expone estos catálogos.
  readonly programOptions = ['Ingeniería de Sistemas', 'Administración de Empresas', 'Contaduría Pública'];
  readonly semesterOptions = ['1° semestre', '2° semestre', '3° semestre', '4° semestre', '5° semestre', '6° semestre', '7° semestre', '8° semestre'];
  readonly subjectOptions = ['Matemáticas', 'Programación I', 'Bases de Datos', 'Física'];

  readonly selectedProgram = signal(this.programOptions[0]);
  readonly selectedSemester = signal('8° semestre');
  readonly selectedSubject = signal(this.subjectOptions[0]);

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

  readonly newSessionModalOpen = signal(false);
  readonly newSessionLabel = signal('');
  readonly newSessionDay = signal('');
  readonly newSessionDuration = signal(4);
  readonly newSessionSubmitting = signal(false);
  readonly newSessionSubmitted = signal(false);

  readonly confirmingDelete = signal(false);
  readonly deleting = signal(false);

  readonly selectedSession = computed<ClassSession | null>(
    () => this.sessions().find((session) => session.id === this.selectedSessionId()) ?? null,
  );

  readonly rows = computed<StudentRow[]>(() => {
    const session = this.selectedSession();
    if (!session) return [];

    return this.students().map((student) => {
      const hours = session.hours[student.id] ?? 0;
      const status = statusFor(hours, session.duration);
      const bars = Array.from({ length: session.duration }, (_, hour) => ({ filled: hour < hours }));
      return { student, hours, duration: session.duration, status, bars };
    });
  });

  readonly pageNumbers = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

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
      this.sessions.set(snapshot.sessions);
      this.lastSaved = cloneSessions(snapshot.sessions);
      this.selectedSessionId.set(snapshot.sessions.at(-1)?.id ?? null);
      this.loading.set(false);
    });

    this.loadStudentsPage(1);
  }

  private loadStudentsPage(pageNumber: number): void {
    this.studentsLoading.set(true);
    this.pagedUsersApi.getPage(pageNumber, this.pageSize).subscribe({
      next: (page) => {
        this.students.set(
          page.items.map((user) => ({
            id: user.id,
            fullName: user.fullName,
            documentType: user.documentType,
            documentNumber: user.documentNumber,
            email: user.email,
          })),
        );
        this.currentPage.set(page.pageNumber);
        this.totalPages.set(page.totalPages);
        this.totalStudents.set(page.totalCount);
        this.studentsLoading.set(false);
      },
      error: () => {
        this.studentsLoading.set(false);
      },
    });
  }

  goToPage(pageNumber: number): void {
    if (pageNumber < 1 || pageNumber > this.totalPages() || pageNumber === this.currentPage()) return;
    this.loadStudentsPage(pageNumber);
  }

  statusLabel(status: AttendanceStatus): string {
    return STATUS_LABELS[status];
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  sessionOptionLabel(session: ClassSession): string {
    return `${session.label} · ${session.day} · ${session.duration}h`;
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
          ? {
              ...session,
              duration,
              hours: Object.fromEntries(
                Object.entries(session.hours).map(([studentId, hours]) => [studentId, Math.min(hours, duration)]),
              ),
            }
          : session,
      ),
    );
    this.saveNote.set('La jornada cambió y las horas superiores al máximo fueron ajustadas.');
  }

  setHours(studentId: string, value: string): void {
    const sessionId = this.selectedSessionId();
    const session = this.selectedSession();
    if (!sessionId || !session) return;

    const parsed = Number.parseInt(value, 10);
    const clamped = Number.isNaN(parsed) ? 0 : Math.min(Math.max(parsed, 0), session.duration);

    this.sessions.update((sessions) =>
      sessions.map((current) =>
        current.id === sessionId
          ? { ...current, hours: { ...current.hours, [studentId]: clamped } }
          : current,
      ),
    );
    this.saveNote.set(null);
  }

  openAddSessionModal(): void {
    this.newSessionLabel.set('');
    this.newSessionDay.set('');
    this.newSessionDuration.set(4);
    this.newSessionSubmitted.set(false);
    this.newSessionModalOpen.set(true);
  }

  closeAddSessionModal(): void {
    this.newSessionModalOpen.set(false);
  }

  setNewSessionDuration(duration: number): void {
    this.newSessionDuration.set(duration);
  }

  submitNewSession(): void {
    this.newSessionSubmitted.set(true);

    const label = this.newSessionLabel().trim();
    const day = this.newSessionDay().trim();
    if (!label || !day) return;

    this.newSessionSubmitting.set(true);
    this.attendanceApi
      .addSession({ label, day, duration: this.newSessionDuration() })
      .subscribe({
        next: (created) => {
          this.sessions.update((sessions) => [...sessions, created]);
          this.lastSaved = cloneSessions(this.sessions());
          this.selectedSessionId.set(created.id);
          this.newSessionSubmitting.set(false);
          this.newSessionModalOpen.set(false);
          this.saveNote.set(`Fecha "${label} ${day}" creada correctamente.`);
        },
        error: () => {
          this.newSessionSubmitting.set(false);
          this.saveNote.set('No fue posible crear la fecha. Intenta nuevamente.');
        },
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
