import { Component, inject, OnDestroy, OnInit, output, signal } from '@angular/core';
import { Button } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { Condominium } from '../../../features/condominiums/models/condominium.model';
import { ChatService } from '../../services/chat.service';
import { AsyncPipe, NgClass, NgFor, NgOptimizedImage, TitleCasePipe } from '@angular/common';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { GetCondominiumsJoinedByUserResponse } from '../../../features/condominiums/models/getCondominiumsJoinedByUser.response';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { User } from '../../../core/auth/layout/auth-layout/user.type';
import { CondominiumService } from '../../../features/condominiums/services/condominium.service';
import { AutoUnsubscribe } from '../../decorators/autounsuscribe.decorator';

@AutoUnsubscribe()
@Component({
  selector: 'shared-chats-drawer',
  imports: [Button, DrawerModule, NgFor, NgClass, NgOptimizedImage, AsyncPipe, TitleCasePipe],
  templateUrl: './chats-drawer.component.html',
  styleUrl: './chats-drawer.component.css'
})
export class ChatsDrawerComponent implements OnInit, OnDestroy{

  areCondominiumsLoading = signal(true)
  showDrawer = false;
  private condominiumsSubject = new BehaviorSubject<GetCondominiumsJoinedByUserResponse[]>([]);
  condominiums$ = this.condominiumsSubject.asObservable();
  currentCondominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null = null;
  private condominiumService = inject(CondominiumService)
  private chatService = inject(ChatService);
  private authenticationService = inject(AuthenticationService);
  private destroy$ = new Subject<void>();
  currentUser: User | null = null;
  onCondominiumSelected = output<Pick<Condominium, 'id' | 'name' | 'imageUrl' | 'address'>>();

  ngOnInit(): void {
    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      this.currentUser = userData?.data!;
      // Solo llamamos getUserCondominiums() cuando this.currentUser está definido
      if (this.currentUser?.id) this.getUserCondominiums(); 
    });
  }

  showDrawerDialog(): void {
    this.showDrawer = true;
  }

  getUserCondominiums(): void {
    console.log("USERID", this.currentUser?.id)
    this.condominiumService.getCondominiumsJoinedByUser({userId: this.currentUser?.id!}).pipe(takeUntil(this.destroy$)).subscribe((response) => {
      this.condominiumsSubject.next(response.data);
      this.areCondominiumsLoading.set(false);
    });
  }

  emitCondominium(condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null): void {
      if(condominium) {
        this.currentCondominium = condominium;
        this.chatService.setChatOptions({ type: "condominium", condominium, user: null})
        this.onCondominiumSelected.emit(condominium);
      } 
  }

  ngOnDestroy(): void {
    this.condominiumsSubject.complete();
  }
}
