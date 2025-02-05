import {Component, OnInit, signal} from '@angular/core';
import {FormComponent} from '../../../../shared/components/form/form.component';
import {SharedFormField} from '../../../../shared/components/form/shared-form-field.interface';
import {FormGroup, Validators} from '@angular/forms';
import {passwordsMatchValidator} from './passwordsMatch.validator';
import {SharedForm} from '../../../../shared/components/form/shared-form.interface';

@Component({
  selector: 'core-register-user-page',
  imports: [FormComponent],
  templateUrl: './register-user-page.component.html',
  styles: ``
})
export class RegisterUserPageComponent {

  private readonly formGroup = signal<FormGroup>(new FormGroup({}));

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
    },
    {
      name: "cellphone",
      label: "Celular",
      type: "text",
      validators: [Validators.required, Validators.minLength(10), Validators.maxLength(25)],
      errorMessages: {
        required: "Este campo es requerido",
        minlength: "El celular debe tener al menos 10 dígitos",
        maxlength: "El celular debe tener máximo 25 dígitos"
      },
      icon: "pi-mobile"
    }
  ]);

  registerFormSettings = signal<SharedForm>({
    fields: this.registerFormFields(),
    baseButtonLabel: "Registrarse",
    submittedButtonLabel: "Registrando exitosamente",
    formValidators: [passwordsMatchValidator("password", "confirmPassword")]
  })

  // Handler para recibir el FormGroup del componente hijo
  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  onSubmit(value: any) {
    console.log("Event: ", value);
  }
}
