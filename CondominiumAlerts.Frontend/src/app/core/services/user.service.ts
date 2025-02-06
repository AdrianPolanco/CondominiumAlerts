import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../enviroments/environment';
import {RegisterUserResponse} from '../register/models/RegisterUserResponse';
import {RegisterUserRequest} from '../register/models/RegisterUserRequest';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) { }

  baseUrl = environment.backBaseUrl;

  registerUser(registerUserRequest: RegisterUserRequest) {
    return this.httpClient.post<RegisterUserResponse>(`${this.baseUrl}/users/register`, registerUserRequest);
  }

  convertToRegisterUserRequest(user: any) {
    return {
      username: user.username,
      name: user.name,
      lastname: user.lastname,
      email: user.email,
      password: user.password,
      confirmPassword: user.confirmPassword,
      phoneNumber: {
        number: user.cellphone
      }
    };
  }

}
