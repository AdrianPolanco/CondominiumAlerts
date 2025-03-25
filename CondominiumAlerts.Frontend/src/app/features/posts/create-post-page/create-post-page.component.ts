import { Component, OnInit, ViewChild, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, Validators } from '@angular/forms';
import { PostService } from '../../posts/services/post.service';
import { AuthService } from '../../../core/auth/services/auth.service';
import { CreatePostCommand } from '../models/posts.model';
import { FormComponent } from '../../../shared/components/form/form.component';
import { SharedFormField } from '../../../shared/components/form/shared-form-field.interface';
import { SharedForm } from '../../../shared/components/form/shared-form.interface';
import { ButtonDirective } from 'primeng/button';

@Component({
  selector: 'app-create-post-page',
  templateUrl: './create-post-page.component.html',
  styleUrls: ['./create-post-page.component.css'],
  imports: [FormComponent, ButtonDirective]
})
export class PostPageComponent implements OnInit {
  condominiumId: string | null = null;

  // Form fields definition
  postsFormFields = signal<SharedFormField[]>([
    {
      name: 'title',
      label: 'Title',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'El título es requerido.'
      }
    },
    {
      name: 'description',
      label: 'Description',
      type: 'text',
      validators: [Validators.required],
      errorMessages: {
        required: 'La descripción es requerida.'
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
        }
      }
    }
  ]);

  postsFormSettings = signal<SharedForm>({
    fields: this.postsFormFields(),
    baseButtonLabel: 'Enviar',
    submittedButtonLabel: '¡Enviado satisfactoriamente!'
  });

  private readonly formGroup = signal<FormGroup>(new FormGroup({}));

  formComponent = ViewChild(FormComponent);

  constructor(
    private postService: PostService,
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.condominiumId = this.route.snapshot.paramMap.get('condominiumId');
    console.log('Condominium ID:', this.condominiumId);

    if (!this.condominiumId) {
      console.error('No se encontró el condominiumId en la URL');
      this.router.navigate(['']);
    }
  }

  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  onSubmit(value: CreatePostCommand) {
    const userId = this.authService.currentUser?.uid;
    if (!userId) {
      console.error('Usuario no autenticado');
      return;
    }
    const formData: CreatePostCommand = {
      ...value,
      userId: userId,
    };
    if (this.condominiumId) {
      this.postService.createPost(formData, this.condominiumId).subscribe({
        next: (response) => {
          console.log('Post creado satisfactoriamente', response);
          this.router.navigate(['/condominium/index', this.condominiumId]);
        },
        error: (err) => {
          console.error('Error al crear el post', err);
        }
      });
    } else {
      console.error('No se encontró el condominiumId');
    }
  }
}
