import { Injectable } from '@angular/core';
import { Auth } from '@angular/fire/auth';
import { Router } from '@angular/router';
import { signInWithEmailAndPassword, signOut } from 'firebase/auth';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public uidSubject = new Subject<string>();
  public currentUser?: CurrentUser;

  constructor(private auth: Auth, private router: Router) {
    this.auth.onAuthStateChanged((user) => {
      if (user) {
        this.uidSubject.next(user.uid);
        this.currentUser = {
          uid: user.uid,
          email: user.email,
          displayName: user.displayName || user.email?.split('@')[0] || '',
          admin: false,
        };
      }
    });
  }

  public async loginWithEmailAndPassword(email: string, password: string) {
    const result = await signInWithEmailAndPassword(this.auth, email, password);
    return result.user;
  }

  public async getUserToken() {
    return (await this.auth.currentUser?.getIdToken()) || '';
  }

  public async logout() {
    this.currentUser = undefined;
    return await signOut(this.auth);
  }

  get isUserLoggedIn(): boolean {
    return (
      this.auth.currentUser !== null &&
      this.currentUser !== null &&
      this.currentUser !== undefined
    );
  }
}

interface CurrentUser {
  uid: string;
  displayName: string | null;
  email: string | null;
  admin: boolean;
}
