import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoansApi } from './loans-api';
import {
  CatalogItem,
  ItemCondition,
  LoanRequest,
  LoanStatus,
  RequesterRole,
  ReviewMoment,
  StockItem,
} from './loans.models';

interface StatusFilterOption {
  key: 'todos' | LoanStatus;
  label: string;
}

const BLUE = '#0073cb';
const NEUTRAL = '#53565a';
const SUCCESS = '#1c8a4b';
const WARNING = '#b8790a';
const DANGER = '#c0392b';

const STATUS_FILTERS: StatusFilterOption[] = [
  { key: 'todos', label: 'Todos' },
  { key: 'reservado', label: 'Reservado' },
  { key: 'entregado', label: 'Entregado' },
  { key: 'devuelto', label: 'Devuelto' },
  { key: 'atrasado', label: 'Atrasado' },
];

const STATUS_LABELS: Record<LoanStatus, string> = {
  reservado: 'Reservado',
  entregado: 'Entregado',
  devuelto: 'Devuelto',
  atrasado: 'Atrasado',
};

const STATUS_COLORS: Record<LoanStatus, string> = {
  reservado: BLUE,
  entregado: '#003892',
  devuelto: NEUTRAL,
  atrasado: DANGER,
};

const CONDITION_LABELS: Record<ItemCondition, string> = {
  bueno: 'Bueno',
  regular: 'Regular',
  malo: 'Malo',
  danado: 'Dañado',
  pend: 'Pendiente de revisión',
};

const CONDITION_COLORS: Record<ItemCondition, string> = {
  bueno: SUCCESS,
  regular: WARNING,
  malo: DANGER,
  danado: DANGER,
  pend: '#bbbbbb',
};

function todayIso(): string {
  const now = new Date();
  const pad = (value: number) => String(value).padStart(2, '0');
  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
}

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './loans.component.html',
})
export class LoansComponent implements OnInit {
  private readonly loansApi = inject(LoansApi);

  readonly statusFilters = STATUS_FILTERS;
  readonly conditionOptions: { key: ItemCondition; label: string }[] = [
    { key: 'malo', label: 'MALO' },
    { key: 'regular', label: 'Regular' },
    { key: 'bueno', label: 'Bueno' },
  ];
  readonly roleOptions: RequesterRole[] = ['Estudiante', 'Docente'];
  readonly minPickupDate = todayIso();

  readonly loading = signal(true);
  readonly loans = signal<LoanRequest[]>([]);
  readonly stock = signal<StockItem[]>([]);
  readonly catalog = signal<CatalogItem[]>([]);
  readonly searchTerm = signal('');
  readonly statusFilter = signal<'todos' | LoanStatus>('todos');

  // Formulario lateral de revisión / estado de un préstamo existente
  readonly selectedLoanId = signal<string>('');
  readonly reviewMoment = signal<ReviewMoment>('inicio');
  readonly reviewCondition = signal<ItemCondition | null>('bueno');
  readonly reviewStartDate = signal<string>(todayIso());
  readonly reviewEndDate = signal<string>(todayIso());
  readonly reviewNote = signal<string>('');
  readonly reviewMessage = signal<{ text: string; tone: 'success' | 'error' } | null>(null);
  readonly submitting = signal(false);

  readonly selectedLoan = computed(
    () => this.loans().find((loan) => loan.id === this.selectedLoanId()) ?? null,
  );

  // Modal "Nueva solicitud"
  readonly newLoanModalOpen = signal(false);
  readonly newItemCode = signal('');
  readonly newRequesterName = signal('');
  readonly newRequesterRole = signal<RequesterRole | null>(null);
  readonly newPickupDate = signal('');
  readonly newPickupTime = signal('09:00');
  readonly newNote = signal('');
  readonly newLoanSubmitted = signal(false);
  readonly newLoanSubmitting = signal(false);
  readonly newLoanMessage = signal<{ text: string; tone: 'success' | 'error' } | null>(null);

