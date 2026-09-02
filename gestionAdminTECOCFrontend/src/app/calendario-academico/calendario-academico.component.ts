import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CalendarioAcademicoApi } from './calendario-academico-api';
import {
  EVENTO_DESCRIPCION_MAX_LENGTH,
  EVENTO_TITULO_MAX_LENGTH,
  EventoAcademico,
} from './calendario-academico.models';

type Feedback = { text: string; tone: 'success' | 'error' };

@Component({
  selector: 'app-calendario-academico',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './calendario-academico.component.html',
})
export class CalendarioAcademicoComponent implements OnInit {
  private readonly calendarioApi = inject(CalendarioAcademicoApi);

  readonly tituloMaxLength = EVENTO_TITULO_MAX_LENGTH;
  readonly descripcionMaxLength = EVENTO_DESCRIPCION_MAX_LENGTH;

  readonly loading = signal(true);
  readonly eventos = signal<EventoAcademico[]>([]);
  readonly feedback = signal<Feedback | null>(null);

  readonly formOpen = signal(false);
  readonly editingEvento = signal<EventoAcademico | null>(null);
  readonly formTitulo = signal('');
  readonly formDescripcion = signal('');
  readonly formFechaInicio = signal('');
  readonly formFechaFin = signal('');
  readonly formSubmitted = signal(false);
  readonly saving = signal(false);

  readonly deletingEvento = signal<EventoAcademico | null>(null);
  readonly deleting = signal(false);

  readonly totalEventos = computed(() => this.eventos().length);

  readonly tituloError = computed(() => {
    const titulo = this.formTitulo().trim();
    if (!titulo) return 'El título del evento es obligatorio.';
    if (titulo.length > this.tituloMaxLength) {
      return `El título no puede superar los ${this.tituloMaxLength} caracteres.`;
    }
    return null;
  });

  readonly fechaInicioError = computed(() => {
    if (!this.formFechaInicio()) return 'La fecha de inicio es obligatoria.';
    return null;
  });

  readonly fechaFinError = computed(() => {
    const inicio = this.formFechaInicio();
    const fin = this.formFechaFin();
    if (inicio && fin && fin < inicio) {
      return 'La fecha de fin no puede ser anterior a la fecha de inicio.';
    }
    return null;
  });

  readonly formValid = computed(
    () => !this.tituloError() && !this.fechaInicioError() && !this.fechaFinError(),
  );

  ngOnInit(): void {
    this.loadEventos();
  }

  loadEventos(): void {
    this.loading.set(true);
    this.calendarioApi.getEventos().subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.loading.set(false);
      },
      error: (error: Error) => {
        this.loading.set(false);
        this.notify(error.message, 'error');
      },
    });
  }

  openCreate(): void {
    this.editingEvento.set(null);
    this.formTitulo.set('');
    this.formDescripcion.set('');
    this.formFechaInicio.set('');
    this.formFechaFin.set('');
    this.formSubmitted.set(false);
    this.formOpen.set(true);
  }

  openEdit(evento: EventoAcademico): void {
    this.editingEvento.set(evento);
    this.formTitulo.set(evento.titulo);
    this.formDescripcion.set(evento.descripcion ?? '');
    this.formFechaInicio.set(evento.fechaInicio);
    this.formFechaFin.set(evento.fechaFin ?? '');
    this.formSubmitted.set(false);
    this.formOpen.set(true);
  }

  closeForm(): void {
    this.formOpen.set(false);
  }

  saveEvento(): void {
    this.formSubmitted.set(true);
    if (!this.formValid()) return;

    const titulo = this.formTitulo().trim();
    const descripcion = this.formDescripcion().trim() || null;
    const fechaInicio = this.formFechaInicio();
    const fechaFin = this.formFechaFin() || null;
    const editing = this.editingEvento();

    this.saving.set(true);

    const request$ = editing
      ? this.calendarioApi.updateEvento({
          eventoAcademicoId: editing.id,
          titulo,
          descripcion,
          fechaInicio,
          fechaFin,
        })
      : this.calendarioApi.createEvento({ titulo, descripcion, fechaInicio, fechaFin });

    request$.subscribe({
      next: (evento) => {
        this.eventos.update((list) =>
          editing ? list.map((item) => (item.id === evento.id ? evento : item)) : [...list, evento],
        );
        this.saving.set(false);
        this.formOpen.set(false);
        this.notify(editing ? 'Evento actualizado correctamente.' : 'Evento creado correctamente.', 'success');
      },
      error: (error: Error) => {
        this.saving.set(false);
        this.notify(error.message, 'error');
      },
    });
  }

  askDelete(evento: EventoAcademico): void {
    this.deletingEvento.set(evento);
  }

  cancelDelete(): void {
    this.deletingEvento.set(null);
  }

  confirmDelete(): void {
    const evento = this.deletingEvento();
    if (!evento) return;

    this.deleting.set(true);
    this.calendarioApi.deleteEvento(evento.id).subscribe({
      next: () => {
        this.eventos.update((list) => list.filter((item) => item.id !== evento.id));
        this.deleting.set(false);
        this.deletingEvento.set(null);
        this.notify('Evento eliminado correctamente.', 'success');
      },
      error: (error: Error) => {
        this.deleting.set(false);
        this.deletingEvento.set(null);
        this.notify(error.message, 'error');
      },
    });
  }

  private notify(text: string, tone: Feedback['tone']): void {
    this.feedback.set({ text, tone });
    setTimeout(() => this.feedback.set(null), 4000);
  }
}
