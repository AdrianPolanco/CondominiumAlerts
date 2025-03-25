import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, CommonModule, NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';
import { Toolbar } from 'primeng/toolbar';
import { Button } from 'primeng/button';
import { GetCondominiumsUsersResponse } from '../../../features/users/models/user.model';
import { UserService } from '../../../features/users/services/user.service';
import { AuthService } from '../../../core/auth/services/auth.service';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { AutoUnsubscribe } from '../../decorators/autounsuscribe.decorator';

import { CondominiumService } from '../../../features/condominiums/services/condominium.service';
import { User } from '../../../core/auth/layout/auth-layout/user.type';
import { GetCondominiumsJoinedByUserResponse } from "../../../features/condominiums/models/getCondominiumsJoinedByUser.response";
import { ChatService } from '../../services/chat.service';
import { Condominium } from '../../../features/condominiums/models/condominium.model';
import { isUser } from '../../helpers/isUser.helper';
import { Dialog } from 'primeng/dialog';
import { DrawerModule } from 'primeng/drawer';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@AutoUnsubscribe()
@Component({
  selector: 'app-condominiums-layout',
  imports: [Toolbar, NgFor, CommonModule, Button, NgOptimizedImage, Dialog, DrawerModule],
  templateUrl: './condominiums-layout.component.html',
  styleUrl: './condominiums-layout.component.css',
})
export class CondominiumsLayoutComponent implements OnInit {
  private authService = inject(AuthService);
  private condominiumService = inject(CondominiumService)
  private authenticationService = inject(AuthenticationService);
  private chatService = inject(ChatService);
  private router = inject(Router);
  currentUser: User | null = null;
  private condominiumsSubject = new BehaviorSubject<GetCondominiumsJoinedByUserResponse[]>([]);
  condominiums$ = this.condominiumsSubject.asObservable();
  currentCondominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null = null;
  private destroy$ = new Subject<void>();
  areCondominiumsLoading = signal(true)
  showNotifications = false;
  showDrawer = false;
  private breakpointObserver = inject(BreakpointObserver);
  isMobile = false;

  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  ngOnInit(): void {
    this.breakpointObserver.observe([Breakpoints.Handset])
    .pipe(takeUntil(this.destroy$))
    .subscribe(result => {
      this.isMobile = result.matches;
    });

    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      this.currentUser = userData?.data!;
      // Solo llamamos getUserCondominiums() cuando this.currentUser está definido
      if (this.currentUser?.id) this.getUserCondominiums(); 
    });
  }

  showNotificationsDialog(): void {
    this.showNotifications = true;
  }

  showDrawerDialog(): void {
    this.showDrawer = true;
  }

  getLoggedUsername() {
    return this.authService.currentUser?.displayName;
  }

  async goHome() {
    await this.authenticationService.logOut();
    this.router.navigate(['/home']);
  }

  getUserCondominiums(): void {
    console.log("USERID", this.currentUser?.id)
    this.condominiumService.getCondominiumsJoinedByUser({userId: this.currentUser?.id!}).pipe(takeUntil(this.destroy$)).subscribe((response) => {
      this.condominiumsSubject.next(response.data);
      this.areCondominiumsLoading.set(false);
    });
  }

  goToChat(data: User | Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'>): void {
    console.log(data);
    //this.userService.setCurrentCondominiumUser(user);
    const type = isUser(data) ? 'user' : 'condominium';

    this.chatService.setChatOptions({
      type,
      user: isUser(data) ? data as User : null,
      condominium: isUser(data) ? null : data as Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'>,
    })

    this.router.navigate(['condominium/chat']);
  }



  onCondominiumSelected(condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null): void {
    if(condominium) {
      console.log("ACTUALIZANDO CHAT OPTIONS")
      this.chatService.setChatOptions({ type: 'condominium', condominium, user: null });}
    this.currentCondominium = condominium;
  }

  users: Array<GetCondominiumsUsersResponse> = [
    {
      id: 'dddddfsdfdfs',
      fullName: 'Juan Pérez',
      email: 'En línea',
      profilePictureUrl:
        'https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg?t=st=1741553994~exp=1741557594~hmac=6790b2380695298926314ad92146ef39a5664838888864b60616996a0850ffdc&w=740',
    },
  ];
}
