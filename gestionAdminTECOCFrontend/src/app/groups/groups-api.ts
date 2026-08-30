import { Observable } from 'rxjs';
import { CreateGroupRequest, Group, UpdateGroupRequest } from './groups.models';

export abstract class GroupsApi {
  abstract getGroups(): Observable<Group[]>;
  abstract createGroup(request: CreateGroupRequest): Observable<Group>;
  abstract updateGroup(request: UpdateGroupRequest): Observable<Group>;
  abstract deleteGroup(groupId: string): Observable<void>;
}
