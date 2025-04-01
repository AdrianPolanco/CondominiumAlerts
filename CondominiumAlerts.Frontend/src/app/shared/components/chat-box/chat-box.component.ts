import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { ChatSignalRService } from '../../../core/services/chat-signal-r.service';
import { ChatOptions } from '../chat/chat.type';

@Component({
  selector: 'app-chat-box',
  imports: [FileUpload, ReactiveFormsModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css',
})
export class ChatBoxComponent {
  options = input<ChatOptions | null>(null);
  messageControl = new FormControl<string>('');

  constructor(private chatSignalRService: ChatSignalRService) {}

  async sendMessage() {
    if (!this.messageControl.value || this.messageControl.value?.length === 0)
      return;
    
    await this.chatSignalRService.sendMessage(
      this.options()?.condominium?.id!,
      this.messageControl.value
    );
    this.messageControl.reset('');
  }

  uploadImage() {
    document.getElementById('image-uploader')?.querySelector('button')!.click();
  }
}
