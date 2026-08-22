export interface DashboardStat {
  label: string;
  value: string;
  changePercent: number;
  icon: string;
}

export interface ActivityItem {
  id: string;
  title: string;
  timestampIso: string;
}

export interface DashboardSnapshot {
  stats: DashboardStat[];
  recentActivity: ActivityItem[];
}
