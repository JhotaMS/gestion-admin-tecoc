export type LoanStatus = 'reservado' | 'entregado' | 'devuelto' | 'atrasado';
export type ItemCondition = 'bueno' | 'regular' | 'danado' | 'pend';
export type ReviewMoment = 'entrega' | 'devolucion';
export type RequesterRole = 'Estudiante' | 'Docente';

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
}

export interface LoanReviewRequest {
  loanId: string;
  moment: ReviewMoment;
  condition: ItemCondition;
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
