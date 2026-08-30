import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { UserRegistrationApi } from '../../../core/users/user-registration-api';
import { RegisterComponent } from './register.component';
import { AuthApi } from '../../../core/auth/auth-api';
import { UserRegistrationApi } from '../../../core/users/user-registration-api';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let userRegistrationApi: jasmine.SpyObj<UserRegistrationApi>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    userRegistrationApi = jasmine.createSpyObj<UserRegistrationApi>('UserRegistrationApi', ['createUser']);
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['register']);

    userRegistrationApi.createUser.and.returnValue(
      of({
        id: '1',
        fullName: 'Usuario Prueba',
        documentType: 'CC',
        documentNumber: '12345678',
        userName: 'usuario1',
        email: 'usuario@tecoc.edu.co',
      }),
    );
    authService.register.and.returnValue(of({ id: '1', name: 'Usuario Prueba', email: 'usuario@tecoc.edu.co', role: 'user' }));

    await TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [
        provideRouter([]),
        { provide: UserRegistrationApi, useValue: userRegistrationApi },
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should render the registration title', () => {
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Crea tu cuenta');
  });

  it('should register only through the real backend API and not through the mock auth flow', () => {
    component.registerForm.setValue({
      fullName: 'Usuario Prueba',
      documentType: 'CC',
      documentNumber: '12345678',
      username: 'usuario1',
      email: 'usuario@tecoc.edu.co',
      password: 'Test123*',
      confirmPassword: 'Test123*',
    });
    component.acceptTerms.set(true);

    component.onSubmit();

    expect(userRegistrationApi.createUser).toHaveBeenCalledTimes(1);
    expect(authService.register).not.toHaveBeenCalled();
  });
});
