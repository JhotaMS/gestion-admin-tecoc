export type LoanStatus = 'reservado' | 'entregado' | 'devuelto' | 'atrasado';
export type ItemCondition = 'malo' | 'regular' | 'bueno' | 'danado' | 'pend';
export type ReviewMoment = 'inicio' | 'fin' | 'entrega' | 'devolucion';
export type RequesterRole = 'Estudiante' | 'Docente';

export interface TeacherItem {
  id: string;
  name: string;
}

export interface LoanRequest {
  id: string;
  itemName: string;
  itemCode: string;
  requesterName: string;
  requesterRole: string;
  status: LoanStatus;
  scheduleLabel: string;
  dueInDays: number | null;
  condition: ItemCondition;
  startDate?: string;
  endDate?: string;
}

export interface StockItem {
  name: string;
  available: number;
  total: number;
}

export interface CatalogItem {
  name: string;
  code: string;
}

export interface LoansSnapshot {
  loans: LoanRequest[];
  stock: StockItem[];
  catalog: CatalogItem[];
  teachers: TeacherItem[];
}

export interface LoanReviewRequest {
  loanId?: string;
  teacherId?: string;
  teacherName?: string;
  itemCode?: string;
  itemName?: string;
  moment: ReviewMoment | string;
  condition: ItemCondition;
  startDate?: string;
  endDate?: string;
  note?: string;
}

export interface NewLoanRequest {
  itemName: string;
  itemCode: string;
  requesterName: string;
  requesterRole: RequesterRole;
  pickupDateIso: string;
  pickupTime: string;
  note?: string;
}
