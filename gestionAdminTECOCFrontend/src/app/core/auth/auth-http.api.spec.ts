import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AuthHttpApi } from './auth-http.api';

describe('AuthHttpApi', () => {
  let service: AuthHttpApi;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthHttpApi],
    });

    service = TestBed.inject(AuthHttpApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should call the real backend login endpoint with email and password', () => {
    service.login({ username: 'docente@tecoc.edu.co', password: 'Test123*' }).subscribe();

    const req = httpMock.expectOne('/api/v1/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      email: 'docente@tecoc.edu.co',
      password: 'Test123*',
    });
    req.flush({ userId: 'abc-123', email: 'docente@tecoc.edu.co' });
  });
});
