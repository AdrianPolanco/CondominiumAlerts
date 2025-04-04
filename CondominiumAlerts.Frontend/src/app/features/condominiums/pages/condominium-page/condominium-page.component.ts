
import { Component, signal, viewChild } from '@angular/core';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { SharedFormField } from '../../../../shared/components/form/shared-form-field.interface';
import { FormGroup, Validators } from '@angular/forms';
import { SharedForm } from '../../../../shared/components/form/shared-form.interface';
import { CondominiumService } from '../../services/condominium.service';
import { AddCondominiumCommand } from "../../models/addCondominium.command";
import { Router } from '@angular/router';
import {ButtonDirective} from 'primeng/button';
import { ChatsDrawerComponent } from "../../../../shared/components/chats-drawer/chats-drawer.component";

@Component({
    selector: 'app-condominium-page',
    templateUrl: './condominium-page.component.html',
    styleUrls: ['./condominium-page.component.css'],
  imports: [FormComponent, ButtonDirective, ChatsDrawerComponent]
})
export class CondominiumPageComponent {

  constructor(private condominiumService: CondominiumService, private router: Router) { }

  private readonly formGroup = signal<FormGroup>(new FormGroup({}));

  formComponent = viewChild(FormComponent);


  condominiumFormFields = signal<SharedFormField[]>([
    {
      name: 'name',
      label: 'Name',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'El nombre es requerido.'
      }
    },
    {
      name: 'address',
      label: 'Address',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'La dirección es requerida.'
      }
    },
    {
      name: 'imageFile',
      label: 'Subir imagen',
      type: 'file',
      filetype: 'image/*',
      onFileSelect: (event: any) => {

        if (event.files.length > 0) {
          const file = event.files[0];
          this.formGroup().patchValue({
            imageFile: file,
          });
          console.log("Archivo seleccionado")
        }
      }
    }
  ]);

  condominiumFormSettings = signal<SharedForm>({
    fields: this.condominiumFormFields(),
    baseButtonLabel: 'Enviar',
    submittedButtonLabel: '¡Enviado satisfactoriamente!'
  });

    onFormCreated(form: FormGroup) {
        this.formGroup.set(form);
    }
    goToMainPage(){
      this.router.navigate(["condominiums"])
    }

  onSubmit(value: AddCondominiumCommand) {
    const formComponent = this.formComponent();
    this.condominiumService.create(value).subscribe({
      next: (response) => {
        formComponent?.resetForm({
          status: 'success',
          message: '¡Condominio creado satisfactoriamente!',
        });
      },
      error: (err) => {
        console.log(err)
        formComponent?.resetForm({
          status: 'error',
          message: err.error?.message || 'Ha ocurrido un error mientras se creaba el condominio.',
        });
      }
    });
  }
}
