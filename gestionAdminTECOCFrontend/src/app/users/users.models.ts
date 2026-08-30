export interface UserGroup {
  id: string;
  name: string;
  code: string;
}

export interface UserAccount {
  id: string;
  name: string;
  userName: string;
  documentType: string;
  documentNumber: string;
  email: string;
  enabled: boolean;
  group: UserGroup | null;
}

export interface UpdateUserAccountRequest {
  id: string;
  name: string;
  email: string;
  role: string;
  status: UserStatus;
}
