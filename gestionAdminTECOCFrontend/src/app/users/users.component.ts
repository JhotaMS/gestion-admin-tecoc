import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { UsersApi } from './users-api';
import { UserAccount } from './users.models';

import { UserRegistrationApi } from '../core/users/user-registration-api';
import { CreateUserRequest } from '../core/models/user-registration.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {

  // ================================
  // SERVICIOS
  // ================================

  private readonly usersApi = inject(UsersApi);
  private readonly userRegistrationApi = inject(UserRegistrationApi);


  // ================================
  // REGISTRO DE USUARIOS
  // ================================

  readonly showRegisterForm = signal(false);
  readonly registering = signal(false);
  readonly registerError = signal('');

  newUser: CreateUserRequest = {
    fullName: '',
    documentType: '',
    documentNumber: '',
    userName: '',
    email: '',
    password: '',
  };


  // ================================
  // LISTADO DE USUARIOS
  // ================================

  readonly loading = signal(true);
  readonly users = signal<UserAccount[]>([]);
  readonly searchTerm = signal('');


  // ================================
  // FILTRO DE USUARIOS
  // ================================

  readonly filteredUsers = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();

    if (!term) {
      return this.users();
    }

    return this.users().filter(
      (user) =>
        user.name.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term),
    );
  });


  // ================================
  // INICIALIZACIÓN
  // ================================

  ngOnInit(): void {
    this.loadUsers();
  }


  // ================================
  // CARGAR USUARIOS
  // ================================

  private loadUsers(): void {
    this.loading.set(true);

    this.usersApi.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);
      },
    });
  }


  // ================================
  // INICIALES DEL USUARIO
  // ================================

  initials(name: string): string {
    const parts = name.trim().split(/\s+/);

    return (
      (parts[0]?.[0] ?? '') +
      (parts[1]?.[0] ?? '')
    ).toUpperCase();
  }


  // ================================
  // BUSCADOR
  // ================================

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
  }


  // ================================
  // ABRIR FORMULARIO DE REGISTRO
  // ================================

  openRegisterForm(): void {
    this.registerError.set('');

    this.newUser = {
      fullName: '',
      documentType: '',
      documentNumber: '',
      userName: '',
      email: '',
      password: '',
    };

    this.showRegisterForm.set(true);
  }


  // ================================
  // CERRAR FORMULARIO
  // ================================

  closeRegisterForm(): void {
    this.showRegisterForm.set(false);
    this.registerError.set('');
  }


  // ================================
  // REGISTRAR USUARIO
  // ================================

  registerUser(form: any): void {
    if (form.invalid) {
      return;
    }

    this.registering.set(true);
    this.registerError.set('');

    this.userRegistrationApi.createUser(this.newUser).subscribe({

      // ------------------------------
      // REGISTRO EXITOSO
      // ------------------------------

      next: () => {
        this.registering.set(false);

        this.closeRegisterForm();

        // Recargar la lista de usuarios
        this.loadUsers();
      },


      // ------------------------------
      // ERROR
      // ------------------------------

      error: (error: Error) => {
        this.registering.set(false);

        this.registerError.set(
          error.message ||
          'No fue posible registrar el usuario.',
        );
      },

    });
  }
}
