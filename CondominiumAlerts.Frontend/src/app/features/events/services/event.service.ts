import {Injectable, OnDestroy} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, catchError, of, Subject, takeUntil, tap} from 'rxjs';
import {CondominiumEvent, PartialEvent} from '../event.type';
import {AutoUnsubscribe} from '../../../shared/decorators/autounsuscribe.decorator';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {User} from '../../../core/auth/layout/auth-layout/user.type';

@AutoUnsubscribe()
@Injectable({
  providedIn: 'root'
})
export class EventService implements OnDestroy {

  private eventsBehaviorSubject = new BehaviorSubject<CondominiumEvent[]>([]);
  private readonly destroy$ = new Subject<void>();
  private token: string|null = null;
  private user: User|null = null
  events$ = this.eventsBehaviorSubject.asObservable();

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

  getEvents(condominiumId: string) {
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
