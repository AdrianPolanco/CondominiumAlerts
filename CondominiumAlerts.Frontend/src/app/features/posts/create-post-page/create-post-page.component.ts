import { Component, signal, viewChild } from '@angular/core';
import { FormComponent } from '../../../shared/components/form/form.component';
import { SharedFormField } from '../../../shared/components/form/shared-form-field.interface';
import { FormGroup, Validators } from '@angular/forms';
import { SharedForm } from '../../../shared/components/form/shared-form.interface';
import { PostService } from '../../posts/services/post.service'
import { CreatePostCommand } from '../models/posts.model'
import { Router } from '@angular/router';
import { Feedback } from '../../../shared/components/form/feedback.interface';
import { ButtonDirective } from 'primeng/button';

@Component({
  selector: 'app-create-post-page',
  templateUrl: './create-post-page.component.html',
  styleUrls: ['./create-post-page.component.css'],
  imports: [FormComponent, ButtonDirective]
})
export class PostPageComponent {

  constructor(private PostService: PostService, private router: Router) { }

  // Signal for the form group
  private readonly formGroup = signal<FormGroup>(new FormGroup({}));

  // Reference to the form component
  formComponent = viewChild(FormComponent);

  postsFormFields = signal<SharedFormField[]>([
    {
      name: 'title',
      label: 'Title',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'El nombre es requerido.'
      }
    },
    {
      name: 'description',
      label: 'Description',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'La dirección es requerida.'
      }
    },
    {
      name: 'imageUrl',
      label: 'Subir imagen',
      type: 'file',
      filetype: 'image/*',
      onFileSelect: (event: any) => {
        if (event.files.length > 0) {
          const file = event.files[0];
          this.formGroup().patchValue({
            imageUrl: file, 
          });
        }
      }
    }
  ]);

  postsFormSettings = signal<SharedForm>({
    fields: this.postsFormFields(),  // Cambia a .() para obtener el valor reactivo
    baseButtonLabel: 'Enviar',
    submittedButtonLabel: '¡Enviado satisfactoriamente!'
  });



  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  goToMainPage() {
    this.router.navigate(["condominium/index"])
  }

  onSubmit(value: CreatePostCommand) {
    const formComponent = this.formComponent();
    this.PostService.createPost(value).subscribe({
      next: (response) => {
        formComponent?.resetForm({
          status: 'success',
          message: '¡Post creado satisfactoriamente!',
        });
        this.goToMainPage();
      },
      error: (err) => {
        formComponent?.resetForm({
          status: 'error',
          message: err.error?.message || 'Ha ocurrido un error mientras se creaba el Post.',
        });
      }
    });
  }
}
