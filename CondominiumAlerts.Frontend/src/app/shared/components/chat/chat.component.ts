import {
  Component,
  computed,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  signal,
  viewChild,
} from '@angular/core';
import { ChatBoxComponent } from '../chat-box/chat-box.component';
import { ChatBubleComponent } from '../chat-buble/chat-buble.component';
import { NgFor } from '@angular/common';
import { ChatService } from '../../services/chat.service';
import { AutoUnsubscribe } from '../../decorators/autounsuscribe.decorator';
import {
  combineLatest,
  firstValueFrom,
  map,
  shareReplay,
  Subject,
  takeUntil,
} from 'rxjs';
import { ChatOptions } from './chat.type';
import { ChatMessageDto } from '../../../core/models/chatMessage.dto';
import { FormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { User } from '../../../core/auth/layout/auth-layout/user.type';
import { SummaryResult } from '../../../features/condominiums/models/summaryResult';
import { Dialog } from 'primeng/dialog';
import { MessageService } from 'primeng/api';
import { Toast } from 'primeng/toast';
import { SummaryStatus } from '../../../features/condominiums/models/summaryStatus.enum';
import { BackArrowComponent } from '../back-arrow/back-arrow.component';
import { CalendarComponent } from '../../../features/events/components/calendar/calendar.component';
import { ChatSignalRService } from '../../../core/services/chat-signal-r.service';

@AutoUnsubscribe()
@Component({
  selector: 'app-chat',
  standalone: true,
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
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css',
  providers: [MessageService],
})
export class ChatComponent implements OnInit, OnDestroy {
  private chatService = inject(ChatService);
  private authenticationService = inject(AuthenticationService);
  private chatSignalRService = inject(ChatSignalRService);
  private destroy$ = new Subject<void>();
  private chatContent = viewChild<ElementRef<HTMLDivElement>>('chatContent');
  show: 'chat' | 'calendar' = 'chat';
  options = signal<ChatOptions | null>(null);
  currentUser: User | null = null;
  messages = signal<ChatMessageDto[]>([]);
  summarizing = signal(false);
  summaryResult = signal<SummaryResult | null>(null);
  summaryJobId: string | null = null;
  isThereSummaryResult = signal(false);
  showSummary = false;
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
    const diffInHours =
      (now.getTime() - messageDate.getTime()) / (1000 * 60 * 60);

    return diffInHours <= 24;
  });

  // Combina los observables para manejar el estado del resumen
  summaryState$ = combineLatest([
    this.chatService.summaryStatus$,
    this.chatService.summaryResult$,
  ]).pipe(
    map(([status, result]) => ({
      status,
      result,
      canShowSummary: status === SummaryStatus.Completed && !!result?.content,
    })),
    shareReplay(1)
  );

  async ngOnInit() {
    await this.chatSignalRService.start();
    this.authenticationService.userData$
      .pipe(takeUntil(this.destroy$))
      .subscribe((userData) => {
        if (userData) this.currentUser = userData?.data;
      });

    this.chatService.chatOptions$
      .pipe(takeUntil(this.destroy$))
      .subscribe((options) => {
        this.leaveOldGroup();
        this.options.set(options);
        this.joinToGroup();

        if (options && options.type === 'condominium' && options.condominium) {
          // Cargar mensajes
          this.chatService
            .getMessagesByCondominium(options.condominium.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe((response) => {
              this.messages.set(response.data);
              this.lastMessage.set(response.data[response.data.length - 1]);
              setTimeout(() => {
                this.scrollToBottom();
              }, 200);
            });

          // Cargar el estado del resumen y el resultado al inicializar
          this.loadSummaryState();

          // Suscripción para manejar el estado del resumen
          this.summaryState$
            .pipe(takeUntil(this.destroy$))
            .subscribe(({ status, result, canShowSummary }) => {
              this.summaryStatus.set(status);
              this.summaryResult.set(result);
              this.isThereSummaryResult.set(canShowSummary);

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
                  break;
                case SummaryStatus.Completed:
                  this.summarizing.set(false);
                  break;
                default:
                  this.summarizing.set(false);
              }
            });

          // Conectar al SignalR hub si hay usuario
          if (this.currentUser?.id) {
            this.chatService
              .connectToCondominiumHub(
                options.condominium.id,
                options.condominium.name,
                this.currentUser.id
              )
              .catch((error) => {
                console.error('Error connecting to hub', error);
              });
          }
        }
      });

    this.chatSignalRService.onNewMessage
      .pipe(takeUntil(this.destroy$))
      .subscribe((message) => {
        if (message) {
          this.messages.update((prevMessages) => [...prevMessages, message]);
          this.lastMessage.set(message);
          setTimeout(() => {
            this.scrollToBottom();
          }, 200);
        }
      });
  }

  private scrollToBottom() {
    const element = this.chatContent()?.nativeElement;
    if (!element) return;

    element.scrollTo({
      top: element.scrollHeight,
      behavior: 'smooth'
    });
  }

  private joinToGroup() {
    const options = this.options();
    if (this.chatSignalRService.isHubConnected && options?.condominium) {
      this.chatSignalRService.joinToGroup(options.condominium.id);
    }

    this.chatSignalRService.onHubConnected
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (value) => {
          if (!value || !options?.condominium?.id) return;
          this.chatSignalRService.joinToGroup(options.condominium.id);
        },
      });
  }

  private leaveOldGroup() {
    const options = this.options();
    if (this.chatSignalRService.isHubConnected && options?.condominium) {
      this.chatSignalRService.leftToGroup(options.condominium.id);
    }
  }

  goTo(page: 'chat' | 'calendar') {
    this.show = page;
    if (page === 'calendar') {
      this.chatService.disconnectFromHub();
      this.showSummary = false;
    }
  }

  private loadSummaryState() {
    this.chatService.loadCurrentSummaryStatus();

    this.chatService
      .getCurrentSummaryResult()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (summary) => {
          if (summary.data?.content) {
            this.summaryResult.set(summary.data);
            this.isThereSummaryResult.set(true);
            this.summarizing.set(false);
          } else {
            this.isThereSummaryResult.set(false);
          }
        },
        error: (err) => {
          console.error('Error loading summary:', err);
          this.isThereSummaryResult.set(false);
        },
      });
  }

  formatSummary() {
    return this.chatService.formatText(this.summaryResult()?.content!);
  }

  async summarizeMessages() {
    if (!this.options() || this.options()?.type !== 'condominium') {
      return;
    }

    this.summarizing.set(true);
    const options = this.options();

    if (options && options.condominium && this.currentUser?.id) {
      try {
        await this.chatService.connectToCondominiumHub(
          options.condominium.id,
          options.condominium.name,
          this.currentUser.id
        );

        this.chatService
          .requestCondominiumSummary()
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: (response) => {
              this.summaryJobId = response.jobId;
              this.summaryStatus.set(SummaryStatus.Created);
            },
            error: (err) => {
              console.error('Error requesting summary', err);
              this.summarizing.set(false);
            },
          });
      } catch (error) {
        console.error('Error connecting to hub', error);
        this.summarizing.set(false);
      }
    }
  }

  cancelSummary() {
    if (this.summaryJobId) {
      this.chatService
        .cancelSummaryRequest(this.summaryJobId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          error: (err) => {
            console.error('Error cancelling summary', err);
          },
        });
    }
  }

  async showSummaryResult() {
    if (this.isThereSummaryResult()) {
      try {
        const summary = await firstValueFrom(
          this.chatService.getCurrentSummaryResult()
        );

        if (summary?.data?.content) {
          this.summaryResult.set(summary.data);
          this.showSummary = true;
        }
      } catch (error) {
        console.error('Error loading summary:', error);
      }
    }
  }

  async ngOnDestroy() {
    await this.chatService.disconnectFromHub();
    const options = this.options();
    if (options?.condominium?.id) {
      this.chatSignalRService.leftToGroup(options.condominium.id);
    }
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected readonly Number = Number;
}
