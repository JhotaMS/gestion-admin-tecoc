// Coincide con el contrato real del backend: GET /api/implementos/disponibles
// (gestionAdminTECOCApi.Application.Features.Implementos.GetImplementosDisponibles) — HU153.

export interface ImplementoDisponible {
  id: string;
  nombre: string;
  codigo: string;
  descripcion: string | null;
  cantidadTotal: number;
  cantidadDisponible: number;
  estado: string;
}

export interface ImplementosDisponiblesResponse {
  implementos: ImplementoDisponible[];
  mensaje: string | null;
}
