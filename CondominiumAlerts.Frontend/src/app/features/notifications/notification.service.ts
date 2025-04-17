import { Injectable } from '@angular/core';
import {AuthenticationService} from '../../core/services/authentication.service';
import {HttpClient} from '@angular/common/http';
import {User} from '../../core/auth/layout/auth-layout/user.type';
import {catchError, of, Subject, takeUntil, tap} from 'rxjs';
import {AutoUnsubscribe} from '../../shared/decorators/autounsuscribe.decorator';
import {CondominiumNotification} from '../events/types/condominiumNotification.type';
import {CondominiumEvent} from '../events/event.type';

@AutoUnsubscribe()
@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private readonly authenticationService: AuthenticationService, private readonly httpClient: HttpClient) {
    this.authenticationService.userToken$.pipe(takeUntil(this.destroy$)).subscribe(token => {
      console.log("TOKEN FROM NOTIFICATION SERVICE", token)
      this.token = token;
    });

    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      console.log("USER FROM NOTIFICATION SERVICE", user)
      if (user?.data) this.user = user?.data!
    });
  }
  private token: string|null = null;
  private user: User|null = null
  private destroy$ = new Subject<void>();

  get(){
    return this.httpClient.get<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] }}>(`/api/notifications/user/${this.user?.id}`, {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    }).pipe(
      tap((response) => {
        console.log("NOTIFICATION RESPONSE", response)
      }),
      catchError((error) => {
        console.error('Error fetching notifications:', error);
        return of<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] }}>({ isSuccess: false, data: {notifications: []} });
      })
    )
  }

  markAsRead(condominiumNotifications: CondominiumNotification[]){
    const condominiumEventsIds = condominiumNotifications.map(event => event.id);

    return this.httpClient.put<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] }}>('/api/notifications/read', condominiumEventsIds, {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    }).pipe(
      tap((response) => {
        console.log("NOTIFICATION RESPONSE", response)
      }),
      catchError((error) => {
        console.error('Error fetching notifications:', error);
        return of<{ isSuccess: boolean, data: { notifications: CondominiumNotification[] }}>({ isSuccess: false, data: {notifications: []} });
      })
    )
  }
}
