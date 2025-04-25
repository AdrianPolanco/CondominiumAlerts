import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { NgFor, CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../../posts/services/post.service';
import { GetCondominiumsUsersResponse } from '../../../users/models/user.model';
import { UserService } from '../../../users/services/user.service';
import { CondominiumService } from '../../services/condominium.service';
import { GetCondominiumResponse } from "../../models/getCondominium.response";
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { FormsModule } from '@angular/forms';
import { PriorityLevelService } from '../../../services/priorityLevel.service';
import { priorityDto } from '../../../priority-levels/models/priorityDto';
import { CommetService } from '../../../Comments/services/comment.service';
import { AddCommentCommand } from '../../../Comments/models/AddComment.Command'
import { getCommentByPostCommand } from '../../../Comments/models/getCommentByPost.Command'
import { getCommentByPostResponse } from '../../../Comments/models/getCommentByPost.Reponse'
import { CreatePostsResponse, UpdatePostCommand, PostFormData } from '../../../posts/models/posts.model';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';
import { getCondominiumTokenResponse } from '../../models/getCondominiumToken.response';
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { User } from '../../../../core/auth/layout/auth-layout/user.type';
import { Subject, takeUntil } from 'rxjs';
import { ChatsDrawerComponent } from '../../../../shared/components/chats-drawer/chats-drawer.component';
import { BackArrowComponent } from '../../../../shared/components/back-arrow/back-arrow.component';
import { CondominiumsLayoutComponent } from '../../../../shared/components/condominiums-layout/condominiums-layout.component';
import { MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { map } from 'rxjs/operators';
@Component({
  selector: 'app-condominium-index',
  standalone: true,
  imports: [
    NgFor,
    ConfirmDialogModule,
    TimeAgoPipe,
    CommonModule,
    FormsModule,
      ButtonModule,
      ChatsDrawerComponent,
      BackArrowComponent,
      CondominiumsLayoutComponent, 
      Toast
  ],
  providers: [ConfirmationService],
  templateUrl: './condominum-index.component.html',
  styleUrls: ['./condominum-index.component.css'],
})
export class CondominumIndexComponent implements OnInit {
  priorityLevels: priorityDto[] = [];
  @Input() postId!: string;

  displayModal: boolean = false; // modal de delete
  postToDelete: any = null;  
  comments: { [postId: string]: getCommentByPostResponse[] } = {};
  showComments: { [postId: string]: boolean } = {};
  newComments: { [postId: string]: { text: string; imageFile?: File; currentImageUrl: string; } } = {};
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
  currentToken: getCondominiumTokenResponse | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private postService: PostService,
    private userService: UserService,
    private condominiumService: CondominiumService,
    private authService: AuthService,
    private priorityService: PriorityLevelService,
    private commentService: CommetService,
    private authenticationService: AuthenticationService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,

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
this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData?.data) {
        this.user = userData?.data
      };
    })
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
    this.loadComments(this.postId);
    this.loadPriorityLevels();
  }
  user: User  | null= null
