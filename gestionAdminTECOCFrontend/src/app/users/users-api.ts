import { Observable } from 'rxjs';
import { UserAccount } from './users.models';

export abstract class UsersApi {
  abstract getUsers(): Observable<UserAccount[]>;
}
