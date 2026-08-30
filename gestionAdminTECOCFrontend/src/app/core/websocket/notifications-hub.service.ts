import { Injectable, OnDestroy, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';
import { NotificationMessage, WebSocketConnectionState } from './websocket.model';

/**
 * Wraps a SignalR HubConnection for real-time notifications.
 *
 * Deployment note (Angular -> Ingress -> AKS -> Hub):
 * - `environment.signalRHubUrl` must resolve through the same Ingress host as the
 *   rest of the API so the WebSocket upgrade (Connection: Upgrade, Upgrade: websocket)
 *   reaches the pod. If the Ingress controller is NGINX, annotations
 *   `nginx.ingress.kubernetes.io/proxy-read-timeout` / `proxy-send-timeout` must be
 *   raised above SignalR's server timeout (default 30s) or idle connections get cut.
 * - Multiple backend replicas require either sticky sessions (session affinity
 *   annotation on the Ingress/Service) or a backplane (e.g. Redis) on the hub side,
 *   because SignalR keeps connection state in-memory per instance.
 * - `skipNegotiation` is left false so the client still performs the `/negotiate`
 *   handshake; it is only safe to skip when the transport is hardcoded to WebSockets
 *   AND the server is configured for it, otherwise reconnects can fail silently.
 */
@Injectable({ providedIn: 'root' })
export class NotificationsHubService implements OnDestroy {
  private readonly authService = inject(AuthService);

  private connection: signalR.HubConnection | null = null;

  private readonly connectionStateSignal = signal<WebSocketConnectionState>('disconnected');
  readonly connectionState = this.connectionStateSignal.asReadonly();

  private readonly notificationsSignal = signal<NotificationMessage[]>([]);
  readonly notifications = this.notificationsSignal.asReadonly();

  async connect(): Promise<void> {
    if (this.connection) {
      return;
    }

    this.connectionStateSignal.set('connecting');

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl, {
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => this.authService.getToken() ?? '',
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
      .configureLogging(
        environment.production ? signalR.LogLevel.Warning : signalR.LogLevel.Information
      )
      .build();

    this.connection.onreconnecting((error) => {
      this.connectionStateSignal.set('reconnecting');
      console.warn('[SignalR] reconnecting...', error);
    });

    this.connection.onreconnected((connectionId) => {
      this.connectionStateSignal.set('connected');
      console.info('[SignalR] reconnected', connectionId);
    });

    this.connection.onclose((error) => {
      this.connectionStateSignal.set('disconnected');
      if (error) {
        console.error('[SignalR] connection closed with error', error);
      }
    });

    this.connection.on('Notification', (message: NotificationMessage) => {
      this.notificationsSignal.update((current) => [message, ...current].slice(0, 20));
    });

    try {
      await this.connection.start();
      this.connectionStateSignal.set('connected');
      console.info('[SignalR] connected via', this.connection.connectionId);
    } catch (error) {
      this.connectionStateSignal.set('disconnected');
      console.error('[SignalR] initial connection failed', error);
    }
  }

  async disconnect(): Promise<void> {
    if (!this.connection) {
      return;
    }
    await this.connection.stop();
    this.connection = null;
    this.connectionStateSignal.set('disconnected');
  }

  ngOnDestroy(): void {
    void this.disconnect();
  }
}
