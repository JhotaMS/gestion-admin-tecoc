export type UserStatus = 'activo' | 'pendiente';

export interface UserAccount {
  id: string;
  name: string;
  email: string;
  role: string;
  registeredAtIso: string;
  status: UserStatus;
}
