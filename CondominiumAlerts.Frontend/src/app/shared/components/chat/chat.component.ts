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

@AutoUnsubscribe()
@Component({
  selector: 'app-chat',
  imports: [ChatBoxComponent, ChatBubleComponent, NgFor],
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
  /*messages = signal<Message[]>([
    {
      id: '1',
      text: 'Hola, ¿cómo están todos en el condominio?',
      creatorUserId: 'user1',
      condominiumId: '123',
      createdAt: new Date('2025-03-12T12:00:00Z'),
      updatedAt: new Date('2025-03-12T12:00:00Z'),
    },
    {
      id: '2',
      text: 'Todo bien, gracias. ¿Y tú?',
      creatorUserId: 'Z2jPvGKvZ6XIhVPLUhisYkJDPb93',
      condominiumId: '123',
      createdAt: new Date('2025-03-12T12:02:00Z'),
      updatedAt: new Date('2025-03-12T12:02:00Z'),
    },
    {
      id: '3',
      text: 'Bien, gracias. ¿Alguna novedad en la junta de vecinos?',
      creatorUserId: 'user1',
      condominiumId: '123',
      createdAt: new Date('2025-03-12T12:05:00Z'),
      updatedAt: new Date('2025-03-12T12:05:00Z'),
    },
    {
      id: '4',
      text: 'Sí, habrá una reunión el viernes a las 7 PM en el salón comunal.',
      creatorUserId: 'Z2jPvGKvZ6XIhVPLUhisYkJDPb93',
      condominiumId: '123',
      createdAt: new Date('2025-03-12T12:07:00Z'),
      updatedAt: new Date('2025-03-12T12:07:00Z'),
    },
    {
      id: '5',
      text: 'Perfecto, estaré ahí. Gracias por avisar.',
      creatorUserId: 'user1',
      condominiumId: '123',
      createdAt: new Date('2025-03-12T12:08:00Z'),
      updatedAt: new Date('2025-03-12T12:08:00Z'),
    },
  ]);*/

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

    effect(() => {
      console.log("MESSAGES", this.messages())
    });
  }
  ngOnDestroy(): void {}
}
