export type UserStatus = 'activo' | 'pendiente';

export interface UserGroup {
  id: string;
  name: string;
  code: string;
}

export interface UserAccount {
  id: string;
  name: string;
  email: string;
  role: string;
  registeredAtIso: string | null;
  status: UserStatus;
  group: UserGroup | null;
}
