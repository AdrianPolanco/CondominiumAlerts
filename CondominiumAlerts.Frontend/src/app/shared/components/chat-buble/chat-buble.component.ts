import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
} from '@angular/core';
import { Message } from '../../../core/models/message.models';
import { AuthService } from '../../../core/auth/services/auth.service';
import { NgClass } from '@angular/common';
import { ChatMessageDto } from '../../../core/models/chatMessage.dto';

@Component({
  selector: 'app-chat-buble',
  imports: [NgClass],
  templateUrl: './chat-buble.component.html',
  styleUrl: './chat-buble.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChatBubleComponent {
  authService = inject(AuthService);
  message = input<ChatMessageDto>();

  get getCurrentUserId() {
    return this.authService.currentUser?.uid;
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
    return ' Juan Pérez';
  }

  isCurrentUser = computed(
    () => this.message()?.creatorUser.id === this.getCurrentUserId
  );
}
