import {HttpClient} from '@angular/common/http';
import {inject, Injectable, OnDestroy} from '@angular/core';
import {BehaviorSubject, catchError, Observable, Subject, takeUntil, tap, throwError} from 'rxjs';
import {ChatOptions} from '../components/chat/chat.type';
import {ChatMessageDto} from '../../core/models/chatMessage.dto';
import * as signalR from '@microsoft/signalr';
import {HttpTransportType, HubConnectionBuilder} from '@microsoft/signalr';
import {AuthenticationService} from '../../core/services/authentication.service';
import {AutoUnsubscribe} from '../decorators/autounsuscribe.decorator';
import {User} from '../../core/auth/layout/auth-layout/user.type';
import {RequestCondominiumSummaryResponse} from '../../features/condominiums/models/requestCondominiumSummary.response';
import {SummaryResult} from '../../features/condominiums/models/summaryResult';
import {MessageService} from 'primeng/api';
import {SummaryStatus} from '../../features/condominiums/models/summaryStatus.enum';
import { GetSummaryStatusResponse } from '../../features/condominiums/models/getSummaryStatus.response';

@AutoUnsubscribe()
@Injectable({
  providedIn: 'root',
})
export class ChatService implements OnDestroy{

  private chatOptions = new BehaviorSubject<ChatOptions | null>(null);
  chatOptions$ = this.chatOptions.asObservable();

  private hubConnection: signalR.HubConnection | null = null;
  private processingStatus = new BehaviorSubject<string | null>(null);
  private summaryStatus = new BehaviorSubject<SummaryStatus|null>(null);
  private summaryResult = new BehaviorSubject<SummaryResult | null>(null);
  private processingError = new BehaviorSubject<string | null>(null);

  processingStatus$ = this.processingStatus.asObservable();
  summaryResult$ = this.summaryResult.asObservable();
  processingError$ = this.processingError.asObservable();
  summaryStatus$ = this.summaryStatus.asObservable();

  token:string | null = null;
  userData: User | null = null;
  private destroy$ = new Subject<void>();
  private messageService = inject(MessageService)

  private summarizingSubject = new BehaviorSubject<boolean>(false);
  summarizing$ = this.summarizingSubject.asObservable();
  isConnecting = false;

  constructor(private readonly httpClient: HttpClient, private readonly authenticationService: AuthenticationService) {
    this.authenticationService.userToken$.pipe(takeUntil(this.destroy$)).subscribe((token) => {
      this.token = token;
    })

    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe((userData) => {
      if(userData) this.userData = userData?.data;
    });
  }

  setChatOptions(options: ChatOptions) {
    this.chatOptions.next(options);
  }

  getMessagesByCondominium(condominiumId: string) {
    return this.httpClient.get<{ isSuccess: boolean, data: ChatMessageDto[]}>(`api/condominiums/${condominiumId}/messages`, {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    });
  }

  getCurrentSummaryResult() {
    const condominiumId = this.chatOptions.value?.condominium?.id;
  if (!condominiumId) {
    return throwError(() => new Error('No hay condominio seleccionado'));
  }

  return this.httpClient.get<{isSuccess: boolean, data:SummaryResult}>(
    `api/condominiums/${condominiumId}/summary`,
    {
      headers: {
        Authorization: `Bearer ${this.token}`
      }
    }
  ).pipe(
    catchError(error => {
      console.error('Error fetching summary result:', error);
      return throwError(() => error);
    })
  );
  }



  formatText(text: string): string[] {
    return text
      .replace(/\*\*(.*?)\*\*/g, '$1')  // Elimina negritas (**texto** -> texto)
      .replace(/- /g, '• ')  // Convierte guiones en viñetas
      .split(/\n+/)  // Divide en líneas por saltos de línea
      .map(line => line.trim())  // Elimina espacios innecesarios
      .filter(line => line);  // Elimina líneas vacías
  }

  //TODO: Implement this method and API
  getMessagesByUsers(userId: string, otherUserId: string) {
   // return this.httpClient.get<{ messages: Message[]}>(`api/users/${userId}/messages/${otherUserId}`);
  }

