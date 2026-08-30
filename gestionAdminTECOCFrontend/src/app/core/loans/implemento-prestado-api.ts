import { Observable } from 'rxjs';
import {
  CreateImplementoPrestadoRequest,
  ImplementoPrestadoResponse,
} from '../models/implemento-prestado.models';

/**
 * Contrato para registrar un prestamo de implemento (endpoint real
 * gestionAdminTECOCApi v1/ImplementosPrestados). No tiene mock: siempre habla con el backend.
 */
export abstract class ImplementoPrestadoApi {
  abstract create(request: CreateImplementoPrestadoRequest): Observable<ImplementoPrestadoResponse>;
}
