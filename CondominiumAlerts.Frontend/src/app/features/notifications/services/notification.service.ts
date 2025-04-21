import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../enviroments/environment';
import { NotificationDto } from '../models/notification.model';

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    private readonly apiUrl = `${environment.backBaseUrl}/api/notifications`;

    constructor(private http: HttpClient) {}

    getUserNotifications(userId: string): Observable<NotificationDto[]> {
        return this.http.get<{data: NotificationDto[]}>(`${environment.backBaseUrl}/user/notifications`, {
            params: { userId }
        }).pipe(map(x => x.data));
    }
}