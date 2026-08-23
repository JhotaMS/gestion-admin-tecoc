// El tipo de documento ahora se obtiene en vivo desde v1/DocumentType (ver
// core/document-types/), así que aquí ya no es una unión fija de literales.
export interface CreateUserRequest {
  fullName: string;
  documentType: string;
  documentNumber: string;
  userName: string;
  email: string;
  password: string;
}

export interface CreateUserResponse {
  id: string;
  fullName: string;
  documentType: string;
  documentNumber: string;
  userName: string;
  email: string;
}
