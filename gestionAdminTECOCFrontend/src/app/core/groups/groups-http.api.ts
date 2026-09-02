import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GroupsApi } from '../../groups/groups-api';
import { CreateGroupRequest, Group, UpdateGroupRequest } from '../../groups/groups.models';

interface GroupDto {
  id: string;
  name: string;
  code: string;
  enabled: boolean;
  cupoTotal: number;
  cupoDisponible: number;
}

interface GetAllGroupsResponseDto {
  groups: GroupDto[];
}

interface ApiErrorBody {
  statusCode?: number;
  message?: string;
}

const BASE_URL = `${environment.apiBaseUrl}/v1/Group`;

@Injectable()
export class GroupsHttpApi extends GroupsApi {
  private readonly http = inject(HttpClient);

  getGroups(): Observable<Group[]> {
    return this.http.get<GetAllGroupsResponseDto>(BASE_URL).pipe(
      map((response) => response.groups.map(toGroup)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible cargar los grupos.'))),
      ),
    );
  }

  createGroup(request: CreateGroupRequest): Observable<Group> {
    return this.http.post<GroupDto>(BASE_URL, request).pipe(
      map(toGroup),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible crear el grupo.'))),
      ),
    );
  }

  updateGroup(request: UpdateGroupRequest): Observable<Group> {
    return this.http.put<GroupDto>(`${BASE_URL}/${request.groupId}`, request).pipe(
      map(toGroup),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible actualizar el grupo.'))),
      ),
    );
  }

  deleteGroup(groupId: string): Observable<void> {
    return this.http.delete<void>(`${BASE_URL}/${groupId}`).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(messageFrom(error, 'No fue posible eliminar el grupo.'))),
      ),
    );
  }
}

function toGroup(dto: GroupDto): Group {
  return {
    id: dto.id,
    name: dto.name,
    code: dto.code,
    enabled: dto.enabled,
    cupoTotal: dto.cupoTotal,
    cupoDisponible: dto.cupoDisponible,
  };
}

function messageFrom(error: HttpErrorResponse, fallback: string): string {
  const body = error.error as ApiErrorBody | undefined;
  return body?.message?.trim() || fallback;
}
