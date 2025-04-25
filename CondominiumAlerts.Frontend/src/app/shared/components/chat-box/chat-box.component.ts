import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FileUpload } from 'primeng/fileupload';
import { ChatSignalRService } from '../../../core/services/chat-signal-r.service';
import { ChatOptions } from '../chat/chat.type';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';
import { MessageService } from 'primeng/api';
import { NgIf } from '@angular/common';
import { AuthenticationService } from '../../../core/services/authentication.service';

@Component({
  selector: 'app-chat-box',
  imports: [FileUpload, ReactiveFormsModule, NgIf],
  templateUrl: './chat-box.component.html',
  styleUrl: './chat-box.component.css',
})
export class ChatBoxComponent {
  options = input<ChatOptions | null>(null);
  messageControl = new FormControl<string>('');
  selectedImageFile: File | null = null;
  imagePreviewUrl: string | null = null;
  token: string | null = null;


  constructor(private chatSignalRService: ChatSignalRService, private http: HttpClient, private messageService: MessageService, private authenticationService: AuthenticationService) {
    this.authenticationService.userToken$.subscribe((token) => {
      if (token) {
        this.token = token
      }
    });
  }

  sendMessage() {
    if (
      (!this.messageControl.value || this.messageControl.value.trim().length === 0) &&
      !this.selectedImageFile
    )
      return;

    let upload$ = of(null as string | null);

    if (this.selectedImageFile) {
      const formData = new FormData();
      formData.append('file', this.selectedImageFile);

      upload$ = this.http.post<{isSuccess: boolean, data: { imageUrl: string|null }}>('/api/chat/image', formData, {
        headers: {
          Authorization: `Bearer ${this.token}`,
        },
      }).pipe(
        map(res => res?.data?.imageUrl)
      );
    }

    upload$.subscribe({
      next: async (imageUrl) => {
        await this.chatSignalRService.sendMessage(
          this.options()?.condominium?.id!,
          this.messageControl.value || '',
          undefined,
          imageUrl || undefined
        );
        this.messageControl.reset('');
        this.removeImage();
      },
      error: (err) => {
        console.error('Error al subir la imagen:', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al enviar el mensaje' });
      }
    });
  }

  onImageSelected(event: any) {
    const file = event.files?.[0];
    if (!file) return;

    this.selectedImageFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreviewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  removeImage() {
    this.selectedImageFile = null;
    this.imagePreviewUrl = null;
  }

  uploadImage() {
    document.getElementById('image-uploader')?.querySelector('button')!.click();
  }
}
