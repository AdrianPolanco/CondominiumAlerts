import { Component, OnInit } from '@angular/core';
import { NgFor, CommonModule, NgOptimizedImage } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../../posts/services/post.service';
import { GetCondominiumsUsersResponse } from '../../../users/models/user.model';
import { UserService } from '../../../users/services/user.service';
import { CondominiumService } from '../../services/condominium.service';
import { GetCondominiumResponse } from "../../models/getCondominium.response";
import { CondominiumsLayoutComponent } from '../../../../shared/components/condominiums-layout/condominiums-layout.component';
import { ChatsDrawerComponent } from "../../../../shared/components/chats-drawer/chats-drawer.component";
import { BackArrowComponent } from "../../../../shared/components/back-arrow/back-arrow.component";
import { Button } from 'primeng/button';
@Component({
  selector: 'app-condominium-index',
  imports: [
    NgFor,
    CommonModule,
    CondominiumsLayoutComponent,
    ChatsDrawerComponent,
    BackArrowComponent,
    Button
],
  templateUrl: './condominum-index.component.html',
  styleUrls: ['./condominum-index.component.css'],
})
export class CondominumIndexComponent implements OnInit {
  users: Array<GetCondominiumsUsersResponse> = [
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
    private postService: PostService,
    private route: ActivatedRoute,
    private userService: UserService,
    private condominiumService: CondominiumService
  ) {}

  ngOnInit(): void {
   this.condominiumId =  this.route.snapshot.paramMap.get("condominiumId")
   console.log(this.condominiumId);
   this.getCondominiumData();
    this.loadPosts();
    this.loadUsers();
  }

  onCondominiumSelected(){
    this.router.navigate(['/condominium/chat']);
  }

  loadPosts(): void {
    this.postService.getPosts().subscribe({
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
    this.userService
      .getCondominiumsUsers({ condominiumId: this.condominiumId ?? '' })
      .subscribe({
        next: (result) => {
          this.users = result;
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  getCondominiumData(): void {
    if(this.condominiumId === null) {
      console.log("ID DE CONDOMINIO NULO")
      this.router.navigate(['/condominiums']);
    }
    this.condominiumService
      .get({ condominiumId: this.condominiumId ?? '' })
      .subscribe({
        next: (result) => {
          this.condominium = result;
          console.log(this.condominium);
        },
        error: (err) => {
          console.log(err);
          this.router.navigate(['/condominiums']);
        },
      });
  }
  goHome(): void {
    this.router.navigate(['']);
  }
  goToLevels(): void {
    this.router.navigate(['/priority-levels/index',  this.condominiumId ?? '' ]);
  }
  openCreatePostModal(): void {
    console.log('Abrir modal de creación de publicaciones');
  }
}
