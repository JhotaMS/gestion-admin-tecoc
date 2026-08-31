import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProgramasAcademicosApi } from '../../programas-academicos/programas-academicos-api';
import { ProgramaAcademico } from '../../programas-academicos/programas-academicos.models';

interface ProgramaAcademicoDto {
  id: string;
  name: string;
  code: string;
  enabled: boolean;
}

interface GetAllProgramasAcademicosResponseDto {
  programasAcademicos: ProgramaAcademicoDto[];
}

interface ApiErrorBody {
  statusCode?: number;
  message?: string;
}

const BASE_URL = `${environment.apiBaseUrl}/v1/ProgramaAcademico`;

@Injectable()
export class ProgramasAcademicosHttpApi extends ProgramasAcademicosApi {
  private readonly http = inject(HttpClient);

  getProgramasAcademicos(): Observable<ProgramaAcademico[]> {
    return this.http.get<GetAllProgramasAcademicosResponseDto>(BASE_URL).pipe(
      map((response) => response.programasAcademicos.map(toProgramaAcademico)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible cargar los programas académicos.'))),
      ),
    );
  }
}

function toProgramaAcademico(dto: ProgramaAcademicoDto): ProgramaAcademico {
  return {
    id: dto.id,
    name: dto.name,
    code: dto.code,
    enabled: dto.enabled,
  };
}

function messageFrom(error: HttpErrorResponse, fallback: string): string {
  const body = error.error as ApiErrorBody | undefined;
  return body?.message?.trim() || fallback;
}
