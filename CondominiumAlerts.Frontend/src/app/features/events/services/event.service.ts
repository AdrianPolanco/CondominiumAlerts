import {inject, Injectable, OnDestroy} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, catchError, of, Subject, switchMap, takeUntil, tap} from 'rxjs';
import {CondominiumEvent, PartialEvent} from '../event.type';
import {AutoUnsubscribe} from '../../../shared/decorators/autounsuscribe.decorator';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {User} from '../../../core/auth/layout/auth-layout/user.type';
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import {CondominiumNotification} from '../types/condominiumNotification.type';
import {NotificationService} from '../../notifications/notification.service';

@AutoUnsubscribe()
@Injectable({
  providedIn: 'root'
})
export class EventService implements OnDestroy {

  private hubConnection: HubConnection | null = null;
  private eventsBehaviorSubject = new BehaviorSubject<CondominiumEvent[]>([]);
  private notificationBehaviorSubject = new BehaviorSubject<CondominiumNotification[]>([]);
  notification$ = this.notificationBehaviorSubject.asObservable();
  private readonly destroy$ = new Subject<void>();
  private token: string|null = null;
  private user: User|null = null
  events$ = this.eventsBehaviorSubject.asObservable();
  private isConnecting = false;
  private notificationService = inject(NotificationService)


