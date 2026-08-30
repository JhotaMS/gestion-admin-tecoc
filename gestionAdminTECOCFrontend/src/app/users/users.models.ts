export type UserStatus = 'activo' | 'pendiente';

export interface UserAccount {
  id: string;
  name: string;
  userName: string;
  documentType: string;
  documentNumber: string;
  email: string;
  role: string;
  registeredAtIso: string;
  status: UserStatus;
}
