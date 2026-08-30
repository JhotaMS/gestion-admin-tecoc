import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, PagedUser } from '../models/paged-users.models';
import { PagedUsersApi } from './paged-users-api';

@Injectable()
export class PagedUsersHttpApi extends PagedUsersApi {
  private readonly http = inject(HttpClient);

  getPage(pageNumber: number, pageSize: number): Observable<PagedResult<PagedUser>> {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);

    return this.http.get<PagedResult<PagedUser>>(`${environment.apiBaseUrl}/api/v1/User/paged`, { params });
  }
}
