import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UserRegistrationApi } from '../../../core/users/user-registration-api';
import { DOCUMENT_TYPE_OPTIONS, DocumentTypeCode } from '../../../core/models/user-registration.models';

interface PasswordStrength {
  percent: number;
  color: string;
  label: string;
}

const STRENGTH_COLORS = ['#bbbbbb', '#53565a', '#0073cb', '#003892', '#00215e'];
const STRENGTH_LABELS = ['', 'Contraseña débil', 'Contraseña aceptable', 'Contraseña segura', 'Contraseña muy segura'];

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly userRegistrationApi = inject(UserRegistrationApi);

  readonly documentTypeOptions = DOCUMENT_TYPE_OPTIONS;

  readonly submitted = signal(false);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly showConfirmPassword = signal(false);
  readonly acceptTerms = signal(false);

  readonly providers = ['Microsoft', 'Google', 'SSO'];

  readonly registerForm = this.formBuilder.group({
    fullName: ['', [Validators.required]],
    documentType: ['' as DocumentTypeCode | '', [Validators.required]],
    documentNumber: ['', [Validators.required, Validators.pattern(/^[0-9]+$/), Validators.minLength(5), Validators.maxLength(15)]],
    username: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  });

  readonly passwordStrength = computed<PasswordStrength>(() => {
    const value = this.registerForm.controls.password.value ?? '';
    if (!value) {
      return { percent: 0, color: STRENGTH_COLORS[0], label: '' };
    }

    let score = 0;
    if (value.length >= 8) score++;
    if (value.length >= 12) score++;
    if (/[0-9]/.test(value)) score++;
    if (/[^A-Za-z0-9]/.test(value)) score++;

    const level = Math.min(score, 4);
    return { percent: level * 25, color: STRENGTH_COLORS[level], label: STRENGTH_LABELS[level] };
  });

  readonly passwordMatch = computed(() => {
    const password = this.registerForm.controls.password.value ?? '';
    const confirmPassword = this.registerForm.controls.confirmPassword.value ?? '';

    if (!confirmPassword) {
      return '';
    }

    return password === confirmPassword ? 'Las contraseñas coinciden' : 'Las contraseñas no coinciden';
  });

  get f() {
    return this.registerForm.controls;
  }

  toggleShowPassword(): void {
    this.showPassword.update((value) => !value);
  }

  toggleShowConfirmPassword(): void {
    this.showConfirmPassword.update((value) => !value);
  }

  toggleTerms(): void {
    this.acceptTerms.update((value) => !value);
  }

  onSubmit(): void {
    this.submitted.set(true);
    this.errorMessage.set(null);

    if (this.registerForm.invalid) {
      this.errorMessage.set('Completa todos los campos para continuar.');
      return;
    }

    if (this.registerForm.value.password !== this.registerForm.value.confirmPassword) {
      this.errorMessage.set('Las contraseñas no coinciden.');
      return;
    }

    if (!this.acceptTerms()) {
      this.errorMessage.set('Debes aceptar los términos y condiciones.');
      return;
    }

    this.loading.set(true);

    const { fullName, documentType, documentNumber, username, email, password } = this.registerForm.getRawValue();

    this.userRegistrationApi
      .createUser({
        fullName: fullName ?? '',
        documentType: (documentType || 'CC') as DocumentTypeCode,
        documentNumber: documentNumber ?? '',
        userName: username ?? '',
        email: email ?? '',
        password: password ?? '',
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigateByUrl('/login');
        },
        error: (error: Error) => {
          this.loading.set(false);
          this.errorMessage.set(error.message || 'No fue posible crear la cuenta.');
        },
      });
  }
}
