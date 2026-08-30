import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsersApi } from './users-api';
import { UserAccount } from './users.models';

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

  readonly loading = signal(true);
  readonly users = signal<UserAccount[]>([]);
  readonly searchTerm = signal('');

  readonly detailsModalOpen = signal(false);
  readonly selectedUser = signal<UserAccount | null>(null);

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
  }

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase();
  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
  }

  documentTypeLabel(code: string): string {
    return DOCUMENT_TYPE_LABELS[code] ?? code;
  }

  openUserDetails(user: UserAccount): void {
    this.selectedUser.set(user);
    this.detailsModalOpen.set(true);
  }

  closeUserDetails(): void {
    this.detailsModalOpen.set(false);
  }
}
