import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../../posts/services/post.service';
import { GetCondominiumsUsersResponse } from '../../../users/models/user.model';
import { UserService } from '../../../users/services/user.service';
import { CondominiumService } from '../../services/condominium.service';
import { GetCondominiumResponse } from "../../models/getCondominium.response";
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { FormsModule } from '@angular/forms';
import { priorityLevelService } from '../../../services/services.service'; 
import { priorityDto } from '../../../priority-levels/models/priorityDto';
import { CreatePostsResponse, UpdatePostCommand, PostFormData } from '../../../posts/models/posts.model';
import { ChatsDrawerComponent } from '../../../../shared/components/chats-drawer/chats-drawer.component';
import { BackArrowComponent } from '../../../../shared/components/back-arrow/back-arrow.component';
import { CondominiumsLayoutComponent } from '../../../../shared/components/condominiums-layout/condominiums-layout.component';

@Component({
  selector: 'app-condominium-index',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
      ButtonModule,
      ChatsDrawerComponent,
      BackArrowComponent,
      CondominiumsLayoutComponent
  ],
  templateUrl: './condominum-index.component.html',
  styleUrls: ['./condominum-index.component.css'],
})
export class CondominumIndexComponent implements OnInit {
  priorityLevels: priorityDto[] = [];
  users: GetCondominiumsUsersResponse[] = [
    {
      id: 'dddddfsdfdfs',
      fullName: 'Juan Pérez',
      email: 'En línea',
      profilePictureUrl:
        'https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?t=st=1741553994~exp=1741557594~hmac=6790b2380695298926314ad92146ef39a5664838888864b60616996a0850ffdc&w=740',
    },
  ];

  postForm: {
    title: string;
    description: string;
    imageFile: File | null;
    currentImageUrl: string;
    levelOfPriorityId: string;
    condominiumId: string;
    userId: string | null;
  };

