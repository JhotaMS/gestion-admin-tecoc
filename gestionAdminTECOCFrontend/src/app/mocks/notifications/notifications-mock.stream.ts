import { Injectable } from '@angular/core';
import { Observable, interval, map } from 'rxjs';
import { NotificationMessage } from '../../core/websocket/websocket.model';

const SAMPLE_TITLES = [
  'Nueva solicitud asignada',
  'Actualización de estado de ticket',
  'Mensaje entrante del área de soporte',
];

/**
 * Simulates the payload a real SignalR hub would push, so the Dashboard can
 * demonstrate real-time updates before the backend hub is available.
 * Replace usages of this stream with NotificationsHubService.notifications
 * once the hub is deployed.
 */
@Injectable({ providedIn: 'root' })
export class NotificationsMockStream {
  stream(): Observable<NotificationMessage> {
    let counter = 0;
    return interval(8000).pipe(
      map(() => {
        counter += 1;
        return {
          id: `mock-${counter}`,
          title: SAMPLE_TITLES[counter % SAMPLE_TITLES.length],
          body: 'Generado por la API simulada de notificaciones.',
          createdAtIso: new Date().toISOString(),
        } satisfies NotificationMessage;
      })
    );
  }
}
