import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../enviroments/environment';
import {RegisterUserResponse} from '../register/models/RegisterUserResponse';
import {RegisterUserRequest} from '../register/models/RegisterUserRequest';
import {Auth, signInWithPopup, GoogleAuthProvider} from '@angular/fire/auth';
import {signOut} from 'firebase/auth';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient, private auth: Auth) { }

  registerUser(registerUserRequest: RegisterUserRequest) {
    return this.httpClient.post<RegisterUserResponse>("/api/users/register", registerUserRequest);
  }

  async loginWithGoogle(){
    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(this.auth, provider);
      return result.user;
    }catch (error) {
      throw error;
    }
  }

  async logOut(){
    try {
      await signOut(this.auth);
    }catch (error) {
      throw error;
    }
  }

  async getCurrentUser(){
    return await this.auth.currentUser;
  }

  convertToRegisterUserRequest(user: any): RegisterUserRequest {
    return {
      username: user.username,
      name: user.name,
      lastname: user.lastname,
      email: user.email,
      password: user.password,
    };

  }

}
