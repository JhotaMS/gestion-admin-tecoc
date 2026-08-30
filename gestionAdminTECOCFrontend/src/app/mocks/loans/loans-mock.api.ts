import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { LoansApi } from '../../loans/loans-api';
import {
  CatalogItem,
  LoanRequest,
  LoanReviewRequest,
  LoansSnapshot,
  NewLoanRequest,
  StockItem,
  UpdateLoanRequest,
} from '../../loans/loans.models';

let loans: LoanRequest[] = [
  { id: '1', itemName: 'Multímetro digital', itemCode: 'MT-014', requesterName: 'Camila Restrepo', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 29 ago 2026', dueInDays: 6, condition: 'bueno' },
  { id: '2', itemName: 'Taladro inalámbrico', itemCode: 'TL-002', requesterName: 'Julián Torres', requesterRole: 'Docente', status: 'reservado', scheduleLabel: 'Retiro hoy 15:00', dueInDays: 0, condition: 'pend' },
  { id: '3', itemName: 'Kit de disección', itemCode: 'KD-031', requesterName: 'Valentina Gómez', requesterRole: 'Estudiante', status: 'atrasado', scheduleLabel: 'Venció 18 ago 2026', dueInDays: -5, condition: 'regular' },
  { id: '4', itemName: 'Proyector portátil', itemCode: 'PR-005', requesterName: 'Andrés Muñoz', requesterRole: 'Docente', status: 'devuelto', scheduleLabel: 'Devuelto 20 ago 2026', dueInDays: null, condition: 'bueno' },
  { id: '5', itemName: 'Osciloscopio de banco', itemCode: 'OS-009', requesterName: 'Laura Serna', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 30 ago 2026', dueInDays: 7, condition: 'bueno' },
  { id: '6', itemName: 'Cámara réflex', itemCode: 'CM-017', requesterName: 'Esteban Cárdenas', requesterRole: 'Estudiante', status: 'reservado', scheduleLabel: 'Retiro hoy 16:30', dueInDays: 0, condition: 'pend' },
  { id: '7', itemName: 'Microscopio óptico', itemCode: 'MC-006', requesterName: 'Daniela Peña', requesterRole: 'Docente', status: 'atrasado', scheduleLabel: 'Venció 19 ago 2026', dueInDays: -4, condition: 'danado' },
  { id: '8', itemName: 'Estación de soldadura', itemCode: 'ES-021', requesterName: 'Ricardo Bermúdez', requesterRole: 'Estudiante', status: 'devuelto', scheduleLabel: 'Devuelto 19 ago 2026', dueInDays: null, condition: 'regular' },
  { id: '9', itemName: 'Trípode profesional', itemCode: 'TR-011', requesterName: 'Sofía Londoño', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 24 ago 2026', dueInDays: 1, condition: 'bueno' },
  { id: '10', itemName: 'Calculadora científica', itemCode: 'CC-044', requesterName: 'Mateo Cifuentes', requesterRole: 'Estudiante', status: 'reservado', scheduleLabel: 'Retiro mañana 09:00', dueInDays: 1, condition: 'pend' },
];

let nextLoanId = 11;

const stock: StockItem[] = [
  { name: 'Multímetro digital', available: 3, total: 8 },
  { name: 'Taladro inalámbrico', available: 1, total: 4 },
  { name: 'Proyector portátil', available: 5, total: 6 },
  { name: 'Microscopio óptico', available: 0, total: 5 },
  { name: 'Trípode profesional', available: 2, total: 5 },
];

const CATALOG: CatalogItem[] = [
  { name: 'Multímetro digital', code: 'MT-014' },
  { name: 'Taladro inalámbrico', code: 'TL-002' },
  { name: 'Kit de disección', code: 'KD-031' },
  { name: 'Proyector portátil', code: 'PR-005' },
  { name: 'Osciloscopio de banco', code: 'OS-009' },
  { name: 'Cámara réflex', code: 'CM-017' },
  { name: 'Microscopio óptico', code: 'MC-006' },
  { name: 'Estación de soldadura', code: 'ES-021' },
  { name: 'Trípode profesional', code: 'TR-011' },
  { name: 'Calculadora científica', code: 'CC-044' },
];

const MESES = ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'];

function scheduleFromPickup(pickupDateIso: string, pickupTime: string): { label: string; dueInDays: number } {
  const [year, month, day] = pickupDateIso.split('-').map(Number);
  const pickup = new Date(year, month - 1, day);
  const today = new Date();
  const today0 = new Date(today.getFullYear(), today.getMonth(), today.getDate());
  const dueInDays = Math.round((pickup.getTime() - today0.getTime()) / 86_400_000);

  let prefix: string;
  if (dueInDays === 0) {
    prefix = 'Retiro hoy';
  } else if (dueInDays === 1) {
    prefix = 'Retiro mañana';
  } else if (dueInDays > 1) {
    prefix = `Retiro ${pickup.getDate()} ${MESES[pickup.getMonth()]} ${pickup.getFullYear()}`;
  } else {
    prefix = `Retiro atrasado ${pickup.getDate()} ${MESES[pickup.getMonth()]} ${pickup.getFullYear()}`;
  }

  return { label: `${prefix} ${pickupTime}`, dueInDays };
}

@Injectable()
export class LoansMockApi extends LoansApi {
  getSnapshot(): Observable<LoansSnapshot> {
    return of({ loans: [...loans], stock: [...stock], catalog: [...CATALOG] }).pipe(delay(300));
  }

  reviewLoan(request: LoanReviewRequest): Observable<LoanRequest> {
    const target = loans.find((loan) => loan.id === request.loanId);
    if (!target) {
      return throwError(() => new Error('Solicitud no encontrada.'));
    }

    const updated: LoanRequest = {
      ...target,
      condition: request.condition,
      status: request.moment === 'entrega' ? 'entregado' : 'devuelto',
    };
    loans = loans.map((loan) => (loan.id === request.loanId ? updated : loan));

    return of(updated).pipe(delay(300));
  }

  createLoan(request: NewLoanRequest): Observable<LoanRequest> {
    const { label, dueInDays } = scheduleFromPickup(request.pickupDateIso, request.pickupTime);

    const created: LoanRequest = {
      id: String(nextLoanId++),
      itemName: request.itemName,
      itemCode: request.itemCode,
      requesterName: request.requesterName,
      requesterRole: request.requesterRole,
      status: 'reservado',
      scheduleLabel: label,
      dueInDays,
      condition: 'pend',
    };

    loans = [created, ...loans];

    return of(created).pipe(delay(400));
  }

  updateLoan(request: UpdateLoanRequest): Observable<LoanRequest> {
    const target = loans.find((loan) => loan.id === request.id);
    if (!target) {
      return throwError(() => new Error('Solicitud no encontrada.'));
    }

    const updated: LoanRequest = {
      ...target,
      itemName: request.itemName,
      itemCode: request.itemCode,
      requesterName: request.requesterName,
      requesterRole: request.requesterRole,
      status: request.status,
      condition: request.condition,
    };
    loans = loans.map((loan) => (loan.id === request.id ? updated : loan));

    return of(updated).pipe(delay(300));
  }

  deleteLoan(loanId: string): Observable<void> {
    const exists = loans.some((loan) => loan.id === loanId);
    if (!exists) {
      return throwError(() => new Error('Solicitud no encontrada.'));
    }

    loans = loans.filter((loan) => loan.id !== loanId);

    return of(undefined).pipe(delay(300));
  }
}