  condominium: GetCondominiumResponse | null = null;
  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  publications: any[] = [];
  condominiumId: string | null = null;
  showPostModal = false;
  editingPost: CreatePostsResponse['data'] | null = null;


  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private postService: PostService,
    private userService: UserService,
    private condominiumService: CondominiumService,
    private authService: AuthService,
    private priorityService: priorityLevelService,
  )
  {
    this.postForm = {
      title: '',
      description: '',
      imageFile: null as File | null,
      currentImageUrl: '',
      levelOfPriorityId: '',
      condominiumId: '',
      userId: this.authService.currentUser?.uid ?? null
    };

  }

  ngOnInit(): void {
    this.condominiumId = this.route.snapshot.paramMap.get("condominiumId");
    console.log('Condominium ID:', this.condominiumId);

    if (!this.condominiumId) {
      console.error('No se encontró el condominiumId en la URL');
      this.router.navigate(['condominium/main-page']);
      return;
    }

    this.getCondominiumData();
    this.loadPosts();
    this.loadUsers();
    this.loadPriorityLevels();
  }

  onCondominiumSelected(): void {
    this.router.navigate(['/condominium/chat']);
  }

  loadPriorityLevels(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.condominiumId) {
        resolve();
        return;
      }

      const query = {
        condominiumId: this.condominiumId,
        pageNumber: 1,
        pageSize: 100
      };

      this.priorityService.getPriorityLevels(query).subscribe({
        next: (result) => {
          this.priorityLevels = result.data.priorities;
          resolve();
        },
        error: (err) => {
          console.error('Error al cargar niveles de prioridad', err);
          reject(err);
        }
      });
    });
  }


  goToCreatePosts(): void {
    console.log('Creating a new post');
    if (this.condominiumId) {
      this.router.navigate([`/posts/create/${this.condominiumId}`]);
    }
  }

  isCurrentUserPost(userId: string): boolean {
    const currentUserId = this.authService.currentUser?.uid;
    return currentUserId === userId;
  }

  loadPosts(): void {
    if (!this.condominiumId) return;

    this.postService.getPosts(this.condominiumId).subscribe({
      next: (data) => {
        console.log('Publicaciones recibidas:', data);
        this.publications = data;
      },
      error: (err) => {
        console.error('Error al cargar las publicaciones:', err);
      },
    });
  }

  loadUsers(): void {
    if (!this.condominiumId) return;

    this.userService.getCondominiumsUsers({ condominiumId: this.condominiumId }).subscribe({
      next: (result) => {
        this.users = result;
      },
      error: (err) => {
        console.log('Error al cargar usuarios:', err);
      },
    });
  }

  getCondominiumData(): void {
    if (!this.condominiumId) return;

    this.condominiumService.get({ condominiumId: this.condominiumId }).subscribe({
      next: (result) => {
        this.condominium = result;
        console.log('Datos del condominio:', this.condominium);
      },
      error: (err) => {
        console.error('Error al cargar datos del condominio:', err);
      },
    });
  }

  goHome(): void {
    this.router.navigate(['']);
  }

  goToLevels(): void {
    if (this.condominiumId) {
      this.router.navigate(['/priority-levels/index', this.condominiumId]);
    }
  }
  

  async openPostModal(postId?: string) {
    this.showPostModal = true;

    if (this.priorityLevels.length === 0) {
      await this.loadPriorityLevels();
    }

    if (postId) {
      // Modo edición
      this.postService.getPostById(postId).subscribe({
        next: (post) => {
          this.editingPost = post;
          this.postForm = {
            ...this.postForm,
            title: post.title,
            description: post.description,
            currentImageUrl: post.imageUrl,
            levelOfPriorityId: post.levelOfPriorityId
          };
        },
        error: (err) => console.error('Error al cargar post', err)
      });
    } else {
      this.editingPost = null;
      this.resetPostForm();
    }
  }

  resetPostForm() {
    this.postForm = {
      title: '',
      description: '',
      imageFile: null,
      currentImageUrl: '',
      levelOfPriorityId: '',
      condominiumId: this.condominiumId || '',
      userId: this.authService.currentUser?.uid || ''
    };
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      if (file.size > 0) { 
        this.postForm.imageFile = file;

        const reader = new FileReader();
        reader.onload = () => {
          this.postForm.currentImageUrl = reader.result as string;
        };
        reader.readAsDataURL(file);
      } else {
        this.postForm.imageFile = null;
        this.postForm.currentImageUrl = '';
      }
    } else {
      this.postForm.imageFile = null;
      this.postForm.currentImageUrl = '';
    }
  }

  savePost() {
    if (!this.condominiumId) return;

    if (this.editingPost) {

      // Edicion de post
      const updateData: UpdatePostCommand = {
        title: this.postForm.title,
        description: this.postForm.description,
        levelOfPriorityId: this.postForm.levelOfPriorityId,
        imageFile: this.postForm.imageFile || undefined
      };

      this.postService.updatePost(this.editingPost.id, updateData).subscribe({
        next: () => {
          this.loadPosts();
          this.closePostModal();
        },
        error: (err) => console.error('Error al actualizar', err)
      });
    } else {

      // Crear post
      if (!this.condominiumId) return;

      const formData: PostFormData = {
        title: this.postForm.title,
        description: this.postForm.description,
        imageFile: this.postForm.imageFile || undefined,
        LevelOfPriorityId: this.postForm.levelOfPriorityId,
      };

      this.postService.createPost(formData, this.condominiumId).subscribe({
        next: () => {
          this.loadPosts();
          this.closePostModal();
        },
        error: (err) => {
          console.error('Error al crear post:', err);
        }
      });
    }
  }

  closePostModal() {
    this.showPostModal = false;
    this.editingPost = null;
    this.resetPostForm();
  }

  editPost(postId: string): void {
    this.openPostModal(postId);
  }

}