  constructor(private readonly httpClient: HttpClient, private readonly authenticationService: AuthenticationService) {
    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      console.log("USER FROM EVENT SERVICE", user)
      if (user?.data) this.user = user?.data!
    });
    this.authenticationService.userToken$.pipe(takeUntil(this.destroy$)).subscribe(token => {
      console.log("TOKEN FROM EVENT SERVICE", token)
      this.token = token;
    });
  }

  // 🔌 Inicia la conexión al Hub si no existe
  private async initHubConnection(): Promise<void> {
    if (this.hubConnection || this.isConnecting) return;
    this.isConnecting = true;

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`/api/events/hubs`, {
        accessTokenFactory: () => this.token ?? '',
        transport: HttpTransportType.ServerSentEvents
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();



    return this.hubConnection.start()
      .then(() => {
        this.isConnecting = false
        this.registerHandlers()
        console.log('✅ Conectado al hub de eventos')
      })
      .catch(err => {
        console.error('❌ Error al conectar al hub:', err)
        throw err;
      });
  }

  // 🔹 Unirse a un grupo
  joinEventGroup(eventId: string): void {
    if (!this.hubConnection) {
      this.initHubConnection().then(() => {
        console.log('🔗 Conectado. Uniéndose al grupo...');
        this.testConnection()
        return this.invokeJoinGroup(eventId);
      })
        .catch(err => console.error('❌ No se pudo conectar al hub ni unirse al grupo:', err));
    } else {
      this.invokeJoinGroup(eventId);
    }
  }

  testConnection(): void {
    if (this.hubConnection && this.hubConnection.state === 'Connected') {
      this.hubConnection.invoke('Echo', 'Test message')
        .then((response) => console.log('✅ Eco recibido:', response))
        .catch(err => console.error('❌ Error en eco:', err));
    } else {
      console.error('❌ No hay conexión activa para probar');
    }
  }

  private registerHandlers(){
    this.hubConnection?.on("EventStarted", (notification: CondominiumNotification) => {
      console.log("Notificación: ", notification);
      const current = this.notificationBehaviorSubject.getValue();
      console.log("Valor actual en notificationBehaviorSubject:", current, "tipo:", typeof current);
      this.notificationBehaviorSubject.next([...current, notification]);

      if (notification.condominiumId) {
        this.get(notification.condominiumId).subscribe({
          next: () => console.log("✅ Eventos actualizados por EventStarted"),
          error: (err) => console.error("❌ Error actualizando eventos por EventStarted", err)
        });
      }
    });

    this.hubConnection?.on("EventFinished", (notification: CondominiumNotification) => {
      console.log("Notificación: ", notification);
      const current = this.notificationBehaviorSubject.getValue();
      console.log("Valor actual en notificationBehaviorSubject:", current, "tipo:", typeof current);
      this.notificationBehaviorSubject.next([...current, notification]);
    });
  }

  private invokeJoinGroup(eventId: string): void {
    this.hubConnection?.invoke('JoinGroup', eventId, this.user?.id)
      .then(() => console.log(`🟢 Unido al grupo de evento ${eventId} a las ${new Date()}`))
      .catch(err => console.error('❌ Error al unirse al grupo:', err));
  }

  // 🔸 Salir del grupo
  leaveEventGroup(eventId: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveGroup', eventId, this.user?.id)
        .then(() => console.log(`🟡 Saliste del grupo de evento ${eventId}`))
        .catch(err => console.error('❌ Error al salir del grupo:', err));
    }
    this.disconnectFromEventHub(eventId);
  }

  // 🔚 Desconectarse completamente del Hub y del grupo
  disconnectFromEventHub(eventId: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveGroup', eventId, this.user?.id)
        .then(() => this.hubConnection?.stop())
        .then(() => {
          this.hubConnection = null;
          console.log('🔌 Desconectado completamente del hub');
        })
        .catch(err => console.error('❌ Error al desconectarse del evento:', err));
    }
  }

  private loadInitialNotifications(): void {
    this.notificationService.get().pipe(takeUntil(this.destroy$)).subscribe(response => {
      if (response.isSuccess) {
        console.log("Notifications data received:", response.data)
        this.notificationBehaviorSubject.next(response.data.notifications);
      }
    });
  }


  get(condominiumId: string) {
    console.log("USER FROM EVENT SERVICE", this.user)
    return this.httpClient.get<{ isSuccess: boolean, data: CondominiumEvent[] }>(`/api/events/${condominiumId}/user/${this.user?.id}`,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }).pipe(
      tap(res => {
        this.eventsBehaviorSubject.next(res.data);               // Actualiza el observable         // Conecta al hub si está suscrito
      }),
      catchError(err => {
        console.error('Error al obtener eventos:', err);
        this.eventsBehaviorSubject.next([]);                   // Vacía el observable si hay error
        return of<{ isSuccess: boolean, data: CondominiumEvent[] }>({ isSuccess: false, data: [] });
      })
    );
  }

  getSubscribedEvents() {
    return this.httpClient.get<{ isSuccess: boolean, data: { events: CondominiumEvent[]} }>(`/api/events/subscribed`,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      });
  }

  save(partialEvent: PartialEvent, condominiumId: string) {

    const requestBody = {
      title: partialEvent.title,
      description: partialEvent.description,
      start: partialEvent.start,
      end: partialEvent.end,
      createdById: this.user?.id,
      condominiumId: condominiumId
    }

    return this.httpClient.post<{ isSuccess: boolean, data: CondominiumEvent }>(`/api/events`, requestBody,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }).pipe(
      switchMap(res => {
        if (res.isSuccess) {
          // Vuelve a obtener la lista de eventos actualizada desde el backend
          return this.get(condominiumId).pipe(
            tap(() => console.log('Eventos actualizados tras creación')),
          );
        } else {
          return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
        }
      }),
      catchError(err => {
        console.error('Error al guardar evento:', err);
        return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
      })
    );
  }

  update(condominiumEvent: CondominiumEvent, condominiumId: string){
    const requestBody = {
      id: condominiumEvent.id,
      title: condominiumEvent.title,
      description: condominiumEvent.description,
      start: condominiumEvent.start,
      end: condominiumEvent.end,
      createdById: condominiumEvent.createdBy.id,
      condominiumId: condominiumId,
      editorId: this.user?.id
    }

    return this.httpClient.put<{ isSuccess: boolean, data: CondominiumEvent }>(`/api/events`, requestBody,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }).pipe(
      switchMap(res => {
        if (res.isSuccess) {
          // Vuelve a obtener la lista de eventos actualizada desde el backend
          return this.get(condominiumId).pipe(
            tap(() => console.log('Eventos actualizados tras actualizacion')),
          );
        } else {
          return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
        }
      }),
      catchError(err => {
        console.error('Error al actualizar evento:', err);
        return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
      }));
  }

  delete(event: string, condominiumId: string) {
    return this.httpClient.delete<{ isSuccess: boolean, data: CondominiumEvent }>(`/api/events/${event}/user/${this.user?.id}`,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }).pipe(
      switchMap(res => {
        if (res.isSuccess) {
          console.log("CAYENDO EN EL EXITO")
          // Vuelve a obtener la lista de eventos actualizada desde el backend
          return this.get(condominiumId).pipe(
            tap(() => console.log('Eventos actualizados tras eliminacion')),
          );
        } else {
          return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
        }
      }),
      catchError(err => {
        console.error('Error al actualizar evento:', err);
        return of({ isSuccess: false, data: [...this.eventsBehaviorSubject.getValue()] });
      }));
  }

  addSubscription(eventId: string, condominiumId: string){
    return this.httpClient.put<{isSuccess: boolean, data:{eventTitle: string}}>(`/api/events/${eventId}/subscribe/${this.user?.id}`, {}, {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    }).pipe(
      switchMap(
        res => {
          if (res.isSuccess) {
            // Vuelve a obtener la lista de eventos actualizada desde el backend
            return this.get(condominiumId).pipe(
              tap(() => console.log('Eventos actualizados tras suscripción')),
            );
          } else {
            return of({ isSuccess: false, data: {events: []} });
          }
        })
    )
  }

  removeSubscription(eventId: string, condominiumId: string){
    return this.httpClient.put<{isSuccess: boolean, data:{eventTitle: string}}>(`/api/events/${eventId}/unsubscribe/${this.user?.id}`, {}, {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    }).pipe(
      switchMap(
        res => {
          if (res.isSuccess) {
            // Vuelve a obtener la lista de eventos actualizada desde el backend
            return this.get(condominiumId).pipe(
              tap(() => console.log('Eventos actualizados tras desuscripción')),
            );
          } else {
            return of({ isSuccess: false, data: {events: []} });
          }
        })
    )
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
