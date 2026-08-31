import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsersApi } from './users-api';
import { UserAccount } from './users.models';
import { GroupsApi } from '../groups/groups-api';
import { Group } from '../groups/groups.models';
import { ProgramasAcademicosApi } from '../programas-academicos/programas-academicos-api';
import { ProgramaAcademico } from '../programas-academicos/programas-academicos.models';

const DOCUMENT_TYPE_LABELS: Record<string, string> = {
  CC: 'Cédula de ciudadanía',
  CE: 'Cédula de extranjería',
  TI: 'Tarjeta de identidad',
  NIT: 'Número de identificación tributaria',
};

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {
  private readonly usersApi = inject(UsersApi);
  private readonly groupsApi = inject(GroupsApi);
  private readonly programasAcademicosApi = inject(ProgramasAcademicosApi);

  readonly loading = signal(true);
  readonly users = signal<UserAccount[]>([]);
  readonly searchTerm = signal('');

  readonly groups = signal<Group[]>([]);
  readonly programasAcademicos = signal<ProgramaAcademico[]>([]);

  // Modal "Detalle de usuario"
  readonly detailsModalOpen = signal(false);
  readonly selectedUser = signal<UserAccount | null>(null);

  // Modal "Editar usuario"
  readonly editingUser = signal<UserAccount | null>(null);
  readonly editSaving = signal(false);

  readonly filteredUsers = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.users();
    }
    return this.users().filter(
      (user) => user.name.toLowerCase().includes(term) || user.email.toLowerCase().includes(term),
    );
  });

  readonly totalUsers = computed(() => this.users().length);

  ngOnInit(): void {
    this.usersApi.getUsers().subscribe((users) => {
      this.users.set(users);
      this.loading.set(false);
    });
    this.groupsApi.getGroups().subscribe((groups) => this.groups.set(groups));
    this.programasAcademicosApi
      .getProgramasAcademicos()
      .subscribe((programasAcademicos) => this.programasAcademicos.set(programasAcademicos));
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
  }

  openUserDetails(user: UserAccount): void {
    this.selectedUser.set(user);
    this.detailsModalOpen.set(true);
  }

  closeUserDetails(): void {
    this.detailsModalOpen.set(false);
  }

  openEdit(user: UserAccount): void {
    this.editingUser.set({ ...user });
  }

  closeEdit(): void {
    this.editingUser.set(null);
  }

  updateEditingField<K extends keyof UserAccount>(field: K, value: UserAccount[K]): void {
    this.editingUser.update((user) => (user ? { ...user, [field]: value } : user));
  }

  onEditingGroupChange(groupId: string): void {
    const group = this.groups().find((item) => item.id === groupId) ?? null;
    this.updateEditingField('group', group);
  }

  onEditingProgramaAcademicoChange(programaAcademicoId: string): void {
    const programaAcademico = this.programasAcademicos().find((item) => item.id === programaAcademicoId) ?? null;
    this.updateEditingField('programaAcademico', programaAcademico);
  }

  saveEdit(): void {
    const user = this.editingUser();
    if (!user || !user.name.trim() || !user.email.trim()) return;

    this.editSaving.set(true);
    this.usersApi
      .updateUser({
        id: user.id,
        name: user.name.trim(),
        email: user.email.trim(),
        group: user.group,
        programaAcademico: user.programaAcademico,
      })
      .subscribe({
        next: (updated) => {
          this.users.update((list) => list.map((u) => (u.id === updated.id ? updated : u)));
          this.editSaving.set(false);
          this.editingUser.set(null);
        },
        error: () => {
          this.editSaving.set(false);
        },
      });
  }
}
