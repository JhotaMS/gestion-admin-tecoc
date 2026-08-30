import { Observable } from 'rxjs';
import { PagedResult, PagedUser } from '../models/paged-users.models';

/**
 * Contrato para consultar usuarios paginados (endpoint real gestionAdminTECOCApi
 * v1/User/paged). No tiene mock: siempre habla con el backend.
 */
export abstract class PagedUsersApi {
  abstract getPage(pageNumber: number, pageSize: number): Observable<PagedResult<PagedUser>>;
}
