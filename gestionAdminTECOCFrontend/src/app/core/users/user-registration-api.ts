import { Observable } from 'rxjs';
import { CreateUserRequest, CreateUserResponse } from '../models/user-registration.models';

/**
 * Contrato para registrar la identificación de una persona (endpoint real
 * gestionAdminTECOCApi v1/User). A diferencia de AuthApi/UsersApi, este no
 * tiene implementación mock: siempre habla con el backend.
 */
export abstract class UserRegistrationApi {
  abstract createUser(request: CreateUserRequest): Observable<CreateUserResponse>;
}
