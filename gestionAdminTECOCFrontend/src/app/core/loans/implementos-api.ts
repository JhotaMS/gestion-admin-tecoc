import { Observable } from 'rxjs';
import { ImplementoOption } from '../models/implemento-prestado.models';

/**
 * Contrato para consultar el catalogo real de implementos (endpoint real
 * gestionAdminTECOCApi v1/Implementos). No tiene mock: siempre habla con el backend.
 */
export abstract class ImplementosApi {
  abstract getAll(): Observable<ImplementoOption[]>;
}
