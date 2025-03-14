import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {RegisterUserResponse} from '../register/models/RegisterUserResponse';
import {RegisterUserRequest} from '../register/models/RegisterUserRequest';
import {Auth, signInWithPopup, GoogleAuthProvider, onAuthStateChanged, idToken} from '@angular/fire/auth';
import {signOut} from 'firebase/auth';
import {
  EditProfileRequest,
  EditProfileResponse,
  PartialProfile,
  Profile
} from '../auth/layout/auth-layout/profile.type';
import {Router} from '@angular/router';
import {BehaviorSubject, delay, firstValueFrom, of, tap} from 'rxjs';
import {UserData} from '../auth/layout/auth-layout/user.type';


@Injectable({
  providedIn: 'root'
})
export class UserService{

  private userDataSubject = new BehaviorSubject<UserData | null>(null);
  userData$ = this.userDataSubject.asObservable();
  private userTokenSubject = new BehaviorSubject<string | null>(null);
  userToken$ = this.userTokenSubject.asObservable();

  constructor(private httpClient: HttpClient, private auth: Auth, private router: Router) {
    // Escuchar cambios en la autenticación, cuando el usuario se autentique, obtener sus datos automáticamente y emitir los datos a los suscriptores
    onAuthStateChanged(this.auth, async (user) => {
      if (user) {
        await this.getUserData();
      } else {
        this.userDataSubject.next(null);
      }
    });

    idToken(this.auth).subscribe(token => {
      this.userTokenSubject.next(token);
    })
  }

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

  async signUpWithGoogle(){
    try {
      const user = await this.loginWithGoogle();

      if(!user) throw new Error("No se pudo iniciar sesión con Google");

      const displayName = user.displayName || '';
      const nameParts = displayName.split(' '); // Divide el nombre en partes
      const firstName = nameParts.slice(0, -1).join(' ') || displayName; // Todo menos la última palabra
      const lastName = nameParts.length > 1 ? nameParts[nameParts.length - 1] : ''; // Última palabra como apellido
      const profilePictureUrl = user.photoURL;

      const registerUserRequest: RegisterUserRequest = {
        username: displayName || user.email?.split('@')[0] || '',
        name: firstName,
        lastname: lastName,
        email: user.email!,
        password: '',
        confirmPassword: '',
        profilePictureUrl
      };

      console.log(registerUserRequest);
      const token = await user.getIdToken();
      this.httpClient.post<RegisterUserResponse>(
        `/api/users/register/google/${user.uid}`,
        registerUserRequest,
        {
          headers: {
            Authorization: `Bearer ${token}`
          }
        }
      ).subscribe({
        next: (response) => {
          console.log(response);
          this.router.navigate(['/']);
        },
        error: (error) => {
          console.error(error);
          this.router.navigate(['/login']);
        }
      });

      return user;
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

  getCurrentUser(){
    return this.auth.currentUser;
  }

  //Método para obtener los datos del usuario autenticado
  private async getUserData(){
    try {
      const currentUserId = this.auth.currentUser?.uid;
      if (!currentUserId) throw new Error("No hay usuario autenticado");
      const token = await this.auth.currentUser?.getIdToken();
      const userData = await firstValueFrom(
        this.httpClient.get<UserData>(`/api/users/${currentUserId}`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        })
      );

      this.userDataSubject.next(userData);
    } catch (error) {
      console.error("Error en getUserData:", error);

      // Si es un error de HTTP, imprime más detalles
      if (error instanceof HttpErrorResponse) {
        console.error("HTTP Error:", error.status, error.message, error.error);
      }

      this.userDataSubject.next(null);
    }
  }

  convertToRegisterUserRequest(user: any): RegisterUserRequest {
    return {
      username: user.username,
      name: user.name,
      lastname: user.lastname,
      email: user.email,
      password: user.password,
      confirmPassword: user.confirmPassword,
      profilePictureUrl: user.profilePictureUrl
    };
  }

  private convertToProfileRequest(data: any): FormData {

    const currentUserId = this.auth.currentUser?.uid;
    const formData = new FormData();

    formData.append("id", currentUserId!);
    formData.append("username", data.username);
    formData.append("name", data.name);
    formData.append("lastname", data.lastname);
    formData.append("address.street", data.street);
    formData.append("address.city", data.city);
    formData.append("address.postalCode", data.postalCode);

    if (data.profilePic) {
      formData.append("profilePic", data.profilePic); // Asegurar que es de tipo File
    }

    return formData;
  }

  editProfile(data: any, token: string | null) {
    const formData = this.convertToProfileRequest(data);
    console.log({profile: formData, token})
    if(!token) throw new Error("No hay token de usuario");
    return this.httpClient.put<EditProfileResponse>('/api/users/edit', formData, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }).pipe(
      delay(5000), // Esperar 5 segundos antes de llamar getUserData
      tap(() => this.getUserData())
    )
  }

}
