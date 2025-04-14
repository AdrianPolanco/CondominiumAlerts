import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { NotificationDto } from '../../features/notifications/models/notification.model';
import { AuthService } from '../auth/services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class NotificationSignalRService {
    private hubConnection: HubConnection | null = null;
    private notificationSubject = new Subject<NotificationDto>();
    public notification$ = this.notificationSubject.asObservable();
    private connectionStatus = new BehaviorSubject<boolean>(false);
    public connectionStatus$ = this.connectionStatus.asObservable();

    constructor(private authService: AuthService) {
        this.buildConnection();
    }

    private buildConnection() {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/api/hubs/notifications', {
                accessTokenFactory: () => this.authService.getUserToken()
            })
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
            this.notificationSubject.next(notification);
        });

        this.hubConnection.onreconnected(() => this.connectionStatus.next(true));
        this.hubConnection.onclose(() => this.connectionStatus.next(false));
    }

    async startConnection(): Promise<void> {
        try {
            await this.hubConnection?.start();
            this.connectionStatus.next(true);
        } catch (err) {
            console.error('Error while starting notification connection: ', err);
        }
    }

    async joinCondominiumGroup(condominiumId: string): Promise<void> {
        if (this.hubConnection?.state === 'Connected') {
            const userId = this.authService.currentUser?.uid;
            if (!userId) return;

            await this.hubConnection.invoke('JoinGroup', condominiumId, userId);
        }
    }

    async leaveCondominiumGroup(condominiumId: string): Promise<void> {
        if (this.hubConnection?.state === 'Connected') {
            const userId = this.authService.currentUser?.uid;
            if (!userId) return;

            await this.hubConnection.invoke('LeaveGroup', condominiumId, userId);
        }
    }
}