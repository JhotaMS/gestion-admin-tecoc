import { Observable } from 'rxjs';
import { DocumentTypeDto } from '../models/document-type.models';

/**
 * Contrato para obtener los tipos de documento configurados (endpoint real
 * gestionAdminTECOCApi v1/DocumentType, mantenido desde el CRUD de HU146).
 * No tiene mock: siempre habla con el backend.
 */
export abstract class DocumentTypeApi {
  abstract getAll(): Observable<DocumentTypeDto[]>;
}
