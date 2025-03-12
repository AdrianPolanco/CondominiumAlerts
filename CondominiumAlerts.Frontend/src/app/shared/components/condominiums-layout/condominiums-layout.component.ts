import { Component, inject } from '@angular/core';
import { NgFor, CommonModule, NgOptimizedImage } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Toolbar } from 'primeng/toolbar';
import { Button } from 'primeng/button';
import { getCondominiumsUsersResponse } from '../../../features/users/models/user.model';
import { UserService } from '../../../features/users/services/user.service';
import { AuthService } from '../../../core/auth/services/auth.service';

@Component({
  selector: 'app-condominiums-layout',
  imports: [Toolbar, NgFor, CommonModule, Button, NgOptimizedImage],
  templateUrl: './condominiums-layout.component.html',
  styleUrl: './condominiums-layout.component.css',
})
export class CondominiumsLayoutComponent {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private router = inject(Router);
  currentUser = this.userService.currentCondominiumUserActive;

  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  getLoggedUsername() {
    return this.authService.currentUser?.displayName;
  }
  goHome(): void {
    this.router.navigate(['']);
  }

  goToChat(user: getCondominiumsUsersResponse): void {
    console.log(user);
    this.userService.setCurrentCondominiumUser(user);
    this.router.navigate(['condominium/chat']);
  }

  users: Array<getCondominiumsUsersResponse> = [
    {
      id: 'dddddfsdfdfs',
      fullName: 'Juan Pérez',
      email: 'En línea',
      profilePictureUrl:
        'https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?t=st=1741553994~exp=1741557594~hmac=6790b2380695298926314ad92146ef39a5664838888864b60616996a0850ffdc&w=740',
    },
  ];
}
