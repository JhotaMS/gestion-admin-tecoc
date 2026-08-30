import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UsersApi } from '../../users/users-api';
import { UpdateUserAccountRequest, UserAccount } from '../../users/users.models';

interface UserDto {
  id: string;
  fullName: string;
  documentType: string;
  documentNumber: string;
  userName: string;
  email: string;
  enabled: boolean;
  group: UserGroupDto | null;
}

interface UserGroupDto {
  id: string;
  name: string;
  code: string;
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

  updateUser(request: UpdateUserAccountRequest): Observable<UserAccount> {
    return this.http
      .put<UserDto>(`${environment.apiBaseUrl}/api/v1/User/${request.id}`, request)
      .pipe(map(toUserAccount));
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
  };
}
