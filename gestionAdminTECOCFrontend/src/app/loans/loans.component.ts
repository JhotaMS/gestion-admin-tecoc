import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoansApi } from './loans-api';
import {
  CatalogItem,
  ItemCondition,
  LoanRequest,
  LoanStatus,
  ReviewMoment,
  StockItem,
  TeacherItem,
} from './loans.models';
import { UsersApi } from '../users/users-api';
import { UserAccount } from '../users/users.models';
import { ImplementosApi } from '../core/loans/implementos-api';
import { ImplementoPrestadoApi } from '../core/loans/implemento-prestado-api';
import {
  ESTADO_TIPO_BUENO,
  ImplementoOption,
  TIPO_REVISION_INICIO,
} from '../core/models/implemento-prestado.models';

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
  private readonly usersApi = inject(UsersApi);
  private readonly implementosApi = inject(ImplementosApi);
  private readonly implementoPrestadoApi = inject(ImplementoPrestadoApi);

  readonly statusFilters = STATUS_FILTERS;
  readonly conditionOptions: { key: ItemCondition; label: string }[] = [
    { key: 'malo', label: 'MALO' },
    { key: 'regular', label: 'Regular' },
    { key: 'bueno', label: 'Bueno' },
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
  readonly teachers = signal<TeacherItem[]>([]);
  readonly searchTerm = signal('');
  readonly statusFilter = signal<'todos' | LoanStatus>('todos');

  // Formulario lateral de revisión / estado de préstamo
  readonly selectedTeacherId = signal<string>('');
  readonly selectedItemCode = signal<string>('');
  readonly reviewMoment = signal<ReviewMoment>('inicio');
  readonly reviewCondition = signal<ItemCondition | null>('bueno');
  readonly reviewStartDate = signal<string>(todayIso());
  readonly reviewEndDate = signal<string>(todayIso());
  readonly reviewNote = signal<string>('');
  readonly reviewMessage = signal<{ text: string; tone: 'success' | 'error' } | null>(null);
  readonly submitting = signal(false);

  // Modal "Nueva solicitud" — registra el préstamo contra el backend real (ImplementosPrestados)
  readonly newLoanModalOpen = signal(false);
  readonly requesterOptions = signal<UserAccount[]>([]);
  readonly implementoOptions = signal<ImplementoOption[]>([]);
  readonly newImplementoId = signal('');
  readonly newUserId = signal('');
  readonly newRequesterRoleLabel = 'Docente';
  readonly newStartDate = signal(todayIso());
  readonly newEndDate = signal(todayIso());
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
      this.teachers.set(snapshot.teachers || []);

      if (snapshot.teachers && snapshot.teachers.length > 0) {
        this.selectedTeacherId.set(snapshot.teachers[0].name);
      }
      if (snapshot.catalog && snapshot.catalog.length > 0) {
        this.selectedItemCode.set(snapshot.catalog[0].code);
      }
      this.loading.set(false);
    });

    this.usersApi.getUsers().subscribe((users) => this.requesterOptions.set(users));
    this.implementosApi.getAll().subscribe((implementos) => this.implementoOptions.set(implementos));
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
    const teacher = this.selectedTeacherId();
    const itemCode = this.selectedItemCode();
    const condition = this.reviewCondition();

    if (!teacher || !itemCode || !condition) {
      this.reviewMessage.set({
        text: 'Por favor selecciona el docente, implemento y estado.',
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

    const item = this.catalog().find((candidate) => candidate.code === itemCode);

    this.submitting.set(true);
    this.loansApi
      .reviewLoan({
        teacherName: teacher,
        itemCode: itemCode,
        itemName: item?.name,
        moment: this.reviewMoment(),
        condition,
        startDate: this.reviewStartDate(),
        endDate: this.reviewEndDate(),
        note: this.reviewNote().trim() || undefined,
      })
      .subscribe({
        next: (updated) => {
          this.loans.update((list) => {
            const exists = list.some((loan) => loan.id === updated.id);
            return exists
              ? list.map((loan) => (loan.id === updated.id ? updated : loan))
              : [updated, ...list];
          });
          this.reviewMessage.set({ text: 'Revisión y préstamo registrado correctamente.', tone: 'success' });
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
    this.newImplementoId.set(this.implementoOptions()[0]?.id ?? '');
    this.newUserId.set(this.requesterOptions()[0]?.id ?? '');
    this.newStartDate.set(todayIso());
    this.newEndDate.set(todayIso());
    this.newNote.set('');
    this.newLoanSubmitted.set(false);
    this.newLoanMessage.set(null);
    this.newLoanModalOpen.set(true);
  }

  closeNewLoanModal(): void {
    this.newLoanModalOpen.set(false);
  }

  submitNewLoan(): void {
    this.newLoanSubmitted.set(true);

    const implementoId = this.newImplementoId();
    const userId = this.newUserId();
    const startDate = this.newStartDate();
    const endDate = this.newEndDate();

    if (!implementoId || !userId || !startDate || !endDate) {
      return;
    }

    if (endDate < startDate) {
      this.newLoanMessage.set('La fecha de fin debe ser igual o posterior a la fecha de inicio.');
      return;
    }

    this.newLoanSubmitting.set(true);
    this.implementoPrestadoApi
      .create({
        userId,
        implementoId,
        tipoRevisionId: TIPO_REVISION_INICIO,
        estadoTipo: ESTADO_TIPO_BUENO,
        fechaInicio: startDate,
        fechaFin: endDate,
        observacion: this.newNote().trim() || undefined,
      })
      .subscribe({
        next: () => {
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set('Solicitud de préstamo registrada correctamente.');
          setTimeout(() => this.closeNewLoanModal(), 900);
        },
        error: (error: Error) => {
          this.newLoanSubmitting.set(false);
          this.newLoanMessage.set(error.message || 'No fue posible registrar la solicitud. Intenta nuevamente.');
        },
      });
  }
}
