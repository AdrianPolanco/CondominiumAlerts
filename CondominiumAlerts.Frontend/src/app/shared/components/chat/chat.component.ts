import {Component, computed, inject, OnDestroy, OnInit, signal,} from '@angular/core';
import {ChatBoxComponent} from '../chat-box/chat-box.component';
import {ChatBubleComponent} from '../chat-buble/chat-buble.component';
import {NgClass, NgFor, NgIf} from '@angular/common';
import {ChatService} from '../../services/chat.service';
import {AutoUnsubscribe} from '../../decorators/autounsuscribe.decorator';
import {combineLatest, firstValueFrom, map, shareReplay, Subject, takeUntil} from 'rxjs';
import {ChatOptions} from './chat.type';
import {ChatMessageDto} from '../../../core/models/chatMessage.dto';
import {FormsModule} from '@angular/forms';
import {Button} from 'primeng/button';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {User} from '../../../core/auth/layout/auth-layout/user.type';
import {SummaryResult} from '../../../features/condominiums/models/summaryResult';
import {Dialog} from 'primeng/dialog';
import {MessageService} from 'primeng/api';
import {Toast} from 'primeng/toast';
import {SummaryStatus} from '../../../features/condominiums/models/summaryStatus.enum';
import { BackArrowComponent } from "../back-arrow/back-arrow.component";
import { CalendarComponent } from "../../../features/events/components/calendar/calendar.component";


