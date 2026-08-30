import { Observable } from 'rxjs';
import {
  CreateImplementoPrestadoRequest,
  ImplementoPrestadoDto,
  ImplementoPrestadoResponse,
} from '../models/implemento-prestado.models';

/**
 * Contrato para registrar y consultar prestamos de implemento (endpoint real
 * gestionAdminTECOCApi v1/ImplementosPrestados). No tiene mock: siempre habla con el backend.
 */
export abstract class ImplementoPrestadoApi {
  abstract create(request: CreateImplementoPrestadoRequest): Observable<ImplementoPrestadoResponse>;
  abstract getAll(): Observable<ImplementoPrestadoDto[]>;
}
