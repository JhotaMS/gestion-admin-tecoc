import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

// Coincide con el enum DocumentType del backend (gestionAdminTECOCApi.Domain.Users.DocumentType).
const DOCUMENT_TYPE_CODES: Record<number, string> = {
  1: 'CC',
  2: 'CE',
  3: 'TI',
  4: 'NIT',
};

interface UserDto {
  id: string;
  fullName: string;
  documentType: number;
  documentNumber: string;
  userName: string;
  email: string;
}

interface GetAllUsersResponseDto {
  users: UserDto[];
}

@Injectable()
export class UsersHttpApi extends UsersApi {
  private readonly http = inject(HttpClient);

  getUsers(): Observable<UserAccount[]> {
    return this.http
      .get<GetAllUsersResponseDto>(`${environment.apiBaseUrl}/api/v1/User`)
      .pipe(map((response) => response.users.map(toUserAccount)));
  }
}

function toUserAccount(dto: UserDto): UserAccount {
  return {
    id: dto.id,
    name: dto.fullName,
    userName: dto.userName,
    documentType: DOCUMENT_TYPE_CODES[dto.documentType] ?? String(dto.documentType),
    documentNumber: dto.documentNumber,
    email: dto.email,
  };
}
