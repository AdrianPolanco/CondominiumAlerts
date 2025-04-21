export type CondominiumNotification = {
  id: string;
  title: string;
  description: string;
  receiverUserId: string;
  condominiumId?: string;
  eventId?: string;
  levelOfPriorityId?: string;
  createdAt: string;
  updatedAt: string;
  read: boolean;
}
