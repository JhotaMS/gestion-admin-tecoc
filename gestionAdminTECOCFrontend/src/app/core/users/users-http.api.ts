import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UsersApi } from '../../users/users-api';
import { UpdateUserAccountRequest, UserAccount, UserGroup, UserProgramaAcademico } from '../../users/users.models';

// Coincide con los codigos que valida DocumentTypeCodes en el backend (gestionAdminTECOCApi.Domain.Users).
// GetAllUsers devuelve el texto descriptivo para mostrarlo tal cual en pantalla, pero el endpoint de
// edicion (UpdateUserCommand) exige el codigo corto, por eso se necesita ir y volver entre ambos aqui.
const DOCUMENT_TYPE_DESCRIPTIONS_BY_CODE: Record<string, string> = {
  CC: 'Cédula de ciudadanía',
  CE: 'Cédula de extranjería',
  TI: 'Tarjeta de identidad',
  NIT: 'Número de identificación tributaria',
};

const DOCUMENT_TYPE_CODES_BY_DESCRIPTION: Record<string, string> = Object.fromEntries(
  Object.entries(DOCUMENT_TYPE_DESCRIPTIONS_BY_CODE).map(([code, description]) => [description, code]),
);

interface UserDto {
  id: string;
  fullName: string;
  documentType: string;
  documentNumber: string;
  userName: string;
  email: string;
  enabled: boolean;
  group: UserGroup | null;
  programaAcademico: UserProgramaAcademico | null;
}

interface GetAllUsersResponseDto {
  users: UserDto[];
}

interface UpdateUserResponseDto {
  id: string;
  fullName: string;
  documentType: string;
  documentNumber: string;
  userName: string;
  email: string;
}

@Injectable()
export class UsersHttpApi extends UsersApi {
  private readonly http = inject(HttpClient);

  getUsers(): Observable<UserAccount[]> {
    return this.http
      .get<GetAllUsersResponseDto>(`${environment.apiBaseUrl}/v1/User`)
      .pipe(map((response) => response.users.map(toUserAccount)));
  }

  updateUser(request: UpdateUserAccountRequest): Observable<UserAccount> {
    // El formulario de edicion solo captura nombre, correo, grupo y programa academico; el resto
    // de campos que exige el endpoint real (tipo/numero de documento, nombre de usuario) se toman
    // del registro ya cargado, ya que no hay un GET por id disponible todavia.
    return this.getUsers().pipe(
      switchMap((users) => {
        const current = users.find((user) => user.id === request.id);
        const documentTypeCode = current
          ? (DOCUMENT_TYPE_CODES_BY_DESCRIPTION[current.documentType] ?? current.documentType)
          : '';

        return this.http
          .put<UpdateUserResponseDto>(`${environment.apiBaseUrl}/v1/User/${request.id}`, {
            id: request.id,
            fullName: request.name,
            documentType: documentTypeCode,
            documentNumber: current?.documentNumber ?? '',
            userName: current?.userName ?? '',
            email: request.email,
            groupId: request.group?.id ?? null,
            programaAcademicoId: request.programaAcademico?.id ?? null,
          })
          .pipe(
            map((dto) => ({
              id: dto.id,
              name: dto.fullName,
              userName: dto.userName,
              documentType: DOCUMENT_TYPE_DESCRIPTIONS_BY_CODE[dto.documentType] ?? dto.documentType,
              documentNumber: dto.documentNumber,
              email: dto.email,
              enabled: current?.enabled ?? true,
              group: request.group,
              programaAcademico: request.programaAcademico,
            })),
          );
      }),
    );
  }
}

function toUserAccount(dto: UserDto): UserAccount {
  return {
    id: dto.id,
    name: dto.fullName,
    userName: dto.userName,
    documentType: dto.documentType,
    documentNumber: dto.documentNumber,
    email: dto.email,
    enabled: dto.enabled,
    group: dto.group,
    programaAcademico: dto.programaAcademico,
  };
}
