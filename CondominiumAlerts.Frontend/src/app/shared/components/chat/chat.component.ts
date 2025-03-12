import {
  Component,
  inject,
  input,
  OnDestroy,
  OnInit,
  signal,
} from '@angular/core';
import { ChatBoxComponent } from '../chat-box/chat-box.component';
import { ChatBubleComponent } from '../chat-buble/chat-buble.component';
import { UserService } from '../../../features/users/services/user.service';
import { Message } from '../../../core/models/index.models';
import { NgFor } from '@angular/common';
import { AuthService } from '../../../core/auth/services/auth.service';

@Component({
  selector: 'app-chat',
  imports: [ChatBoxComponent, ChatBubleComponent, NgFor],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  currentUser = this.userService.currentCondominiumUserActive;

  messages = signal<Message[]>([
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
  ]);

  ngOnInit(): void {}
  ngOnDestroy(): void {}
}
