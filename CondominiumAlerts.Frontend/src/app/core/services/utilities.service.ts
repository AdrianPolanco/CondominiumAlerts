import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class UtilitiesService {
  private messageService = inject(MessageService);

  presentToast(text: string, success = true, duration = 3000) {
    this.messageService.add({
      detail: text,
      severity: success ? 'success' : 'error',
      life: duration,
    });
  }
}