  readonly filteredLoans = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const status = this.statusFilter();
    return this.loans().filter((loan) => {
      const matchesTerm =
        !term ||
        loan.itemName.toLowerCase().includes(term) ||
        loan.requesterName.toLowerCase().includes(term);
      const matchesStatus = status === 'todos' || loan.status === status;
      return matchesTerm && matchesStatus;
    });
  });

  readonly stats = computed(() => {
    const all = this.loans();
    const total = all.length;
    const reservado = all.filter((loan) => loan.status === 'reservado').length;
    const entregado = all.filter((loan) => loan.status === 'entregado').length;
    const atrasado = all.filter((loan) => loan.status === 'atrasado').length;

    return {
      total,
      reservado,
      entregado,
      atrasado,
      reservadoPercent: total ? Math.round((reservado / total) * 100) : 0,
      entregadoPercent: total ? Math.round((entregado / total) * 100) : 0,
      atrasadoPercent: total ? Math.round((atrasado / total) * 100) : 0,
    };
  });

  ngOnInit(): void {
    this.loansApi.getSnapshot().subscribe((snapshot) => {
      this.loans.set(snapshot.loans);
      this.stock.set(snapshot.stock);
      this.catalog.set(snapshot.catalog);

      const initial =
        snapshot.loans.find((loan) => loan.status !== 'devuelto') ?? snapshot.loans[0];
      if (initial) {
        this.selectLoan(initial.id);
      }
      this.loading.set(false);
    });
  }

  setStatusFilter(status: 'todos' | LoanStatus): void {
    this.statusFilter.set(status);
  }

  selectLoan(loanId: string): void {
    this.selectedLoanId.set(loanId);
    this.reviewMessage.set(null);

    const loan = this.loans().find((item) => item.id === loanId);
    if (!loan) {
      return;
    }

    // "Fin Préstamo" aplica a implementos ya entregados; el resto parte de "Inicio".
    this.reviewMoment.set(loan.status === 'entregado' ? 'fin' : 'inicio');

    const editableConditions: ItemCondition[] = ['malo', 'regular', 'bueno'];
    this.reviewCondition.set(
      editableConditions.includes(loan.condition) ? loan.condition : null,
    );

    this.reviewStartDate.set(loan.startDate ?? todayIso());
    this.reviewEndDate.set(loan.endDate ?? todayIso());
    this.reviewNote.set('');
  }

  selectCondition(condition: ItemCondition): void {
    this.reviewCondition.set(condition);
  }

  statusLabel(status: LoanStatus): string {
    return STATUS_LABELS[status];
  }

  statusColor(status: LoanStatus): string {
    return STATUS_COLORS[status];
  }

  conditionLabel(condition: ItemCondition): string {
    return CONDITION_LABELS[condition];
  }

  conditionColor(condition: ItemCondition): string {
    return CONDITION_COLORS[condition];
  }

  scheduleColor(dueInDays: number | null): string {
    if (dueInDays === null) {
      return NEUTRAL;
    }
    if (dueInDays < 0) {
      return DANGER;
    }
    if (dueInDays <= 2) {
      return BLUE;
    }
    return SUCCESS;
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  submitReview(): void {
    const loan = this.selectedLoan();
    const condition = this.reviewCondition();

    if (!loan) {
      this.reviewMessage.set({
        text: 'Selecciona un préstamo existente para registrar la revisión.',
        tone: 'error',
      });
      return;
    }

    if (!condition) {
      this.reviewMessage.set({
        text: 'Selecciona el estado del implemento.',
        tone: 'error',
      });
      return;
    }

    if (this.reviewEndDate() < this.reviewStartDate()) {
      this.reviewMessage.set({
        text: 'La fecha de fin debe ser igual o posterior a la fecha de inicio.',
        tone: 'error',
      });
      return;
    }

    this.submitting.set(true);
    this.loansApi
      .reviewLoan({
        loanId: loan.id,
        teacherName: loan.requesterName,
        itemCode: loan.itemCode,
        itemName: loan.itemName,
        moment: this.reviewMoment(),
        condition,
        startDate: this.reviewStartDate(),
        endDate: this.reviewEndDate(),
        note: this.reviewNote().trim() || undefined,
      })
      .subscribe({
        next: (updated) => {
          this.loans.update((list) =>
            list.map((item) => (item.id === updated.id ? updated : item)),
          );
          this.selectedLoanId.set(updated.id);
          this.reviewMessage.set({
            text: 'Revisión registrada y préstamo actualizado.',
            tone: 'success',
          });
          this.reviewNote.set('');
          this.submitting.set(false);
          setTimeout(() => this.reviewMessage.set(null), 3500);
        },
        error: () => {
          this.reviewMessage.set({
            text: 'No fue posible registrar la revisión. Intenta nuevamente.',
            tone: 'error',
          });
          this.submitting.set(false);
        },
      });
  }

  openNewLoanModal(): void {
    this.newItemCode.set('');
    this.newRequesterName.set('');
    this.newRequesterRole.set(null);
    this.newPickupDate.set('');
    this.newPickupTime.set('09:00');
    this.newNote.set('');
    this.newLoanSubmitted.set(false);
    this.newLoanMessage.set(null);
    this.newLoanModalOpen.set(true);
  }

  closeNewLoanModal(): void {
    this.newLoanModalOpen.set(false);
  }

  selectRole(role: RequesterRole): void {
    this.newRequesterRole.set(role);
  }

  submitNewLoan(): void {
    this.newLoanSubmitted.set(true);

    const itemCode = this.newItemCode();
    const requesterName = this.newRequesterName().trim();
    const role = this.newRequesterRole();
    const pickupDate = this.newPickupDate();

    if (!itemCode || !requesterName || !role || !pickupDate) {
      return;
    }

    // Regla de negocio: una persona solo puede tener un préstamo activo a la vez.
    const hasActiveLoan = this.loans().some(
      (loan) =>
        loan.requesterName.trim().toLowerCase() === requesterName.toLowerCase() &&
        loan.status !== 'devuelto',
    );
    if (hasActiveLoan) {
      this.newLoanMessage.set({
        text: `${requesterName} ya tiene un préstamo activo. Solo se permite un préstamo por persona.`,
        tone: 'error',
      });
      return;
    }

    const item = this.catalog().find((candidate) => candidate.code === itemCode);
    if (!item) {
      return;
    }

    this.newLoanSubmitting.set(true);
    this.loansApi
      .createLoan({
        itemName: item.name,
        itemCode: item.code,
        requesterName,
        requesterRole: role,
        pickupDateIso: pickupDate,
        pickupTime: this.newPickupTime() || '09:00',
        note: this.newNote().trim() || undefined,
      })
      .subscribe({
        next: (created) => {
          this.loans.update((list) => [created, ...list]);
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set({ text: 'Solicitud creada correctamente.', tone: 'success' });
          setTimeout(() => this.closeNewLoanModal(), 900);
        },
        error: () => {
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set({
            text: 'No fue posible crear la solicitud. Intenta nuevamente.',
            tone: 'error',
          });
        },
      });
  }
}
