import { Component, OnInit } from '@angular/core';
import { NgFor, CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { PostService } from '../../../posts/services/post.service';
import { GetCondominiumsUsersResponse } from '../../../users/models/user.model';
import { UserService } from '../../../users/services/user.service';
import { CondominiumService } from '../../services/condominium.service';
import { GetCondominiumResponse } from "../../models/getCondominium.response";
import { CondominiumsLayoutComponent } from '../../../../shared/components/condominiums-layout/condominiums-layout.component';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-condominum-index',
  imports: [NgFor, CommonModule, CondominiumsLayoutComponent],
  templateUrl: './condominum-index.component.html',
  styleUrls: ['./condominum-index.component.css'],
})
export class CondominumIndexComponent implements OnInit {
  users: GetCondominiumsUsersResponse[] = [
    {
      id: 'dddddfsdfdfs',
      fullName: 'Juan Pérez',
      email: 'En línea',
      profilePictureUrl:
        'https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?t=st=1741553994~exp=1741557594~hmac=6790b2380695298926314ad92146ef39a5664838888864b60616996a0850ffdc&w=740',
    },
  ];

  condominium: GetCondominiumResponse | null = null;
  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  publications: any[] = [];
  condominiumId: string | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private postService: PostService,
    private userService: UserService,
    private condominiumService: CondominiumService,
    private authService: AuthService
  ) { }

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
  }

  goToCreatePosts(): void {
    console.log('Creating a new post');
    this.router.navigate([`/posts/create/${this.condominiumId}`]);
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

  openCreatePostModal(): void {
    console.log('Abrir modal de creación de publicaciones');
  }

  editPost(postId: string): void {
    if (!this.condominiumId) {
      console.error('No hay condominiumId disponible');
      return;
    }

    // Navegar a la página de edición con ambos IDs
    this.router.navigate([`/posts/edit/${this.condominiumId}/${postId}`]);
  }
}
