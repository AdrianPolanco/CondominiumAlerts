import {
  Component,
  effect,
  inject,
  input,
  OnDestroy,
  OnInit,
  signal,
} from '@angular/core';
import { ChatBoxComponent } from '../chat-box/chat-box.component';
import { ChatBubleComponent } from '../chat-buble/chat-buble.component';
import { UserService } from '../../../features/users/services/user.service';
import { Message } from '../../../core/models/message.models';
import { NgFor } from '@angular/common';
import { AuthService } from '../../../core/auth/services/auth.service';
import { ChatService } from '../../services/chat.service';
import { AutoUnsubscribe } from '../../decorators/autounsuscribe.decorator';
import { Subject, takeUntil } from 'rxjs';
import { ChatOptions } from './chat.type';
import { ChatMessageDto } from '../../../core/models/chatMessage.dto';
import { FormsModule } from '@angular/forms';
import { ToggleButton } from 'primeng/togglebutton';
import { Button } from 'primeng/button';

@AutoUnsubscribe()
@Component({
  selector: 'app-chat',
  imports: [ChatBoxComponent, ChatBubleComponent, NgFor, FormsModule, Button],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  private chatService = inject(ChatService);
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private messageService = inject(ChatService);
  private destroy$ = new Subject<void>();
  options = signal<ChatOptions | null>(null);
  currentUser = this.userService.currentCondominiumUserActive;
  messages = signal<ChatMessageDto[]>([]);
  summarizing = signal(false)

  ngOnInit(): void {
    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Update the local signal
      this.options.set(options);
      
      // React to the new options value immediately
      if (options && options.type === 'condominium' && options.condominium) {
        console.log("COND FROM SUBSCRIPTION", options.condominium.id);
        this.messageService
          .getMessagesByCondominium(options.condominium.id)
          .pipe(takeUntil(this.destroy$))
          .subscribe((response) => {
            console.log(response);
            this.messages.set(response.data);
          });
      }
    });
  }

  summarizeMessages() {
    this.summarizing.set(!this.summarizing());
  }

  ngOnDestroy(): void {}
}
