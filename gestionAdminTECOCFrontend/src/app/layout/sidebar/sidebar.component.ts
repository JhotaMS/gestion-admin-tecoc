import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MenuItem } from '../../shared/models/menu.model';
import { SIDEBAR_MENU } from './sidebar-menu';

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
