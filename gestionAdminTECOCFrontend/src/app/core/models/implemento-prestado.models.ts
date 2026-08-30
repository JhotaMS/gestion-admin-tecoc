export interface ImplementoOption {
  id: string;
  codigo: string;
  nombre: string;
  descripcion: string;
}

// Coincide con el enum EstadoTipoImplemento del backend (gestionAdminTECOCApi.Domain.Loans).
export const ESTADO_TIPO_BUENO = 3;

// Coincide con los datos sembrados de TiposRevision (1: Inicio Prestamo, 2: Fin Prestamo).
export const TIPO_REVISION_INICIO = 1;

export interface CreateImplementoPrestadoRequest {
  userId: string;
  implementoId: string;
  tipoRevisionId: number;
  estadoTipo: number;
  fechaInicio: string;
  fechaFin: string;
  observacion?: string;
}

export interface ImplementoPrestadoResponse {
  id: string;
  userId: string;
  implementoId: string;
  tipoRevisionId: number;
  estadoTipo: string;
  fechaInicio: string;
  fechaFin: string;
  observacion?: string;
}

// Coincide con ImplementoPrestadoDto (GetAllImplementosPrestadosQuery), ya enriquecido en el
// backend con el nombre/código del implemento y el nombre del solicitante.
export interface ImplementoPrestadoDto {
  id: string;
  userId: string;
  requesterName: string;
  implementoId: string;
  itemName: string;
  itemCode: string;
  tipoRevisionId: number;
  estadoTipo: string;
  fechaInicio: string;
  fechaFin: string;
  observacion?: string;
}
