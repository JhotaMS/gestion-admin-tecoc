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
  danado: 'Dañado',
  pend: 'Pendiente de revisión',
};

const CONDITION_COLORS: Record<ItemCondition, string> = {
  bueno: SUCCESS,
  regular: WARNING,
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
    { key: 'bueno', label: 'Bueno' },
    { key: 'regular', label: 'Regular' },
    { key: 'danado', label: 'Dañado' },
  ];
  readonly roleOptions: RequesterRole[] = ['Estudiante', 'Docente'];
  readonly editStatusOptions = STATUS_FILTERS.filter((option) => option.key !== 'todos') as {
    key: LoanStatus;
    label: string;
  }[];
  readonly minPickupDate = todayIso();

  readonly loading = signal(true);
  readonly loans = signal<LoanRequest[]>([]);
  readonly stock = signal<StockItem[]>([]);
  readonly catalog = signal<CatalogItem[]>([]);
  readonly searchTerm = signal('');
  readonly statusFilter = signal<'todos' | LoanStatus>('todos');

  readonly selectedLoanId = signal<string | null>(null);
  readonly reviewMoment = signal<ReviewMoment>('entrega');
  readonly reviewCondition = signal<ItemCondition | null>(null);
  readonly reviewNote = signal('');
  readonly reviewMessage = signal<{ text: string; tone: 'success' | 'error' } | null>(null);
  readonly submitting = signal(false);

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
  readonly newLoanMessage = signal<string | null>(null);

  // Modal "Eliminar por ID"
  readonly deleteModalOpen = signal(false);
  readonly deleteIdInput = signal('');
  readonly deleteSubmitting = signal(false);

  // Modal "Editar solicitud"
  readonly editingLoan = signal<LoanRequest | null>(null);
  readonly editSaving = signal(false);

  readonly deleteMatch = computed<LoanRequest | null>(() => {
    const id = this.deleteIdInput().trim();
    if (!id) return null;
    return this.loans().find((loan) => loan.id === id) ?? null;
  });

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
      this.selectedLoanId.set(snapshot.loans[0]?.id ?? null);
      this.loading.set(false);
    });
  }

  setStatusFilter(status: 'todos' | LoanStatus): void {
    this.statusFilter.set(status);
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

  openDeleteModal(): void {
    this.deleteIdInput.set('');
    this.deleteModalOpen.set(true);
  }

  closeDeleteModal(): void {
    this.deleteModalOpen.set(false);
  }

  setDeleteIdInput(value: string): void {
    this.deleteIdInput.set(value.replace(/[^0-9]/g, ''));
  }

  confirmDeleteById(): void {
    const match = this.deleteMatch();
    if (!match) return;

    this.deleteSubmitting.set(true);
    this.loansApi.deleteLoan(match.id).subscribe({
      next: () => {
        this.loans.update((list) => list.filter((loan) => loan.id !== match.id));
        if (this.selectedLoanId() === match.id) {
          this.selectedLoanId.set(this.loans()[0]?.id ?? null);
        }
        this.deleteSubmitting.set(false);
        this.deleteModalOpen.set(false);
      },
      error: () => {
        this.deleteSubmitting.set(false);
      },
    });
  }

  openEditLoan(loan: LoanRequest): void {
    this.editingLoan.set({ ...loan });
  }

  closeEditLoan(): void {
    this.editingLoan.set(null);
  }

  updateEditingLoanField<K extends keyof LoanRequest>(field: K, value: LoanRequest[K]): void {
    this.editingLoan.update((loan) => (loan ? { ...loan, [field]: value } : loan));
  }

  saveEditLoan(): void {
    const loan = this.editingLoan();
    if (!loan || !loan.itemName.trim() || !loan.requesterName.trim()) return;

    this.editSaving.set(true);
    this.loansApi.updateLoan(loan).subscribe({
      next: (updated) => {
        this.loans.update((list) => list.map((current) => (current.id === updated.id ? updated : current)));
        this.editSaving.set(false);
        this.editingLoan.set(null);
      },
      error: () => {
        this.editSaving.set(false);
      },
    });
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  submitReview(): void {
    const loanId = this.selectedLoanId();
    const condition = this.reviewCondition();

    if (!loanId || !condition) {
      this.reviewMessage.set({
        text: 'Selecciona el estado del implemento antes de registrar.',
        tone: 'error',
      });
      return;
    }

    this.submitting.set(true);
    this.loansApi
      .reviewLoan({ loanId, moment: this.reviewMoment(), condition, note: this.reviewNote().trim() || undefined })
      .subscribe({
        next: (updated) => {
          this.loans.update((list) => list.map((loan) => (loan.id === updated.id ? updated : loan)));
          this.reviewMessage.set({ text: 'Revisión registrada correctamente.', tone: 'success' });
          this.reviewNote.set('');
          this.submitting.set(false);
          setTimeout(() => this.reviewMessage.set(null), 3000);
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
          this.selectedLoanId.set(created.id);
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set('Solicitud creada correctamente.');
          setTimeout(() => this.closeNewLoanModal(), 900);
        },
        error: () => {
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set('No fue posible crear la solicitud. Intenta nuevamente.');
        },
      });
  }
}
