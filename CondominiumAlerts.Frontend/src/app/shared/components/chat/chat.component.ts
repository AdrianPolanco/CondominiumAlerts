import { Component, inject, input, OnDestroy, OnInit } from '@angular/core';
import { ChatBoxComponent } from '../chat-box/chat-box.component';
import { ChatBubleComponent } from '../chat-buble/chat-buble.component';
import { UserService } from '../../../features/users/services/user.service';

@Component({
  selector: 'app-chat',
  imports: [ChatBoxComponent, ChatBubleComponent],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  private userService = inject(UserService);
  currentUser = this.userService.currentCondominiumUserActive;

  ngOnInit(): void {}
  ngOnDestroy(): void {}
}
