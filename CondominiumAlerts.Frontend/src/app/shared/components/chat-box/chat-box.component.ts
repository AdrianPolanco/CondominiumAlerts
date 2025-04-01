import { Component } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { SignalRService } from '../../../core/services/signal-r.service';

@Component({
  selector: 'app-chat-box',
  imports: [FileUpload, ReactiveFormsModule],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css',
})
export class ChatBoxComponent {
  messageControl = new FormControl();
  constructor(private signalRService: SignalRService) {}

  sendMessage() {
    console.log(this.messageControl.value);
    this.messageControl.reset('');
  }

  uploadImage() {
    document.getElementById('image-uploader')?.querySelector('button')!.click();
  }
}
