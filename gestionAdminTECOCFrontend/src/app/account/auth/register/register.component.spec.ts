import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { RegisterComponent } from './register.component';
import { AuthApi } from '../../../core/auth/auth-api';
import { UserRegistrationApi } from '../../../core/users/user-registration-api';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [
        provideRouter([]),
        { provide: AuthApi, useValue: { login: () => of(null), register: () => of(null), getCurrentUser: () => of(null) } },
        { provide: UserRegistrationApi, useValue: { createUser: () => of(null) } },
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
});
