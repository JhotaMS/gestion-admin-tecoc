import { Observable } from 'rxjs';
import { UpdateUserAccountRequest, UserAccount } from './users.models';

export abstract class UsersApi {
  abstract getUsers(): Observable<UserAccount[]>;
  abstract updateUser(request: UpdateUserAccountRequest): Observable<UserAccount>;
}
