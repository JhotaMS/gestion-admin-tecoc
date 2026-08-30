import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UsersApi } from '../../users/users-api';
import { UserAccount } from '../../users/users.models';

interface UsersResponse {
  users: ApiUser[];
}

interface ApiUser {
  id: string;
  fullName: string;
  userName: string;
  email: string;
  enabled: boolean;
  group: ApiGroup | null;
}

interface ApiGroup {
  id: string;
  name: string;
  code: string;
}

@Injectable()
export class UsersHttpApi extends UsersApi {
  private readonly http = inject(HttpClient);

  getUsers(): Observable<UserAccount[]> {
    return this.http.get<UsersResponse>(`${environment.apiBaseUrl}/User`).pipe(
      map((response) =>
        response.users.map((user) => ({
          id: user.id,
          name: user.fullName,
          email: user.email,
          role: user.userName,
          registeredAtIso: null,
          status: user.enabled ? 'activo' : 'pendiente',
          group: user.group,
        })),
      ),
    );
  }
}
