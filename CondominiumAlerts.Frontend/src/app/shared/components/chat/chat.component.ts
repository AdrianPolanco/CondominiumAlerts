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
import { Button } from 'primeng/button';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { User } from '../../../core/auth/layout/auth-layout/user.type';
import { SplitterModule } from 'primeng/splitter';


@AutoUnsubscribe()
@Component({
  selector: 'app-chat',
  imports: [ChatBoxComponent, ChatBubleComponent, NgFor, FormsModule, Button, SplitterModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  private chatService = inject(ChatService);
  private authenticationService = inject(AuthenticationService);

  private destroy$ = new Subject<void>();
  options = signal<ChatOptions | null>(null);
  currentUser: User | null = null;
  messages = signal<ChatMessageDto[]>([]);
  summarizing = signal(false)
  summaryResult = signal<string | null>(null);
  summaryJobId: string | null = null;

  ngOnInit(): void {
    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData) this.currentUser = userData?.data;
    });
    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Actualizar la opción de chat actual
      this.options.set(options);
      
      // Reaccionar a nuevas opciones de chat
      if (options && options.type === 'condominium' && options.condominium) {
        console.log("COND FROM SUBSCRIPTION", options.condominium.id);
        this.chatService
          .getMessagesByCondominium(options.condominium.id)
          .pipe(takeUntil(this.destroy$))
          .subscribe((response) => {
            console.log(response);
            this.messages.set(response.data);
          });
      }
    });
  }

  async summarizeMessages() {
    console.log("CURRENT OPTIONS", this.options());
    if(!this.options()) {
      console.log("SUMMARIZEMESSAGES NO EJECUTADO: NO HAY OPCIONES");
      return};
    if(this.options()?.type !== 'condominium') {
      console.log("SUMMARIZEMESSAGES NO EJECUTADO: NO ES UN CONDOMINIO");
      return
    };

    this.summarizing.set(true);

    const options = this.options();

    if (options && options.condominium && this.currentUser?.id) {
      console.log("SUMMARIZEMESSAGES EJECUTANDOSE");
      try {
          await this.chatService
            .connectToCondominiumHub(options.condominium.id, options.condominium.name, this.currentUser.id)
      } catch (error) {
        console.log("ERROR AL CONECTAR CON EL HUB", error);
        this.summarizing.set(false);
      }



      console.log("CONEXION SIGNALR ESTABLECIDA EXITOSAMENTE");
        // Tras conectarse, se suscribe a los mensajes
      this.chatService.processingStatus$
          .pipe(takeUntil(this.destroy$))
          .subscribe(status => {
            if (status === null) this.summarizing.set(false); 
          });
        
        // Suscribirse a los resultados del resumen
        const summaryResultSubscription = this.chatService.summaryResult$
          .pipe(takeUntil(this.destroy$))
          .subscribe(summary => {
            if(!summary) return;
            this.summaryResult.set(summary);
            summaryResultSubscription.unsubscribe();
            this.chatService.disconnectFromHub();
          });
        
        // Subscrirse a los errores
        const processingErrorSubscription = this.chatService.processingError$
          .pipe(takeUntil(this.destroy$))
          .subscribe(error => {
            if (error) {
              // Handle error (could add error handling state to your component)
              this.summarizing.set(false);
              processingErrorSubscription.unsubscribe();
              this.chatService.disconnectFromHub();
              console.log('Error processing summary', error);
            }
          });
        
        // Solicitar el resumen
        this.chatService.requestCondominiumSummary()
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (response) => {
              console.log('Summary requested successfully', response);
              this.summaryJobId = response.jobId;
            },
            error: (err) => {
              console.error('Error requesting summary', err);
              this.summarizing.set(false);
              this.chatService.disconnectFromHub();
            }
          });
        }

        console.log("RESUMEN SOLICITADO")
      
  }

  cancelSummary() {
    if (this.summaryJobId) {
      this.chatService.cancelSummaryRequest(this.summaryJobId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            console.log('Summary cancelled');
            this.summarizing.set(false);
            this.summaryJobId = null;
            
            // Desconectar del hub después de cancelar
            this.chatService.disconnectFromHub();
          },
          error: (err) => {
            console.error('Error cancelling summary', err);
          }
        });
    }
  }

  ngOnDestroy(): void {
    if (this.summarizing()) this.chatService.disconnectFromHub();
  }

}
