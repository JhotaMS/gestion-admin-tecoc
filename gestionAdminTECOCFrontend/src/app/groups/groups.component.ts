import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GroupsApi } from './groups-api';
import {
  GROUP_CODE_MAX_LENGTH,
  GROUP_NAME_MAX_LENGTH,
  Group,
} from './groups.models';

type Feedback = { text: string; tone: 'success' | 'error' };

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './groups.component.html',
})
export class GroupsComponent implements OnInit {
  private readonly groupsApi = inject(GroupsApi);

  readonly nameMaxLength = GROUP_NAME_MAX_LENGTH;
  readonly codeMaxLength = GROUP_CODE_MAX_LENGTH;

  readonly loading = signal(true);
  readonly groups = signal<Group[]>([]);
  readonly feedback = signal<Feedback | null>(null);

  readonly formOpen = signal(false);
  readonly editingGroup = signal<Group | null>(null);
  readonly formName = signal('');
  readonly formCode = signal('');
  readonly formSubmitted = signal(false);
  readonly saving = signal(false);

  readonly deletingGroup = signal<Group | null>(null);
  readonly deleting = signal(false);

  readonly totalGroups = computed(() => this.groups().length);

  readonly nameError = computed(() => {
    const name = this.formName().trim();
    if (!name) return 'El nombre del grupo es obligatorio.';
    if (name.length > this.nameMaxLength) {
      return `El nombre no puede superar los ${this.nameMaxLength} caracteres.`;
    }
    return null;
  });

  readonly codeError = computed(() => {
    const code = this.formCode().trim();
    if (!code) return 'El código del grupo es obligatorio.';
    if (code.length > this.codeMaxLength) {
      return `El código no puede superar los ${this.codeMaxLength} caracteres.`;
    }
    return null;
  });

  readonly formValid = computed(() => !this.nameError() && !this.codeError());

  ngOnInit(): void {
    this.loadGroups();
  }

  loadGroups(): void {
    this.loading.set(true);
    this.groupsApi.getGroups().subscribe({
      next: (groups) => {
        this.groups.set(groups);
        this.loading.set(false);
      },
      error: (error: Error) => {
        this.loading.set(false);
        this.notify(error.message, 'error');
      },
    });
  }

  openCreate(): void {
    this.editingGroup.set(null);
    this.formName.set('');
    this.formCode.set('');
    this.formSubmitted.set(false);
    this.formOpen.set(true);
  }

  openEdit(group: Group): void {
    this.editingGroup.set(group);
    this.formName.set(group.name);
    this.formCode.set(group.code);
    this.formSubmitted.set(false);
    this.formOpen.set(true);
  }

  closeForm(): void {
    this.formOpen.set(false);
  }

  saveGroup(): void {
    this.formSubmitted.set(true);
    if (!this.formValid()) return;

    const name = this.formName().trim();
    const code = this.formCode().trim();
    const editing = this.editingGroup();

    this.saving.set(true);

    const request$ = editing
      ? this.groupsApi.updateGroup({ groupId: editing.id, name, code })
      : this.groupsApi.createGroup({ name, code });

    request$.subscribe({
      next: (group) => {
        this.groups.update((list) =>
          editing ? list.map((item) => (item.id === group.id ? group : item)) : [...list, group],
        );
        this.saving.set(false);
        this.formOpen.set(false);
        this.notify(editing ? 'Grupo actualizado correctamente.' : 'Grupo creado correctamente.', 'success');
      },
      error: (error: Error) => {
        this.saving.set(false);
        this.notify(error.message, 'error');
      },
    });
  }

  askDelete(group: Group): void {
    this.deletingGroup.set(group);
  }

  cancelDelete(): void {
    this.deletingGroup.set(null);
  }

  confirmDelete(): void {
    const group = this.deletingGroup();
    if (!group) return;

    this.deleting.set(true);
    this.groupsApi.deleteGroup(group.id).subscribe({
      next: () => {
        this.groups.update((list) => list.filter((item) => item.id !== group.id));
        this.deleting.set(false);
        this.deletingGroup.set(null);
        this.notify('Grupo eliminado correctamente.', 'success');
      },
      error: (error: Error) => {
        this.deleting.set(false);
        this.deletingGroup.set(null);
        this.notify(error.message, 'error');
      },
    });
  }

  private notify(text: string, tone: Feedback['tone']): void {
    this.feedback.set({ text, tone });
    setTimeout(() => this.feedback.set(null), 4000);
  }
}
