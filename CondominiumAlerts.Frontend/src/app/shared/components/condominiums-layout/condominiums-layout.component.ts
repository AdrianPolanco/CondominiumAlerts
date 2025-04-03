import { Component, inject, input, OnInit, signal } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';
import { GetCondominiumsUsersResponse } from '../../../features/users/models/user.model';
import { AuthService } from '../../../core/auth/services/auth.service';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { AutoUnsubscribe } from '../../decorators/autounsuscribe.decorator';
import { CondominiumService } from '../../../features/condominiums/services/condominium.service';
import { User } from '../../../core/auth/layout/auth-layout/user.type';
import { GetCondominiumsJoinedByUserResponse } from "../../../features/condominiums/models/getCondominiumsJoinedByUser.response";
import { ChatService } from '../../services/chat.service';
import { Condominium } from '../../../features/condominiums/models/condominium.model';
import { DrawerModule } from 'primeng/drawer';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ChatsDrawerComponent } from "../chats-drawer/chats-drawer.component";
import { UserOptionsComponent } from "../../../core/auth/layout/user-options/user-options.component";

@AutoUnsubscribe()
@Component({
  selector: 'shared-condominiums-layout',
  imports: [
    CommonModule, 
    DrawerModule, 
    ChatsDrawerComponent, 
    UserOptionsComponent
  ],
  templateUrl: './condominiums-layout.component.html',
  styleUrl: './condominiums-layout.component.css',
})
export class CondominiumsLayoutComponent implements OnInit {
  private condominiumService = inject(CondominiumService)
  private authService = inject(AuthService);
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

  showChatDrawer = input(false);

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

    this.chatService.chatOptions$.pipe().subscribe((chatOptions) => {
      if (chatOptions) {
        this.currentCondominium = chatOptions.condominium;
      }
    });
  }

  showNotificationsDialog(): void {
    this.showNotifications = true;
  }

  getLoggedUsername() {
    return this.authService.currentUser?.displayName;
  }

  async goHome() {
    await this.authenticationService.logOut();
    this.router.navigate(['/home']);
  }

  onCondominiumSelected(condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null): void {
    console.log("ACTUALIZANDO CHAT OPTIONS")
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