@AutoUnsubscribe()
@Component({
  selector: 'app-chat',
  imports: [
    ChatBoxComponent,
    ChatBubleComponent,
    NgFor,
    FormsModule,
    Button,
    Toast,
    Dialog,
    BackArrowComponent,
    CalendarComponent,
    NgClass
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
  providers: [MessageService]
})
export class ChatComponent implements OnInit, OnDestroy {
  private chatService = inject(ChatService);
  private authenticationService = inject(AuthenticationService);
  private destroy$ = new Subject<void>();
  show: "chat" | "calendar" = "chat";
  options = signal<ChatOptions | null>(null);
  currentUser: User | null = null;
  messages = signal<ChatMessageDto[]>([]);
  summarizing = signal(false)
  summaryResult = signal<SummaryResult | null>(null);
  summaryJobId: string | null = null;
  isThereSummaryResult = signal(false);
  showSummary = false
  summaryStatus = signal<SummaryStatus | null>(null);
  SummaryStatusEnum = SummaryStatus;
  lastMessage = signal<ChatMessageDto | null>(null);
  wasLastMessageToday = computed(() => {
    const message = this.lastMessage();
    if (!message || !message.createdAt) {
      return false;
    }

    const messageDate = new Date(message.createdAt);
    const now = new Date();
    const diffInHours = (now.getTime() - messageDate.getTime()) / (1000 * 60 * 60);

    return diffInHours <= 24
  });
 // Combina los observables para manejar el estado del resumen
 summaryState$ = combineLatest([
    this.chatService.summaryStatus$,
    this.chatService.summaryResult$
  ]).pipe(
    map(([status, result]) => ({
      status,
      result,
      canShowSummary: status === SummaryStatus.Completed && !!result?.content
    })),
    shareReplay(1)
  );

  ngOnInit(): void {
    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData) this.currentUser = userData?.data;
      console.log("TOKEN", this.currentUser);
    });

    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Actualizar la opción de chat actual
      this.options.set(options);

      // Reaccionar a nuevas opciones de chat
      if (options && options.type === 'condominium' && options.condominium) {
        console.log("COND FROM SUBSCRIPTION", options.condominium.id);

        // Cargar mensajes
        this.chatService
          .getMessagesByCondominium(options.condominium.id)
          .pipe(takeUntil(this.destroy$))
          .subscribe((response) => {
            console.log("MESSAGES RECOVERED: ", response);
            this.messages.set(response.data);
            this.lastMessage.set(response.data[response.data.length - 1]);
            console.log("LAST MESSAGE", this.lastMessage())
          });

        // Cargar el estado del resumen y el resultado al inicializar
        this.loadSummaryState();

        // Suscribirse a actualizaciones
         // Suscripción para manejar el estado del resumen
    this.summaryState$
    .pipe(takeUntil(this.destroy$))
    .subscribe(({ status, result, canShowSummary }) => {
      // Actualiza las señales basadas en el estado
      this.summaryStatus.set(status);
      this.summaryState$
      .pipe(takeUntil(this.destroy$))
      .subscribe(({ status, result, canShowSummary }) => {
        // Comprehensive state management
        this.summaryStatus.set(status);
          switch (status) {
            case SummaryStatus.Cancelled:
              this.summarizing.set(false);
              this.summaryResult.set(null);
              this.summaryJobId = null;
              break;
            case SummaryStatus.Processing:
            case SummaryStatus.Created:
            case SummaryStatus.Queued:
              this.summarizing.set(true);
              this.isThereSummaryResult.set(false);
              break;
            case SummaryStatus.Completed:
              this.summarizing.set(false);
              this.summaryResult.set(result);
              this.isThereSummaryResult.set(canShowSummary);
              break;
            default:
              this.summarizing.set(false);
              this.isThereSummaryResult.set(false);
          }
      }
    );
    });

        // Conectar automáticamente al SignalR hub para recibir actualizaciones en tiempo real
        if (this.currentUser?.id) {
          this.chatService.connectToCondominiumHub(
            options.condominium.id,
            options.condominium.name,
            this.currentUser.id
          ).catch(error => {
            console.error("Error connecting to hub during initialization", error);
          });
        }

      }
    });

  }

  goTo(page: "chat" | "calendar"){
    this.show = page;
    if(page === "calendar") {
      this.chatService.disconnectFromHub();
      this.showSummary = false;
      console.log("CHATOPTIONS FROM CALENDAR", this.options());
    }
  }

  // Nuevo método para cargar el estado del resumen
  private loadSummaryState() {
    // Cargar estado
    this.chatService.loadCurrentSummaryStatus();

    // Cargar resultado si existe
    this.chatService.getCurrentSummaryResult()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (summary) => {
          console.log("LOADED SUMMARY", summary);
          if (summary.data?.content) {
            console.log("SUMMARY DATA", summary.data);
            this.summaryResult.set(summary.data);
            this.isThereSummaryResult.set(true);
            this.summarizing.set(false);

            console.log("ISTHERESUMMARY", this.isThereSummaryResult());
          } else {
            this.isThereSummaryResult.set(false);
          }
        },
        error: (err) => {
          console.error("Error loading summary:", err);
          this.isThereSummaryResult.set(false);
        }
      });
  }

  formatSummary(){
    return this.chatService.formatText(this.summaryResult()?.content!);
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

        // Tras conectarse, se suscribe a los mensajes
      this.chatService.processingStatus$
          .pipe(takeUntil(this.destroy$))
          .subscribe(status => {
            if(status === "COMPLETED") this.summarizing.set(false);
            //if (status === null) this.summarizing.set(false);
          });

        // Suscribirse a los resultados del resumen
        this.chatService.summaryResult$
          .pipe(takeUntil(this.destroy$))
          .subscribe(async(summary) => {
            if(!summary) return;
            this.summaryResult.set(summary);
            this.isThereSummaryResult.set(true);
            console.log("HAY SUMMARYRESULT", this.summaryResult());
            console.log("SUMMARIZERESULT EJECUTADO", this.summaryResult());
           // summaryResultSubscription.unsubscribe();
            //await this.chatService.disconnectFromHub();
          });

        // Subscrirse a los errores
        const processingErrorSubscription = this.chatService.processingError$
          .pipe(takeUntil(this.destroy$))
          .subscribe(async(error) => {
            if (error) {
              // Handle error (could add error handling state to your component)
              //this.summarizing.set(false);
              processingErrorSubscription.unsubscribe();
              //await this.chatService.disconnectFromHub();
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

              this.summarizing.set(true);
              this.summaryStatus.set(SummaryStatus.Created);
              this.isThereSummaryResult.set(false);
            },
            error: (err) => {
              console.error('Error requesting summary', err);
              this.summarizing.set(false);
            }
          });
        }
  }

  cancelSummary() {
    if (this.summaryJobId) {
      console.log("CANCELLING SUMMARY...")
      console.log("JOBID", this.summaryJobId)
      this.chatService.cancelSummaryRequest(this.summaryJobId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
           /* this.summarizing.set(false);
            this.summaryJobId = null;*/
            //await this.chatService.disconnectFromHub();
          },
          error: (err) => {
            console.error('Error cancelling summary', err);
          }
        });
    }
  }

  async showSummaryResult(){
    if (this.isThereSummaryResult()) {
      try {
        // Siempre intentar cargar el resumen fresco desde el servidor
        const summary = await firstValueFrom(this.chatService.getCurrentSummaryResult());

        if (summary && summary.data?.content) {
          // Actualiza el resumen local con los datos del servidor
          this.summaryResult.set(summary.data);
          // Muestra el modal con el contenido actualizado
          this.showSummary = true;
        } else {
          console.error('No se pudo obtener el contenido del resumen');
        }
      } catch (error) {
        console.error('Error al cargar el resumen:', error);
      }
    }
  }

  async ngOnDestroy() {
    await this.chatService.disconnectFromHub()
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected readonly Number = Number;
}
