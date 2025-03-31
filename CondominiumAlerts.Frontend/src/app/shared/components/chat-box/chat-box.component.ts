import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { ChatSignalRService } from '../../../core/services/chat-signal-r.service';

@Component({
  selector: 'app-chat-box',
  imports: [FileUpload, ReactiveFormsModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css',
})
export class ChatBoxComponent {
  messageControl = new FormControl();
  constructor(private chatSignalRService: ChatSignalRService) {}

  sendMessage() {
    console.log(this.messageControl.value);
    this.messageControl.reset('');
  }

  uploadImage() {
    document.getElementById('image-uploader')?.querySelector('button')!.click();
  }
}
