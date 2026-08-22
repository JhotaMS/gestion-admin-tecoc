import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { DashboardApi } from '../../dashboard/dashboard-api';
import { DashboardSnapshot } from '../../dashboard/dashboard.models';

const SNAPSHOT: DashboardSnapshot = {
  stats: [
    { label: 'Usuarios activos', value: '1,284', changePercent: 4.6, icon: 'users' },
    { label: 'Solicitudes hoy', value: '327', changePercent: 2.1, icon: 'file-text' },
    { label: 'Tickets abiertos', value: '18', changePercent: -8.3, icon: 'life-buoy' },
    { label: 'Tiempo resp. prom.', value: '2.4h', changePercent: -5.2, icon: 'clock' },
  ],
  recentActivity: [
    { id: 'a1', title: 'Nueva solicitud registrada por J. Pérez', timestampIso: '2026-08-22T08:15:00Z' },
    { id: 'a2', title: 'Ticket #482 cerrado por soporte', timestampIso: '2026-08-22T07:50:00Z' },
    { id: 'a3', title: 'Usuario María G. actualizó su perfil', timestampIso: '2026-08-22T07:20:00Z' },
  ],
};

@Injectable()
export class DashboardMockApi extends DashboardApi {
  getSnapshot(): Observable<DashboardSnapshot> {
    return of(SNAPSHOT).pipe(delay(400));
  }
}
