import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { DashboardApi } from './dashboard-api';
import { DashboardStat, ActivityItem } from './dashboard.models';
import { NotificationsMockStream } from '../mocks/notifications/notifications-mock.stream';
import { NotificationMessage } from '../core/websocket/websocket.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly dashboardApi = inject(DashboardApi);
  private readonly notificationsStream = inject(NotificationsMockStream);
  private streamSubscription?: Subscription;

  readonly loading = signal(true);
  readonly stats = signal<DashboardStat[]>([]);
  readonly recentActivity = signal<ActivityItem[]>([]);
  readonly liveNotifications = signal<NotificationMessage[]>([]);

  ngOnInit(): void {
    this.dashboardApi.getSnapshot().subscribe((snapshot) => {
      this.stats.set(snapshot.stats);
      this.recentActivity.set(snapshot.recentActivity);
      this.loading.set(false);
    });

    // Simulated real-time channel — swap for NotificationsHubService once the
    // SignalR hub is deployed (see core/websocket/notifications-hub.service.ts).
    this.streamSubscription = this.notificationsStream.stream().subscribe((message) => {
      this.liveNotifications.update((current) => [message, ...current].slice(0, 5));
    });
  }

  ngOnDestroy(): void {
    this.streamSubscription?.unsubscribe();
  }
}
