import { ChatCreatorUserDto } from "./chatCreatorUser.dto";

export type ChatMessageDto = {
    id: string;
    text: string;
    mediaUrl?: string;
    creatorUser: ChatCreatorUserDto;
    creatorUserId?: string;
    receiverUserId?: string;
    condominiumId?: string;
    messageBeingRepliedToId?: string;
    createdAt: Date;
}