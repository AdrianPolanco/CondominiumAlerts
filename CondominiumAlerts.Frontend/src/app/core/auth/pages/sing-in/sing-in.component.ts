import { Component, OnInit, signal, viewChild } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormGroup, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { SharedForm } from '../../../../shared/components/form/shared-form.interface';
import { SharedFormField } from '../../../../shared/components/form/shared-form-field.interface';
import { FormComponent } from '../../../../shared/components/form/form.component';

import { Button } from 'primeng/button';
import { Toolbar } from 'primeng/toolbar';
import { RouterLink } from '@angular/router';
import { Divider } from 'primeng/divider';
import { NgOptimizedImage } from '@angular/common';
import { AuthenticationService } from '../../../services/authentication.service';
import { MessageService } from 'primeng/api';
import { FirebaseError } from 'firebase/app';

@Component({
  selector: 'core-sing-in',
  imports: [
    FormComponent,
    Button,
    Toolbar,
    RouterLink,
    Divider,
    NgOptimizedImage,
  ],
  templateUrl: './sing-in.component.html',
  styleUrl: './sing-in.component.css',
})
export class SingInComponent {
  private readonly formGroup = signal<FormGroup>(new FormGroup({}));
  formComponent = viewChild(FormComponent);

  constructor(
    private authService: AuthService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private messageService: MessageService
  ) {}

  public features: { title: string; details: string }[] = [
    {
      title: 'Registra tu cuenta',
      details: 'Crea tu cuenta en unos pocos pasos',
    },
    {
      title: 'Accede a tu cuenta',
      details: 'Inicia sesión con tu cuenta y disfruta de nuestros servicios',
    },
    {
      title: 'Invita a tus amigos',
      details: 'Invita a tus amigos y vecinos a unirse a tu comunidad',
    },
    {
      title: 'Solicita servicios',
      details:
        'Solicita servicios y ayuda a tus vecinos a través de nuestra plataforma',
    },
    {
      title: 'Administra tu comunidad desde tu dispositivo',
      details: '¡Administra tu comunidad a solo un click de distancia!',
    },
  ];

  public onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  public async login(data: any) {
    const { email, password } = data;
    try {
      const result = await this.authService.loginWithEmailAndPassword(
        email,
        password
      );
      if (result.user) {
        //Link de la pagina principal de la app
        this.router.navigateByUrl('/condominium/main-page');
        return;
      }
      this.presentToast('Error al iniciar sesión. Intente nuevamente.');
    } catch (error: any) {
      if (error instanceof FirebaseError) {
        this.handleFirebaseError(error);
        return;
      }
      this.presentToast('Ocurrió un error inesperado.');
    }
  }

  async loginWithGoogle() {
    try {
      const user = await this.authenticationService.signUpWithGoogle();
      console.log('Usuario logueado:', user);
      if (user) {
        this.router.navigateByUrl('/condominium/main-page');
        return;
      }

    } catch (e) {
      this.presentToast('Ocurrió un error inesperado.');
    }
  }

  private loginFormSettingsFormFields = signal<SharedFormField[]>([
    {
      name: 'email',
      label: 'E-mail',
      type: 'text',
      validators: [Validators.required, Validators.email],
      errorMessages: {
        required: 'Este campo es requerido',
        email: 'El e-mail no es válido',
      },
      icon: 'pi-at',
    },
    {
      name: 'password',
      label: 'Contraseña',
      type: 'password',
      validators: [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(25),
      ],
      errorMessages: {
        required: 'Este campo es requerido',
        minlength: 'La contraseña debe tener al menos 8 caracteres',
        maxlength: 'La contraseña no puede tener más de 25 caracteres',
      },
      icon: 'pi-lock',
    },
  ]);

  private handleFirebaseError(error: FirebaseError) {
    switch (error.code) {
      case 'auth/invalid-credential':
        this.presentToast('Correo o contraseña incorrectos.');
        break;
      case 'auth/user-not-found':
        this.presentToast('No existe una cuenta con este correo.');
        break;
      case 'auth/wrong-password':
        this.presentToast('Contraseña incorrecta.');
        break;
      case 'auth/too-many-requests':
        this.presentToast('Demasiados intentos. Intente más tarde.');
        break;
      default:
        this.presentToast('Error al iniciar sesión.');
        break;
    }
  }

  public presentToast(text: string, success = false) {
    this.messageService.add({
      detail: text,
      severity: success ? 'success' : 'error',
    });
  }

  public loginFormSettings = signal<SharedForm>({
    fields: this.loginFormSettingsFormFields(),
    baseButtonLabel: 'Iniciar sesión',
    submittedButtonLabel: 'Iniciar sesión',
  });
}
