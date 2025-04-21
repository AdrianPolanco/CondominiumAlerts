import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { ChatMessageDto } from '../../../core/models/chatMessage.dto';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { Subject, takeUntil } from 'rxjs';
import { User } from '../../../core/auth/layout/auth-layout/user.type';

@Component({
  selector: 'app-chat-buble',
  imports: [NgClass],
  templateUrl: './chat-buble.component.html',
  styleUrl: './chat-buble.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChatBubleComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
  private destroy$ = new Subject<void>();
  private authenticationService = inject(AuthenticationService);
  message = input<ChatMessageDto>();

  ngOnInit(): void {
    this.authenticationService.userData$
      .pipe(takeUntil(this.destroy$))
      .subscribe((userData) => {
        if (userData?.data) {
          this.currentUser = userData.data;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get getCurrentUserId() {
    return this.currentUser?.id ?? '';
  }

  formatDate(date: Date): string {
    const options: Intl.DateTimeFormatOptions = {
      hour: '2-digit',
      minute: '2-digit',
    };
    const formattedDate = new Date(date).toLocaleTimeString([], options);
    return formattedDate;
  }

  getUserNameById(uid: string | undefined) {
    return 'Usuario interno';
  }

  isCurrentUser = computed(
    () =>
      (this.message()?.creatorUser?.id || this.message()?.creatorUserId) ===
      this.getCurrentUserId
  );
}
