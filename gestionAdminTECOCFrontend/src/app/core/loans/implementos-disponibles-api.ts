import { Observable } from 'rxjs';
import { ImplementosDisponiblesResponse } from '../models/implemento-disponible.models';

/**
 * Contrato para consultar los implementos disponibles para préstamo
 * (endpoint real gestionAdminTECOCApi GET /api/implementos/disponibles — HU153).
 * No tiene mock: siempre habla con el backend.
 */
export abstract class ImplementosDisponiblesApi {
  abstract getDisponibles(): Observable<ImplementosDisponiblesResponse>;
}
