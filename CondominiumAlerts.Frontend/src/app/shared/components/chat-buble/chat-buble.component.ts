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
import { ChatCreatorUserDto } from '../../../core/models/chatCreatorUser.dto';
import { DateEsPipe } from "../../pipes/date-es.pipe";

@Component({
  selector: 'app-chat-buble',
  imports: [NgClass, DateEsPipe],
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

  getUserNameById(chatCreatorUserDto: ChatCreatorUserDto) {
    return chatCreatorUserDto?.username;
  }

  isCurrentUser = computed(
    () =>
      (this.message()?.creatorUser?.id || this.message()?.creatorUserId) ===
      this.getCurrentUserId
  );
}
