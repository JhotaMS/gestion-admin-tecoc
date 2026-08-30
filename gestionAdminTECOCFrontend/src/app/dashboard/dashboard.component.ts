import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { DashboardApi } from './dashboard-api';
import { DashboardStat, ActivityItem } from './dashboard.models';
import { NotificationsMockStream } from '../mocks/notifications/notifications-mock.stream';
import { NotificationMessage } from '../core/websocket/websocket.model';

interface WeeklyBar {
  value: number;
  heightPercent: number;
}

const WEEKLY_REQUESTS = [42, 58, 51, 66, 74, 63, 81, 72, 88, 79, 94, 86];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly dashboardApi = inject(DashboardApi);
  private readonly notificationsStream = inject(NotificationsMockStream);
  private streamSubscription?: Subscription;
  private clockTimer?: ReturnType<typeof setInterval>;

  readonly loading = signal(true);
  readonly stats = signal<DashboardStat[]>([]);
  readonly recentActivity = signal<ActivityItem[]>([]);
  readonly liveNotifications = signal<NotificationMessage[]>([]);
  readonly clock = signal('');

  readonly weeklyRequests: WeeklyBar[] = WEEKLY_REQUESTS.map((value) => ({ value, heightPercent: value }));

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

    this.updateClock();
    this.clockTimer = setInterval(() => this.updateClock(), 1000);
  }

  ngOnDestroy(): void {
    this.streamSubscription?.unsubscribe();
    if (this.clockTimer) {
      clearInterval(this.clockTimer);
    }
  }

  clearNotifications(): void {
    this.liveNotifications.set([]);
  }

  private updateClock(): void {
    const now = new Date();
    const pad = (value: number) => String(value).padStart(2, '0');
    this.clock.set(`${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`);
  }
}