destroy$ = new Subject<void>;
  getLink(): void{

    let url = `${window.location.origin}/condominium/joinWithToken/`
    if(this.currentToken != null && this.currentToken.token && new Date(this.currentToken?.expirDate) > new Date() ){
     // console.log("not server response")
      url += encodeURIComponent(this.currentToken.token) 
   //   console.log(url)
      this.copyToClipBoard(url)

      return;
    }

    this.condominiumService.getCondominiumToken({
      UserId: this.user?.id === undefined ? "": this.user.id,
      condominiumId: this.condominiumId ?? ""

    }).subscribe({
      next:  (result) => {
        this.currentToken = result.data
        url += encodeURIComponent(this.currentToken.token)
      //  console.log(this.currentToken)     
    //    console.log(url)
        this.copyToClipBoard(url)
      },
      error: (err)=>{
      //  console.error(err)
      }
    })
  }
  //TODO: When changing to  https remove this code and use the modern approach (await navigator.clipboard.writeText(text))
  copyToClipBoard(value:string){
    const textArea = document.createElement('textarea')
    textArea.value = value;
    textArea.style.position = 'fixed'
    document.body.appendChild(textArea)
    textArea.focus();
    textArea.select();
    document.execCommand('copy');
    document.body.removeChild(textArea)
    this.messageService.add({
      severity: 'success',
      summary: 'Enlace copiado',
      detail: 'El enlace para unirse al condominio ha sido copiado al portapapeles',
      life: 3000
    });
  }
  
  toggleComments(postId: string): void {
    this.showComments[postId] = !this.showComments[postId];
    console.log('Toggle comments para post:', postId);
    if (this.showComments[postId] && !this.comments[postId]) {
      this.loadComments(postId);
    }
    if (!this.newComments[postId]) {
      this.newComments[postId] = { text: '', imageFile: undefined, currentImageUrl: ''};
    }
  }

  loadComments(postId: string): void {
    if (!postId) return;

    const command: getCommentByPostCommand = { postid: postId };

    this.commentService.getCommentsByPost(command).subscribe({
      next: (comments) => {
        this.comments[postId] = comments?.data || [];
        this.initializeNewComments(postId);
        console.log('Comentarios cargados:', this.comments[postId]);
      },
      error: (err) => {
        console.error('Error al cargar comentarios:', err);
        this.comments[postId] = [];
      }
    });
  }

  initializeNewComments(postId: string): void {
    if (!this.newComments[postId]) {
      this.newComments[postId] = { text: '', imageFile: undefined, currentImageUrl: '' };
    }
  }


  onCommentFileSelected(event: Event, postId: string): void {
    const input = event.target as HTMLInputElement;

    if (!this.newComments[postId]) {
      this.newComments[postId] = { text: '', imageFile: undefined, currentImageUrl: '' };
    }

    // Limpiar la vista previa anterior si existe
    if (this.newComments[postId].currentImageUrl) {
      URL.revokeObjectURL(this.newComments[postId].currentImageUrl);
    }

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.newComments[postId].imageFile = file;

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.newComments[postId].currentImageUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    } else {
      this.newComments[postId].imageFile = undefined;
      this.newComments[postId].currentImageUrl = '';
    }
  }


  submitComment(postId: string): void {
    const commentData = this.newComments[postId];

    // Validar que haya texto en el comentario
    if (!commentData.text || !commentData.text.trim()) {
      return;
    }

    const command: AddCommentCommand = {
      text: commentData.text,
      ImageFile: commentData.imageFile
    };

    this.commentService.createComment(command, postId).subscribe({
      next: () => {
        // Resetear completamente el formulario de comentario
        this.resetCommentForm(postId);
        // Recargar los comentarios
        this.loadComments(postId);
      },
      error: (err) => {
        console.error('Error al enviar comentario:', err);
      }
    });
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

        this.publications.forEach(publication => {
          this.showComments[publication.id] = false; // Inicialmente ocultos
          this.newComments[publication.id] = { text: '', imageFile: undefined, currentImageUrl: '' };
          this.loadComments(publication.id); // Cargar comentarios en segundo plano
        });
      },
      error: (err) => {
        console.error('Error al cargar las publicaciones:', err);
      },
    });
  }

  loadUsers(): void {
    if (!this.condominiumId) return;

    this.userService.getCondominiumsUsers({ condominiumId: this.condominiumId }).subscribe({
      next: (users) => {
        this.users = users
 
  },
      error: (err) => {
        console.error('Error al cargar usuarios:', err);
      }
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

  resetCommentForm(postId: string): void {
    if (this.newComments[postId]?.currentImageUrl) {
      URL.revokeObjectURL(this.newComments[postId].currentImageUrl);
    }

    this.newComments[postId] = {
      text: '',
      imageFile: undefined,
      currentImageUrl: ''
    };

    const fileInput = document.querySelector(`input[type="file"][data-post-id="${postId}"]`) as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  //Confirmacion para eliminar
  confirmDelete(postId: string): void {
    this.confirmationService.confirm({
      message: '¿Estás seguro de que deseas eliminar esta publicación?',
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí',
      rejectLabel: 'No',
      accept: () => {
        this.postService.deletePost({
          id: postId,
          condominiumId: this.condominiumId!,
        }).subscribe({
          next: () => {
            this.loadPosts(); 
            this.messageService.add({
              severity: 'success',
              summary: 'Post Eliminado',
              detail: 'El post ha sido eliminado exitosamente.',
              life: 3000
            });
          },
          error: (err) => {
            console.error('Error al eliminar el post:', err);
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Hubo un error al eliminar el post.',
              life: 3000
            });
          }
        });
      }
    });
  }


}