  // Metodo para solicitar un resumen de los mensajes del condominio
  requestCondominiumSummary(): Observable<RequestCondominiumSummaryResponse> {
    const currentOptions = this.chatOptions.value;

    if (currentOptions?.type !== 'condominium' || !currentOptions.condominium || !this.userData) {
      throw new Error('Condominio no seleccionado o no disponible');
    }

    // Restea los resultados previos
    this.summaryResult.next(null);
    this.processingError.next(null);
    this.summaryStatus.next(SummaryStatus.Created);

    // Hace una solicitud al endpoint para empezar un resumen
    return this.httpClient.post<RequestCondominiumSummaryResponse>(`api/condominiums/${currentOptions.condominium.id}/summary/${this.userData?.id}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }
    );
  }

  // Metodo para cancelar una solicitud de resumen
  cancelSummaryRequest(jobId: string): Observable<any> {
    const currentOptions = this.chatOptions.value;

    if ((currentOptions?.type === 'condominium' && !currentOptions?.condominium?.id) || !this.token) {
      throw new Error('Condominio no seleccionado o no disponible');
    }

    return this.httpClient.delete<any>(`api/condominiums/${currentOptions?.condominium?.id}/summary/${this.userData?.id}/cancel/${jobId}`,
      {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }
    ).pipe(
      tap(() => {
        // Immediately update local state
        this.summaryStatus.next(SummaryStatus.Cancelled);
        this.summaryResult.next(null);
        this.processingStatus.next(null);
      }),
      catchError(error => {
        console.error('Cancellation error', error);
        return throwError(() => error);
      })
    );
  }

  // Metodo para conectarse al hub de SignalR para un condominio
  async connectToCondominiumHub(condominiumId: string, condominiumName: string, userId: string): Promise<void> {
    // Prevenir conexiones múltiples simultáneas
    if (this.isConnecting) {
      console.log("Ya hay una conexión en curso, esperando...");
      return;
    }

    this.isConnecting = true;

    try {
      // Si ya estamos conectados al mismo grupo, no volver a conectar
      if (this.hubConnection &&
        this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.log("Ya estamos conectados al hub");
      this.isConnecting = false;
      return;
    }
      // Disconnect from any existing hub connection
      await this.disconnectFromHub();

      console.log("Intentando conectar a SignalR...");
      // Create a new hub connection
      this.hubConnection = new HubConnectionBuilder()
        .withUrl('/api/condominiums/hubs/summary', {
          transport: HttpTransportType.ServerSentEvents,
          accessTokenFactory: () => this.token || ''
        })
        .configureLogging(signalR.LogLevel.Debug)
        .withAutomaticReconnect()
        .build();

      // Set up event handlers
      this.setupHubEventHandlers();

      // Start the connection
      this.hubConnection.start().then(async() => {
          //console.log("FINALMENTE CONECTADO A SIGNALR");
          if(this.hubConnection) await this.hubConnection.invoke('JoinGroup', condominiumId, condominiumName, userId);
        })
        .catch(err => {
          console.error("Error al conectar a SignalR:", err);
        });

      // Join the group for this condominium

    } catch (error) {
      console.error('ERROR AL ESTABLECER CONEXION CON SIGNALR: ', error);
      // Re-throw the error to ensure the promise is rejected properly
      throw error;
    }finally{
      this.isConnecting = false;
    }
  }

  // Metodo para manejar eventos del hub
  private setupHubEventHandlers(): void {
    //console.log("Trying to set up hub event handlers...");
    if (!this.hubConnection) return;

    //console.log("Setting up hub event handlers...");

    this.hubConnection.on('RequestNewSummary', () => {
      // Broadcast that a new summary request has been initiated
      this.summaryStatus.next(SummaryStatus.Created);
      this.summaryResult.next(null);
      this.processingStatus.next(null);
    });

    this.hubConnection.on('NotifyProcessingStarted', (message: string) => {
      //console.log('Processing started: ', message);
      this.processingStatus.next(message);
    });

    this.hubConnection.on('SendSummary', (summary: any) => {
      //console.log('Summary received: ', summary);
      this.summaryResult.next(summary);
      this.processingStatus.next(null);
    });

    this.hubConnection.on('NotifyProcessingError', (errorMessage: string) => {
      //console.error('Processing error: ', errorMessage);
      this.processingError.next(errorMessage);
      this.processingStatus.next(null);
    });

    this.hubConnection.off('CancelledProcessing');

    this.hubConnection.on('CancelledProcessing', async (message: string) => {
     // console.log('Processing cancelled: ', message);
     // console.log('Summary cancelled');
      this.processingStatus.next(null);
      this.summaryStatus.next(SummaryStatus.Cancelled);
      this.summaryResult.next(null);
      await this.disconnectFromHub()
    });

    this.hubConnection.on("UserNotInCondominium", (errorMessage: string) => {
     // console.log("Processing failed: ", errorMessage);
      this.processingError.next(errorMessage);
      this.processingStatus.next(errorMessage)
      this.summaryResult.next(null)
    })

    this.hubConnection.on("ProcessingComplete", async (message: string) => {
      //console.log("Processing complete: ", message);
      this.processingStatus.next("COMPLETED");

      // Cargar el resumen cuando se completa
      this.getCurrentSummaryResult().subscribe({
        next: (response) => {
          if (response.isSuccess && response.data) {
            this.summaryResult.next(response.data);
            this.summaryStatus.next(SummaryStatus.Completed);
          }
        },
        error: (err) => console.error("Error loading completed summary:", err)
      });
      await this.disconnectFromHub();
    })

    this.hubConnection.on("NoNewMessages", (message: string) => {
      this.messageService.add({severity:'error', summary:'No hay mensajes nuevos', detail: message});
    })

    this.hubConnection.on("UpdateSummaryStatus", (status: SummaryStatus) => {
        console.log("Summary status updated: ", status);
        this.summaryStatus.next(status);
    })
  }

  loadCurrentSummaryStatus() {
    const currentOptions = this.chatOptions.value;

    if (currentOptions?.type === 'condominium' && currentOptions.condominium) {
      this.httpClient.get<{isSuccess: boolean, data: GetSummaryStatusResponse}>(`api/condominiums/${currentOptions.condominium.id}/summary/status`, {
        headers: {
          Authorization: `Bearer ${this.token}`
        }
      }).subscribe({
        next: (response) => {
          console.log('Summary status loaded successfully', response);
          this.summaryStatus.next(response.data.status);

          if (response.data.status === SummaryStatus.Cancelled) {
            this.summaryResult.next(null);
            this.processingStatus.next(null);
          }
        },
        error: (err) => {
          console.error('Error loading summary status', err);
          this.summaryStatus.next(null);
        }
      });
    }
  }

  // Metodo para desconectarse del hub
  async disconnectFromHub(): Promise<void> {
    if (this.hubConnection) {
      const currentOptions = this.chatOptions.value;

      try {
        // Check connection state before attempting to leave group
        if (this.hubConnection.state === signalR.HubConnectionState.Connected &&
            currentOptions?.type === 'condominium' &&
            currentOptions.condominium && this.userData) {

          try {
            await this.hubConnection.invoke('LeaveGroup',
              currentOptions.condominium.id,
              currentOptions.condominium.name,
              this.userData.id); // Usar userData.id en lugar de username que parece causar problemas
          } catch (leaveError) {
            console.warn('No se pudo dejar el grupo, pero continuamos con la desconexión:', leaveError);
          }
        }

        // Only stop if not already disconnected
        if (this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
          await this.hubConnection.stop();
          console.log('SignalR connection stopped');
        }
      } catch (error) {
        console.error('Error during hub disconnection: ', error);
      } finally {
        this.hubConnection = null;
      }
    }
  }

    ngOnDestroy(): void {
      this.summarizingSubject.complete();
      this.chatOptions.complete();
      this.processingStatus.complete();
      this.summaryResult.complete();
      this.processingError.complete();
      this.summaryStatus.complete();
      this.summarizingSubject.complete();
      this.messageService.clear();
      this.destroy$.next();
      this.destroy$.complete();
    }
}
