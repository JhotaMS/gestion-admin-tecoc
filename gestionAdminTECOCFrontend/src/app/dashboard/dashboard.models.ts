export interface DashboardStat {
  label: string;
  value: string;
  changePercent: number;
  icon: string;
  barPercent: number;
}

export type ActivityTone = 'accent' | 'neutral';

export interface ActivityItem {
  id: string;
  title: string;
  timestampIso: string;
  tag: string;
  state: string;
  tone: ActivityTone;
}

export interface DashboardSnapshot {
  stats: DashboardStat[];
  recentActivity: ActivityItem[];
}
