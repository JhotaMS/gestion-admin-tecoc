export interface EventoAcademico {
  id: string;
  titulo: string;
  descripcion: string | null;
  fechaInicio: string;
  fechaFin: string | null;
  enabled: boolean;
}

export interface CreateEventoAcademicoRequest {
  titulo: string;
  descripcion: string | null;
  fechaInicio: string;
  fechaFin: string | null;
}

export interface UpdateEventoAcademicoRequest {
  eventoAcademicoId: string;
  titulo: string;
  descripcion: string | null;
  fechaInicio: string;
  fechaFin: string | null;
}

export const EVENTO_TITULO_MAX_LENGTH = 150;
export const EVENTO_DESCRIPCION_MAX_LENGTH = 500;
