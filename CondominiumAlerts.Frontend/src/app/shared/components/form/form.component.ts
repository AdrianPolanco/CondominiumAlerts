import {Component, computed, EventEmitter, input, OnChanges, output, signal, SimpleChanges,
  WritableSignal
} from '@angular/core';
import {SharedFormField} from './shared-form-field.interface';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {Select} from 'primeng/select';
import {Checkbox} from 'primeng/checkbox';
import {Textarea} from 'primeng/textarea';
import {InputGroupAddon} from 'primeng/inputgroupaddon';
import {InputGroup} from 'primeng/inputgroup';
import {NgClass} from '@angular/common';
import {DropdownModule} from 'primeng/dropdown';
import {InputText} from 'primeng/inputtext';
import {ButtonDirective, ButtonLabel} from 'primeng/button';
import {Password} from 'primeng/password';
import {SharedForm} from './shared-form.interface';
import {ProgressSpinner} from 'primeng/progressspinner';
import {Feedback} from './feedback.interface';

@Component({
  selector: 'shared-form',
  imports: [ReactiveFormsModule, Checkbox, Textarea, InputGroupAddon, InputGroup, NgClass, DropdownModule, InputText, ButtonDirective, Password, ProgressSpinner],
  templateUrl: './form.component.html'
})
export class FormComponent {
  constructor(private fb: FormBuilder) {
    //Inicializa el FormGroup
    this.form = signal<FormGroup>(this.fb.group({}));
  }

  showValidationErrors(fieldName: string) {
    const control = this.form().get(fieldName);
    console.log(`Errores en el campo "${fieldName}":`, control?.errors);
    
  }

  formSettings = input.required<SharedForm>();
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
    this.createForm();
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
      this.onFormSubmitted.emit(this.form().value);
    }
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
