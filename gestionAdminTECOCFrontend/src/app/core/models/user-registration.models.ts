export type DocumentTypeCode = 'CC' | 'CE' | 'TI' | 'NIT';

export interface DocumentTypeOption {
  code: DocumentTypeCode;
  label: string;
}

// Debe coincidir con gestionAdminTECOCApi.Domain.Users.DocumentTypeCodes (backend).
export const DOCUMENT_TYPE_OPTIONS: DocumentTypeOption[] = [
  { code: 'CC', label: 'Cédula de ciudadanía' },
  { code: 'CE', label: 'Cédula de extranjería' },
  { code: 'TI', label: 'Tarjeta de identidad' },
  { code: 'NIT', label: 'NIT' },
];

export interface CreateUserRequest {
  fullName: string;
  documentType: DocumentTypeCode | '';
  documentNumber: string;
  userName: string;
  email: string;
  password: string;
}

export interface CreateUserResponse {
  id: string;
  fullName: string;
  documentType: DocumentTypeCode;
  documentNumber: string;
  userName: string;
  email: string;
}
