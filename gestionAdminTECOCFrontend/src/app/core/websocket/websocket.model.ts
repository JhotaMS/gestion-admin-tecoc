export type WebSocketConnectionState =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting';

export interface NotificationMessage {
  id: string;
  title: string;
  body: string;
  createdAtIso: string;
}
