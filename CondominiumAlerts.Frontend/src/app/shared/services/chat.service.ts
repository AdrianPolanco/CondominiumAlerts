import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Message } from '../../core/models/message.models';
import { BehaviorSubject } from 'rxjs';
import { ChatOptions } from '../components/chat/chat.type';
import { ChatMessageDto } from '../../core/models/chatMessage.dto';

@Injectable({
  providedIn: 'root'
})
export class ChatService{

  private chatOptions = new BehaviorSubject<ChatOptions | null>(null);
  chatOptions$ = this.chatOptions.asObservable();

  constructor(private httpClient: HttpClient) { }

  setChatOptions(options: ChatOptions) {
    this.chatOptions.next(options);
  }

  getMessagesByCondominium(condominiumId: string) {
    return this.httpClient.get<{ isSuccess: boolean, data: ChatMessageDto[]}>(`api/condominiums/${condominiumId}/messages`);
  }

  //TODO: Implement this method and API
  getMessagesByUsers(userId: string, otherUserId: string) {
   // return this.httpClient.get<{ messages: Message[]}>(`api/users/${userId}/messages/${otherUserId}`);
  }
}
