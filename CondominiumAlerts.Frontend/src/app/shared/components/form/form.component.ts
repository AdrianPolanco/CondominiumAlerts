import {Component, computed, input, OnInit, output, signal, WritableSignal} from '@angular/core';
import {SharedFormField} from './shared-form-field.interface';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {Checkbox} from 'primeng/checkbox';
import {Textarea} from 'primeng/textarea';
import {InputGroupAddon} from 'primeng/inputgroupaddon';
import {InputGroup} from 'primeng/inputgroup';
import {NgClass} from '@angular/common';
import {DropdownModule} from 'primeng/dropdown';
import {InputText} from 'primeng/inputtext';
import {ButtonDirective} from 'primeng/button';
import {Password} from 'primeng/password';
import {SharedForm} from './shared-form.interface';
import {ProgressSpinner} from 'primeng/progressspinner';
import {Feedback} from './feedback.interface';

@Component({
  selector: 'shared-form',
  imports: [
    ReactiveFormsModule,
    Checkbox,
    Textarea,
    InputGroupAddon,
    InputGroup,
    NgClass,
    DropdownModule,
    InputText,
    ButtonDirective,
    Password,
    ProgressSpinner
  ],
  templateUrl: './form.component.html'
})
export class FormComponent implements OnInit{
  constructor(private fb: FormBuilder) {
    //Inicializa el FormGroup
    this.form = signal<FormGroup>(this.fb.group({}));
    //this.formSettings = signal<SharedForm>(this.formSettingsInput());
  }

  formSettingsInput = input.required<SharedForm>();
  formSettings!: WritableSignal<SharedForm>;
  fields = computed<SharedFormField[]>(() => this.formSettings().fields);

  form: WritableSignal<FormGroup<any>>;
  isLoading = signal<boolean>(false);
  feedback = computed<Feedback|undefined>(() => this.formSettings().feedback);

  //Evento que se emite cuando se envía el formulario
  onFormSubmitted = output<any>();
  //Evento que se emite cuando se crea el formulario
  onFormCreated = output<FormGroup>();


  //Crea el formulario al inicializar el componente
  ngOnInit() {
    this.formSettings = signal(this.formSettingsInput());
    this.createForm();

    this.form().valueChanges.subscribe(() => {
      if (this.feedback()?.status === 'success') {
        this.formSettings.set({
          ...this.formSettings(),
          feedback: { status: 'none', message: '' }
        });
      }
    });
  }

  createForm() {
    const group: any = {};

    // Crear los controles básicos
    this.fields().forEach(field => {
      group[field.name] = ['', field.validators || []];
    });

    // Crear el formGroup
    const formGroup = this.fb.group(group);

    // Agregar validadores personalizados a nivel de formulario si existen
    if (this.formSettings().formValidators?.length) {
      formGroup.addValidators(this.formSettings().formValidators!);
    }

    this.form.set(formGroup);
    this.onFormCreated.emit(formGroup);
  }

  onSubmit() {
    //Si el formulario es válido, emite el evento formSubmitted con el valor del formulario
    if (this.form().valid) {
      this.isLoading.set(true);
      this.onFormSubmitted.emit(this.form().value);
    }
  }

  resetForm(feedback: Feedback) {
    if(feedback.status === "success") this.form().reset();
    this.isLoading.set(false);
    const updatedSettings = {
      ...this.formSettings(),
      feedback: feedback
    };

    // Use the input method to update form settings
    this.formSettings.set(updatedSettings);
  }


  getErrorMessage(field: SharedFormField): string {
    const control = this.form().get(field.name);
    if (!control) return '';

    // Verificar errores del control
    if (control.errors) {
      for (const error in control.errors) {
        if (field.errorMessages?.[error]) {
          return field.errorMessages[error];
        }
      }
    }

    // Verificar errores a nivel de formulario
    const formErrors = this.form().errors;
    if (formErrors) {
      for (const error in formErrors) {
        if (field.errorMessages?.[error]) {
          return field.errorMessages[error];
        }
      }
    }

    return 'Error desconocido';
  }
}
