import { Observable } from 'rxjs';
import { PrestamoDetalle } from '../models/prestamo-detalle.models';

/**
 * Contrato para consultar el detalle real de un prestamo (endpoint real
 * gestionAdminTECOCApi GET /api/v1/Prestamo/{id}). No tiene mock: siempre habla con el backend.
 */
export abstract class PrestamoDetalleApi {
  abstract getById(id: string): Observable<PrestamoDetalle>;
}
