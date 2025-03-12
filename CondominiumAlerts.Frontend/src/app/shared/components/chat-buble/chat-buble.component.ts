import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
} from '@angular/core';
import { Message } from '../../../core/models/index.models';
import { AuthService } from '../../../core/auth/services/auth.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-chat-buble',
  imports: [NgClass],
  templateUrl: './chat-buble.component.html',
  styleUrl: './chat-buble.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChatBubleComponent {
  authService = inject(AuthService);
  message = input<Message>();

  get getCurrentUserId() {
    return this.authService.currentUser?.uid;
  }

  formatDate(date: Date): string {
    const options: Intl.DateTimeFormatOptions = {
      hour: '2-digit',
      minute: '2-digit',
    };
    return date.toLocaleTimeString([], options);
  }

  getUserNameById(uid: string | undefined) {
    return ' Juan Pérez';
  }

  isCurrentUser = computed(
    () => this.message()?.creatorUserId === this.getCurrentUserId
  );
}
