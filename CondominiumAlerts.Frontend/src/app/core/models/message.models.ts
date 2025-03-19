export interface Message {
  id: string;
  text: string;
  mediaUrl?: string;
  creatorUserId: string;
  receiverUserId?: string;
  condominiumId: string;
  messageBeingRepliedToId?: string;
  createdAt: Date;
  updatedAt: Date;
  messageBeingRepliedTo?: Message;
}
