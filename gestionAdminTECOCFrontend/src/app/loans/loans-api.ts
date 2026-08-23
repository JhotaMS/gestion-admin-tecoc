import { Observable } from 'rxjs';
import { LoanRequest, LoanReviewRequest, LoansSnapshot, NewLoanRequest } from './loans.models';

export abstract class LoansApi {
  abstract getSnapshot(): Observable<LoansSnapshot>;
  abstract reviewLoan(request: LoanReviewRequest): Observable<LoanRequest>;
  abstract createLoan(request: NewLoanRequest): Observable<LoanRequest>;
}
