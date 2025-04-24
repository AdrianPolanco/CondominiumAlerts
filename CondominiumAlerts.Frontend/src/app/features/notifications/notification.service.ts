import { Injectable, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../core/services/authentication.service';
import { HttpClient } from '@angular/common/http';
import { User } from '../../core/auth/layout/auth-layout/user.type';
import { catchError, of, Subject, takeUntil, tap } from 'rxjs';
import { AutoUnsubscribe } from '../../shared/decorators/autounsuscribe.decorator';
import { CondominiumNotification } from '../events/types/condominiumNotification.type';
import { CondominiumEvent } from '../events/event.type';
import { CondominiumService } from '../condominiums/services/condominium.service';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

@AutoUnsubscribe()
@Injectable({
    providedIn: 'root'
})
export class NotificationService implements OnDestroy {
    private readonly notificationReceived$ = new Subject<CondominiumNotification[]>();
    public notifications$ = this.notificationReceived$.asObservable();

    private token: string | null = null;
    private user: User | null = null;
    private destroy$ = new Subject<void>();
    private hubConnection?: signalR.HubConnection;

    constructor(
        private readonly authenticationService: AuthenticationService,
        private readonly httpClient: HttpClient,
        private readonly condominiumService: CondominiumService
    ) {
        this.authenticationService.userToken$
            .pipe(takeUntil(this.destroy$))
            .subscribe(token => {
                this.token = token;
                if (token) this.initSignalRConnection();
            });

        this.authenticationService.userData$
            .pipe(takeUntil(this.destroy$))
            .subscribe(user => {
                if (user?.data) {
                    this.user = user.data;
                    if (this.hubConnection && this.user) {
                        condominiumService.getCondominiumsJoinedByUser({
                            userId: this.user.id
                        }).pipe(
                            takeUntil(this.destroy$)
                        ).subscribe({
                            next: value => {
                                value.data.forEach(value => {
                                    this.joinNotificationGroup(
                                        value.id
                                    );
                                });
                            }
                        });

                    }
                }
            });
    }


    private initSignalRConnection() {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/hubs/notification', {
                accessTokenFactory: () => this.token || ''
            })
            .configureLogging(LogLevel.Information)
            .build();

        this.hubConnection.on("ReciveNotification", (notification: CondominiumNotification) => {
            // Update local notifications
            this.get().subscribe(); // Refresh notifications
        });

        this.hubConnection.start()
            .catch(err => console.error('Error establishing SignalR connection:', err));
    }

    private joinNotificationGroup(condominiumId: string) {
        if (this.hubConnection && this.user?.id) {
            this.hubConnection.invoke('JoinGroup', condominiumId, this.user.id)
                .catch(err => console.error('Error joining notification group:', err));
        }
    }

    get() {
        return this.httpClient.get<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] } }>(`/api/notifications/user/${this.user?.id}`, {
            headers: {
                Authorization: `Bearer ${this.token}`
            }
        }).pipe(
            tap(response => {
                if (response.isSuccess) {
                    this.notificationReceived$.next(response.data.notifications);
                }
            }),
            catchError((error) => {
                console.error('Error fetching notifications:', error);
                return of<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] } }>({ isSuccess: false, data: { notifications: [] } });
            })
        )
    }

    markAsRead(condominiumNotifications: CondominiumNotification[]) {
        const condominiumEventsIds = condominiumNotifications.map(event => event.id);

        return this.httpClient.put<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] } }>('/api/notifications/read', condominiumEventsIds, {
            headers: {
                Authorization: `Bearer ${this.token}`
            }
        }).pipe(
            tap((response) => {
                console.log("NOTIFICATION RESPONSE", response)
            }),
            catchError((error) => {
                console.error('Error fetching notifications:', error);
                return of<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] } }>({ isSuccess: false, data: { notifications: [] } });
            })
        )
    }

    ngOnDestroy() {
        if (this.hubConnection) {
            this.hubConnection.stop();
        }
    }
}
