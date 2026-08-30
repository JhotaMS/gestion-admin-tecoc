import { Observable } from 'rxjs';
import { LoanRequest, LoanReviewRequest, LoansSnapshot, NewLoanRequest, UpdateLoanRequest } from './loans.models';

export abstract class LoansApi {
  abstract getSnapshot(): Observable<LoansSnapshot>;
  abstract reviewLoan(request: LoanReviewRequest): Observable<LoanRequest>;
  abstract createLoan(request: NewLoanRequest): Observable<LoanRequest>;
  abstract updateLoan(request: UpdateLoanRequest): Observable<LoanRequest>;
  abstract deleteLoan(loanId: string): Observable<void>;
}
