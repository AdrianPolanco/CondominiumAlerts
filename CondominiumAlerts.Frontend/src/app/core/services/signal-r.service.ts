import { Injectable } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { AuthService } from '../auth/services/auth.service';
import { Subject } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: HubConnection;
  onHubConnected = new Subject<boolean>();

  constructor(private authService: AuthService) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7048/hubs/chat', {
        accessTokenFactory: () => this.authService.getUserToken(),
      })
      .build();
  }

  async start() {
    try {
      if (this.authService.isUserLoggedIn) {
        await this.hubConnection.start();
        this.onHubConnected.next(true);
      }
    } catch (error) {
      this.onHubConnected.next(false);
    }
  }

  async closeConnection() {
    try {
      await this.hubConnection.stop();
      this.onHubConnected.next(false);
    } catch (error) {
      this.onHubConnected.next(false);
    }
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
