import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import {
  getCondominiumsUsersCommand,
  getCondominiumsUsersResponse,
} from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly _currentCondominiumUserActive = signal<
    getCondominiumsUsersResponse | undefined
  >(undefined);
  currentCondominiumUserActive =
    this._currentCondominiumUserActive.asReadonly();

  constructor(private hhtpClient: HttpClient) {}

  setCurrentCondominiumUser(user: getCondominiumsUsersResponse) {
    this._currentCondominiumUserActive.set(user);
  }

  getCondominiumsUsers(
    cmd: getCondominiumsUsersCommand
  ): Observable<Array<getCondominiumsUsersResponse>> {
    return this.hhtpClient.get<Array<getCondominiumsUsersResponse>>(
      '/api/user/GetCondominiumUsers',
      { params: { condominiumId: cmd.condominiumId } }
    );
  }
}
