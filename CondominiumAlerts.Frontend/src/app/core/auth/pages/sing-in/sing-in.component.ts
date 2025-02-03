import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  FormControl,
  Validators,
} from '@angular/forms';

import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { Router } from '@angular/router';
@Component({
  selector: 'app-sing-in',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PasswordModule,
    InputTextModule,
    ButtonModule,
    MessageModule,
  ],
  templateUrl: './sing-in.component.html',
  styleUrl: './sing-in.component.css',
})
export class SingInComponent {
  public loginForm: FormGroup;
  public errorMessage: string | null = null;
  public isLoading = false;
  constructor(
    private authService: AuthService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  public async login() {
    // Validar
    const { email, password } = this.loginForm.value;
    const user = await this.authService.loginWithEmailAndPassword(
      email,
      password
    );

    if (user) {
      this.router.navigateByUrl('/HomePage');
    }
  }

  get emailControl() {
    return this.loginForm.get('email') as FormControl;
  }

  get passwordControl() {
    return this.loginForm.get('password') as FormControl;
  }
}
