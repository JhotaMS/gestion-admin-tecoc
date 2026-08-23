import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DocumentTypeDto } from '../models/document-type.models';
import { DocumentTypeApi } from './document-type-api';

interface GetAllDocumentTypesResponse {
  documentTypes: DocumentTypeDto[];
}

@Injectable()
export class DocumentTypeHttpApi extends DocumentTypeApi {
  private readonly http = inject(HttpClient);

  getAll(): Observable<DocumentTypeDto[]> {
    return this.http
      .get<GetAllDocumentTypesResponse>(`${environment.apiBaseUrl}/v1/DocumentType`)
      .pipe(map((response) => response.documentTypes));
  }
}
