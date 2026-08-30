import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsersApi } from './users-api';
import { UserAccount, UserStatus } from './users.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {
  private readonly usersApi = inject(UsersApi);

  readonly loading = signal(true);
  readonly users = signal<UserAccount[]>([]);
  readonly searchTerm = signal('');

  readonly statusOptions: { key: UserStatus; label: string }[] = [
    { key: 'activo', label: 'Activo' },
    { key: 'pendiente', label: 'Pendiente' },
  ];

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

  readonly stats = computed(() => {
    const all = this.users();
    const total = all.length;
    const activos = all.filter((user) => user.status === 'activo').length;
    const pendientes = total - activos;
    const now = new Date();
    const nuevos = all.filter((user) => {
      const registered = new Date(user.registeredAtIso);
      return (
        registered.getFullYear() === now.getFullYear() && registered.getMonth() === now.getMonth()
      );
    }).length;

    return {
      total,
      activos,
      pendientes,
      nuevos,
      activosPercent: total ? Math.round((activos / total) * 100) : 0,
      pendientesPercent: total ? Math.round((pendientes / total) * 100) : 0,
      nuevosPercent: total ? Math.round((nuevos / total) * 100) : 0,
    };
  });

  ngOnInit(): void {
    this.usersApi.getUsers().subscribe((users) => {
      this.users.set(users);
      this.loading.set(false);
    });
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
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

  saveEdit(): void {
    const user = this.editingUser();
    if (!user || !user.name.trim() || !user.email.trim()) return;

    this.editSaving.set(true);
    this.usersApi.updateUser(user).subscribe({
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
