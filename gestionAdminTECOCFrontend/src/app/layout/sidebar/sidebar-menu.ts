import { MenuItem } from '../../shared/models/menu.model';

/**
 * Starter menu. Add new sections here as modules are built —
 * SidebarComponent renders this list without further changes.
 */
export const SIDEBAR_MENU: MenuItem[] = [
  { label: 'Dashboard', icon: 'layout-dashboard', link: '/dashboard' },
  {
    label: 'Gestión',
    icon: 'folder-kanban',
    children: [
      { label: 'Solicitudes', icon: 'file-text', link: '/dashboard' },
      { label: 'Usuarios', icon: 'users', link: '/usuarios' },
      { label: 'Préstamo de implementos', icon: 'package', link: '/prestamos' },
      { label: 'Asistencia', icon: 'calendar-check', link: '/asistencia' },
    ],
  },
  { label: 'Configuración', icon: 'settings', link: '/dashboard' },
];
