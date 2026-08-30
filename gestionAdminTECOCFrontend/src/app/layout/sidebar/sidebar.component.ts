import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MenuItem } from '../../shared/models/menu.model';
import { SIDEBAR_MENU } from './sidebar-menu';

interface Particle {
  left: number;
  top: number;
  size: number;
  duration: number;
  delay: number;
  opacity: number;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
})
export class SidebarComponent {
  readonly menu: MenuItem[] = SIDEBAR_MENU;
  readonly collapsed = signal(false);
  private readonly expandedLabels = signal<Set<string>>(new Set());

  readonly particles: Particle[] = Array.from({ length: 16 }, () => ({
    left: 4 + Math.random() * 92,
    top: 8 + Math.random() * 86,
    size: 3 + Math.random() * 6,
    duration: 14 + Math.random() * 16,
    delay: -(Math.random() * 26),
    opacity: 0.3 + Math.random() * 0.5,
  }));

  toggleCollapsed(): void {
    this.collapsed.update((value) => !value);
  }

  toggleGroup(item: MenuItem): void {
    this.expandedLabels.update((current) => {
      const next = new Set(current);
      if (next.has(item.label)) {
        next.delete(item.label);
      } else {
        next.add(item.label);
      }
      return next;
    });
  }

  isExpanded(item: MenuItem): boolean {
    return this.expandedLabels().has(item.label);
  }
}
