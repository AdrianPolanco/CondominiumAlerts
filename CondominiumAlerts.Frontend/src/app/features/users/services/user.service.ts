import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import {
  GetCondominiumsUsersCommand,
  GetCondominiumsUsersResponse,
} from '../models/user.model';
import { Observable } from 'rxjs';

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
  ): Observable<Array<GetCondominiumsUsersResponse>> {
    return this.hhtpClient.get<Array<GetCondominiumsUsersResponse>>(
      '/api/user/GetCondominiumUsers',
      { params: { condominiumId: cmd.condominiumId } }
    );
  }
}
