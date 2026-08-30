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
  TeacherItem,
} from '../../loans/loans.models';

const TEACHERS: TeacherItem[] = [
  { id: 't1', name: 'Julián Torres' },
  { id: 't2', name: 'Andrés Muñoz' },
  { id: 't3', name: 'Daniela Peña' },
  { id: 't4', name: 'Carlos Mendoza' },
  { id: 't5', name: 'Martha Lucía Gómez' },
  { id: 't6', name: 'Camila Restrepo' },
];

let loans: LoanRequest[] = [
  { id: 'l1', itemName: 'Multímetro digital', itemCode: 'MT-014', requesterName: 'Camila Restrepo', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 29 ago 2026', dueInDays: 6, condition: 'bueno' },
  { id: 'l2', itemName: 'Taladro inalámbrico', itemCode: 'TL-002', requesterName: 'Julián Torres', requesterRole: 'Docente', status: 'reservado', scheduleLabel: 'Retiro hoy 15:00', dueInDays: 0, condition: 'pend' },
  { id: 'l3', itemName: 'Kit de disección', itemCode: 'KD-031', requesterName: 'Valentina Gómez', requesterRole: 'Estudiante', status: 'atrasado', scheduleLabel: 'Venció 18 ago 2026', dueInDays: -5, condition: 'regular' },
  { id: 'l4', itemName: 'Proyector portátil', itemCode: 'PR-005', requesterName: 'Andrés Muñoz', requesterRole: 'Docente', status: 'devuelto', scheduleLabel: 'Devuelto 20 ago 2026', dueInDays: null, condition: 'bueno' },
  { id: 'l5', itemName: 'Osciloscopio de banco', itemCode: 'OS-009', requesterName: 'Laura Serna', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 30 ago 2026', dueInDays: 7, condition: 'bueno' },
  { id: 'l6', itemName: 'Cámara réflex', itemCode: 'CM-017', requesterName: 'Esteban Cárdenas', requesterRole: 'Estudiante', status: 'reservado', scheduleLabel: 'Retiro hoy 16:30', dueInDays: 0, condition: 'pend' },
  { id: 'l7', itemName: 'Microscopio óptico', itemCode: 'MC-006', requesterName: 'Daniela Peña', requesterRole: 'Docente', status: 'atrasado', scheduleLabel: 'Venció 19 ago 2026', dueInDays: -4, condition: 'danado' },
  { id: 'l8', itemName: 'Estación de soldadura', itemCode: 'ES-021', requesterName: 'Ricardo Bermúdez', requesterRole: 'Estudiante', status: 'devuelto', scheduleLabel: 'Devuelto 19 ago 2026', dueInDays: null, condition: 'regular' },
  { id: 'l9', itemName: 'Trípode profesional', itemCode: 'TR-011', requesterName: 'Sofía Londoño', requesterRole: 'Estudiante', status: 'entregado', scheduleLabel: 'Vence 24 ago 2026', dueInDays: 1, condition: 'bueno' },
  { id: 'l10', itemName: 'Calculadora científica', itemCode: 'CC-044', requesterName: 'Mateo Cifuentes', requesterRole: 'Estudiante', status: 'reservado', scheduleLabel: 'Retiro mañana 09:00', dueInDays: 1, condition: 'pend' },
];

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
    return of({
      loans: [...loans],
      stock: [...stock],
      catalog: [...CATALOG],
      teachers: [...TEACHERS],
    }).pipe(delay(300));
  }

  reviewLoan(request: LoanReviewRequest): Observable<LoanRequest> {
    let target = request.loanId ? loans.find((loan) => loan.id === request.loanId) : undefined;

    if (!target && request.teacherName && request.itemCode) {
      target = loans.find(
        (loan) => loan.itemCode === request.itemCode && loan.requesterName === request.teacherName
      );
    }

    const updatedCondition = request.condition;
    const isEntrega = request.moment === 'entrega' || request.moment === '2';
    const updatedStatus = isEntrega ? 'entregado' : 'devuelto';

    if (target) {
      const updated: LoanRequest = {
        ...target,
        condition: updatedCondition,
        status: updatedStatus,
        startDate: request.startDate,
        endDate: request.endDate,
      };
      loans = loans.map((loan) => (loan.id === target!.id ? updated : loan));
      return of(updated).pipe(delay(300));
    }

    const item = CATALOG.find((c) => c.code === request.itemCode) || {
      name: request.itemName || 'Implemento',
      code: request.itemCode || 'IMP-001',
    };

    const newCreated: LoanRequest = {
      id: `loan-${Date.now()}`,
      itemName: item.name,
      itemCode: item.code,
      requesterName: request.teacherName || 'Docente',
      requesterRole: 'Docente',
      status: updatedStatus,
      scheduleLabel: request.endDate ? `Hasta ${request.endDate}` : 'En uso',
      dueInDays: 3,
      condition: updatedCondition,
      startDate: request.startDate,
      endDate: request.endDate,
    };

    loans = [newCreated, ...loans];
    return of(newCreated).pipe(delay(300));
  }

  createLoan(request: NewLoanRequest): Observable<LoanRequest> {
    const { label, dueInDays } = scheduleFromPickup(request.pickupDateIso, request.pickupTime);

    const created: LoanRequest = {
      id: `loan-${Date.now()}`,
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
}
