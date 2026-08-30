export type UserStatus = 'activo' | 'pendiente';

export interface UserAccount {
  id: string;
  name: string;
  email: string;
  role: string;
  registeredAtIso: string;
  status: UserStatus;
}

export interface UpdateUserAccountRequest {
  id: string;
  name: string;
  email: string;
  role: string;
  status: UserStatus;
}
