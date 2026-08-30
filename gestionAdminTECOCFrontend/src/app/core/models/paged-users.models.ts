export interface PagedUser {
  id: string;
  fullName: string;
  userName: string;
  email: string;
  documentType: string;
  documentNumber: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
