import {Component, signal, viewChild} from '@angular/core';
import {FormComponent} from '../../../../shared/components/form/form.component';
import {SharedFormField} from '../../../../shared/components/form/shared-form-field.interface';
import {FormGroup, Validators} from '@angular/forms';
import {passwordsMatchValidator} from './passwordsMatch.validator';
import {SharedForm} from '../../../../shared/components/form/shared-form.interface';
import {Button} from 'primeng/button';
import {Toolbar} from 'primeng/toolbar';
import {RouterLink} from '@angular/router';
import {Divider} from 'primeng/divider';
import {NgOptimizedImage} from '@angular/common';
import {UserService} from '../../../services/user.service';
import {RegisterUserRequest} from '../../models/RegisterUserRequest';
import {Feedback} from '../../../../shared/components/form/feedback.interface';

@Component({
  selector: 'core-register-user-page',
  imports: [FormComponent, Button, Toolbar, RouterLink, Divider, NgOptimizedImage],
  templateUrl: './register-user-page.component.html',
  styles: ``
})
export class RegisterUserPageComponent {

  constructor(private userService: UserService) { }

  private readonly formGroup = signal<FormGroup>(new FormGroup({}));
  formComponent = viewChild(FormComponent);

  registerFormFields = signal<SharedFormField[]>([
    {
      name: "username",
      label: "Nombre de usuario",
      type: "text",
      validators: [Validators.required, Validators.minLength(4), Validators.maxLength(25)],
      errorMessages: {
        required: "Este campo es requerido",
        minlength: "El nombre de usuario debe tener al menos 4 caracteres",
        maxlength: "El nombre de usuario no puede tener más de 25 caracteres"
      },
      icon: "pi-user"
    },
    {
      name: "name",
      label: "Nombre",
      type: "text",
      validators: [Validators.required],
      errorMessages: {
        required: "Este campo es requerido"
      },
      icon: "pi-question-circle"
    },
    {
      name: "lastname",
      label: "Apellido",
      type: "text",
      validators: [Validators.required, Validators.minLength(3), Validators.maxLength(200)],
      errorMessages: {
        required: "Este campo es requerido",
        minlength: "El apelldo debe tener al menos 3 caracteres",
        maxlength: "El apellido no puede tener más de 200 caracteres"
      },
      icon: "pi-question-circle"
    },
    {
      name: "email",
      label: "E-mail",
      type: "text",
      validators: [Validators.required, Validators.email],
      errorMessages: {
        required: "Este campo es requerido",
        email: "El e-mail no es válido"
      },
      icon: "pi-at"
    },
    {
      name: "password",
      label: "Contraseña",
      type: "password",
      validators: [Validators.required, Validators.minLength(8), Validators.maxLength(25)],
      errorMessages: {
        required: "Este campo es requerido",
        minlength: "La contraseña debe tener al menos 8 caracteres",
        maxlength: "La contraseña no puede tener más de 25 caracteres"
      },
      icon: "pi-lock"
    },
    {
      name: "confirmPassword",
      label: "Confirmar contraseña",
      type: "password",
      validators: [Validators.required],
      errorMessages: {
        required: "Este campo es requerido",
        passwordMismatch: "Las contraseñas no coinciden"
      },
      icon: "pi-lock",
      showFormErrors: true
    }
  ]);

  registerFormSettings = signal<SharedForm>({
    fields: this.registerFormFields(),
    baseButtonLabel: "Registrarse",
    submittedButtonLabel: "Registrado exitosamente",
    formValidators: [passwordsMatchValidator("password", "confirmPassword")]
  })

  features: { title: string, details: string}[] = [
    { title: "Registra tu cuenta", details: "Crea tu cuenta en unos pocos pasos" },
    { title: "Accede a tu cuenta", details: "Inicia sesión con tu cuenta y disfruta de nuestros servicios" },
    { title: "Invita a tus amigos", details: "Invita a tus amigos y vecinos a unirse a tu comunidad"},
    { title: "Solicita servicios", details: "Solicita servicios y ayuda a tus vecinos a través de nuestra plataforma"},
    { title: "Administra tu comunidad desde tu dispositivo", details: "¡Administra tu comunidad a solo un click de distancia!"}
  ]

  // Handler para recibir el FormGroup del componente hijo
  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  onSubmit(value: any) {
    const request: RegisterUserRequest = this.userService.convertToRegisterUserRequest(value);
    console.log("Request: ", request);
    const formComponent = this.formComponent();
    this.userService.registerUser(request).subscribe({
      next(response) {
        const status = response.isSuccess ? "success" : "error";

        const message = "Usuario registrado exitosamente";

        const feedback: Feedback = { status, message };
        formComponent?.resetForm(feedback);
      },
      error(err) {
        const status = "error";
        console.log(err)
        const message = err.error.Errors[0].Message;
        console.log("Message: ", message);
        const feedback: Feedback = { status, message };
        formComponent?.resetForm(feedback);
      }
    });
  }

  async loginWithGoogle() {
    try{
      await this.userService.loginWithGoogle();
    }catch (e) {
      console.log(e)
    }
  }

  async logOut() {
    try {
      await this.userService.logOut();
    } catch (e) {
      console.log(e);
    }
  }
}
