import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

interface PasswordStrength {
  percent: number;
  color: string;
  label: string;
}

const STRENGTH_COLORS = ['#bbbbbb', '#53565a', '#0073cb', '#003892', '#00215e'];
const STRENGTH_LABELS = ['', 'Contraseña débil', 'Contraseña aceptable', 'Contraseña segura', 'Contraseña muy segura'];

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css',
})
export class SigninComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly submitted = signal(false);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly remember = signal(true);

  readonly providers = ['Microsoft', 'Google', 'SSO'];

  readonly loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  private readonly passwordValue = toSignal(this.loginForm.controls.password.valueChanges, {
    initialValue: '',
  });

  readonly passwordStrength = computed<PasswordStrength>(() => {
    const value = this.passwordValue() ?? '';
    if (!value) {
      return { percent: 0, color: STRENGTH_COLORS[0], label: '' };
    }

    let score = 0;
    if (value.length >= 6) score++;
    if (value.length >= 10) score++;
    if (/[0-9]/.test(value)) score++;
    if (/[^A-Za-z0-9]/.test(value)) score++;

    const level = Math.min(score, 4);
    return { percent: level * 25, color: STRENGTH_COLORS[level], label: STRENGTH_LABELS[level] };
  });

  get f() {
    return this.loginForm.controls;
  }

  toggleShowPassword(): void {
    this.showPassword.update((value) => !value);
  }

  toggleRemember(): void {
    this.remember.update((value) => !value);
  }

  onSubmit(): void {
    this.submitted.set(true);
    this.errorMessage.set(null);

    if (this.loginForm.invalid) {
      return;
    }

    this.loading.set(true);
    const { email, password } = this.loginForm.getRawValue();

    this.authService.login({ email: email ?? '', password: password ?? '' }).subscribe({
      next: () => {
        this.loading.set(false);
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/dashboard';
        this.router.navigateByUrl(returnUrl);
      },
      error: (error: Error) => {
        this.loading.set(false);
        this.errorMessage.set(error.message || 'No fue posible iniciar sesión');
      },
    });
  }
}
