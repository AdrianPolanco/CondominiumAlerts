import { Component, OnInit, ViewChild, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, Validators } from '@angular/forms';
import { PostService } from '../../../posts/services/post.service';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { UpdatePostCommand } from '../../models/posts.model';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { SharedFormField } from '../../../../shared/components/form/shared-form-field.interface';
import { SharedForm } from '../../../../shared/components/form/shared-form.interface';
import { ButtonDirective } from 'primeng/button';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-edit-post-page',
  templateUrl: './edit-post-page.component.html',
  styleUrls: ['./edit-post-page.component.css'],
  imports: [FormComponent, ButtonDirective, NgIf]
})
export class EditPostPageComponent implements OnInit {
  condominiumId: string | null = null;
  postId: string | null = null;
  currentImageUrl: string | null = null;

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
      label: 'Cambiar imagen',
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
    baseButtonLabel: 'Actualizar',
    submittedButtonLabel: '¡Actualizado satisfactoriamente!'
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
    this.postId = this.route.snapshot.paramMap.get('postId');

    if (!this.condominiumId || !this.postId) {
      console.error('Faltan parámetros en la URL');
      this.router.navigate(['']);
      return;
    }

    this.loadPostData();
  }

  loadPostData(): void {
    this.postService.getPostById(this.postId!).subscribe({
      next: (post) => {
        this.currentImageUrl = post.imageUrl;
        this.formGroup().patchValue({
          title: post.title,
          description: post.description
        });
      },
      error: (err) => {
        console.error('Error al cargar el post', err);
      }
    });
  }

  onFormCreated(form: FormGroup) {
    this.formGroup.set(form);
  }

  onSubmit(value: UpdatePostCommand) {
    if (!this.postId) return;

    const formData: UpdatePostCommand = {
      title: value.title,
      description: value.description,
      imageFile: value.imageFile,
      levelOfPriorityId: 'fc279d15-da6d-4e8e-9407-74dc73b4a628' // Mismo ID fijo que en creación
    };

    this.postService.updatePost(this.postId, formData).subscribe({
      next: (response) => {
        console.log('Post actualizado satisfactoriamente', response);
        this.router.navigate(['/condominium/index', this.condominiumId]);
      },
      error: (err) => {
        console.error('Error al actualizar el post', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/condominium/index', this.condominiumId]);
  }
}
