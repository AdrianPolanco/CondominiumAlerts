import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import {
  GetCondominiumsUsersCommand,
  GetCondominiumsUsersResponse,
} from '../models/user.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators'; 

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly _currentCondominiumUserActive = signal<
    GetCondominiumsUsersResponse | undefined
  >(undefined);
  currentCondominiumUserActive =
    this._currentCondominiumUserActive.asReadonly();

  constructor(private hhtpClient: HttpClient) {}

  setCurrentCondominiumUser(user: GetCondominiumsUsersResponse) {
    this._currentCondominiumUserActive.set(user);
  }

  getCondominiumsUsers(
    cmd: GetCondominiumsUsersCommand
  ): Observable<GetCondominiumsUsersResponse[]> {
    return this.hhtpClient.get<any>(
      '/api/user/GetCondominiumUsers',
      { params: { condominiumId: cmd.condominiumId } }
    ).pipe(
      map(response => {
        console.log('Respuesta completa de usuarios:', response);
        return response.data || [];
      })
    );
  }
}
