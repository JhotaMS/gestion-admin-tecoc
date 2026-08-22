import { Observable } from 'rxjs';
import { DashboardSnapshot } from './dashboard.models';

export abstract class DashboardApi {
  abstract getSnapshot(): Observable<DashboardSnapshot>;
}
