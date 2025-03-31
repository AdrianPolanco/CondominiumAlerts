import { Injectable } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  HubConnectionState,
  HttpTransportType,
} from '@microsoft/signalr';
import { AuthService } from '../auth/services/auth.service';
import { Subject } from 'rxjs';
import { ChatMessageDto } from '../models/chatMessage.dto';
@Injectable({
  providedIn: 'root',
})
export class ChatSignalRService {
  private hubConnection: HubConnection;
  onHubConnected = new Subject<boolean>();
  onNewMessage = new Subject<ChatMessageDto>();

  constructor(private authService: AuthService) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/api/hubs/chat', {
        accessTokenFactory: () => this.authService.getUserToken(),
        transport: HttpTransportType.ServerSentEvents,
      })
      .configureLogging(LogLevel.Debug)
      .withAutomaticReconnect()
      .build();
  }

  async start() {
    try {
      if (!this.isHubConnected) {
        await this.hubConnection.start();
        this.onHubConnected.next(true);
        this.addEventHandlers();
      }
    } catch (error) {
      this.onHubConnected.next(false);
      console.log(error);
    }
  }

  private addEventHandlers() {
    
  }

  async closeConnection() {
    try {
      await this.hubConnection.stop();
      this.onHubConnected.next(false);
    } catch (error) {
      this.onHubConnected.next(false);
    }
  }

  async sendMessage(){
  }

  async joinToGroup(group: string) {
    await this.hubConnection.invoke('JoinGroup', group);
  }

  async leftToGroup(group: string) {
    await this.hubConnection.invoke('LeaveGroup', group);
  }

  get isHubConnected() {
    return this.hubConnection.state === HubConnectionState.Connected;
  }
}
