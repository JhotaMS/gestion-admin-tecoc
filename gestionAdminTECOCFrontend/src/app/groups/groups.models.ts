export interface Group {
  id: string;
  name: string;
  code: string;
  enabled: boolean;
  cupoTotal: number;
  cupoDisponible: number;
}

export interface CreateGroupRequest {
  name: string;
  code: string;
  cupoTotal: number;
}

export interface UpdateGroupRequest {
  groupId: string;
  name: string;
  code: string;
  cupoTotal: number;
}

export const GROUP_NAME_MAX_LENGTH = 100;
export const GROUP_CODE_MAX_LENGTH = 30;
