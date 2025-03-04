import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {getCondominiumsUsersCommand, getCondominiumsUsersResponse } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private hhtpClient: HttpClient) { }

  getCondominiumsUsers(cmd: getCondominiumsUsersCommand): Observable<Array<getCondominiumsUsersResponse>> {

    return this.hhtpClient.get<Array<getCondominiumsUsersResponse>>("/api/user/GetCondominiumUsers", {params: {condominiumId: cmd.condominiumId}})
  }
}
