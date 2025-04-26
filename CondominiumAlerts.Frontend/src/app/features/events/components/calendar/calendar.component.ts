import {Component, inject, input, LOCALE_ID, OnDestroy, OnInit} from '@angular/core';
import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import {CondominiumEvent, PartialEvent} from '../../event.type';
import esLocale from '@fullcalendar/core/locales/es';
import { CalendarOptions } from '@fullcalendar/core/index.js';
import {EventService} from '../../services/event.service';
import {Condominium} from '../../../condominiums/models/condominium.model';
import {ChatService} from '../../../../shared/services/chat.service';
import {Subject, takeUntil} from 'rxjs';
import {AutoUnsubscribe} from '../../../../shared/decorators/autounsuscribe.decorator';
import {Dialog} from 'primeng/dialog';
import {Button, ButtonDirective, ButtonIcon, ButtonLabel} from 'primeng/button';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import {DatePipe, NgClass, NgIf, TitleCasePipe} from '@angular/common';
import {InputText} from 'primeng/inputtext';
import {Textarea} from 'primeng/textarea';
import {MessageService} from 'primeng/api';
import {Toast} from 'primeng/toast';
import {ProgressSpinner} from 'primeng/progressspinner';
import {User} from '../../../../core/auth/layout/auth-layout/user.type';
import {AuthenticationService} from '../../../../core/services/authentication.service';
import {Avatar} from 'primeng/avatar';
import {Tooltip} from 'primeng/tooltip';

@AutoUnsubscribe()
@Component({
  imports: [FullCalendarModule, Dialog, NgClass, InputText, ReactiveFormsModule, ButtonDirective, NgIf, Textarea, DatePipe, Button, Toast, ProgressSpinner, Avatar, TitleCasePipe, Tooltip],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css',
  selector: 'feature-calendar',
  providers: [
    { provide: LOCALE_ID, useValue: 'es-ES' }
  ]
})
export class CalendarComponent implements  OnInit, OnDestroy {
  isLoading = true;
  condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null = null;
  destroy$ = new Subject<void>();
  form: FormGroup;
  isEditable = false;
  modalMode: 'create' | 'edit' = 'create';
  events: CondominiumEvent[]|null = null;
  selectedEvent: CondominiumEvent | null = null;
  private messageService: MessageService = inject(MessageService)
  isDeleteModalOpen: boolean = false;
  visible: boolean = false;
  options: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    weekends: true,
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next',
      center: 'title',
    },
    editable: true,
    selectable: true,
    locale: esLocale,
    locales: [esLocale],
    dateClick: (info) => {
      try {
        const now = new Date();
        const selectedDate = new Date(info.date.getTime());

        // Comparar solo fechas (sin hora)
        const nowDateOnly = new Date(now);
        nowDateOnly.setHours(0, 0, 0, 0);

        const selectedDateOnly = new Date(selectedDate);
        selectedDateOnly.setHours(0, 0, 0, 0);

        // Si el día seleccionado es anterior a hoy, no continuar
        if (selectedDateOnly < nowDateOnly) return;

        let startDate: Date;
        const FIFTEEN_MIN = 15 * 60 * 1000;

        // Si es hoy, startDate = ahora + 15min
        // Si es otro día, startDate = fecha seleccionada a las 00:15
        if (selectedDateOnly.getTime() === nowDateOnly.getTime()) {
          startDate = new Date(now.getTime() + FIFTEEN_MIN);
        } else {
          startDate = new Date(selectedDateOnly);
          startDate.setHours(0, 15, 0, 0);
        }

        const endDate = new Date(startDate.getTime() + FIFTEEN_MIN);

        const formattedStart = this.formatDateToInput(startDate);
        const formattedEnd = this.formatDateToInput(endDate);

        this.form.patchValue({
          title: '',
          description: '',
          start: formattedStart,
          end: formattedEnd,
        });

        this.form.enable();
        this.isEditable = true;
        this.visible = true;
        this.modalMode = 'create';
      } catch (error) {
        console.error("Error en dateClick:", error);
      }
    },
    eventClick: (info) => {
      const seletectedEvent = this.events?.filter((event) => event.id === info.event.id)[0];
      if (seletectedEvent) {
        this.showDialog(seletectedEvent);
        this.modalMode = 'edit';
      }
    },
    dayMaxEventRows: 3,
    eventDrop: (info) => {
      const event = this.events?.filter((event) => event.id === info.event.id)[0];
      const newStart = info.event.start;
      const newEnd = info.event.end;
      const now = new Date();
      const selectedDate = new Date(newStart?.getTime()!);

      const nowDateOnly = new Date(now);
      nowDateOnly.setHours(0, 0, 0, 0);

      const selectedDateOnly = new Date(selectedDate);
      selectedDateOnly.setHours(0, 0, 0, 0);

      if (selectedDateOnly < nowDateOnly) {
        info.revert(); // <- ¡Importante!
        return;
      }

      if (event?.isStarted) {
        this.messageService.add({
          text: 'No puedes mover un evento que ya ha comenzado.',
          severity: 'error',
          summary: 'Error al mover evento'
        });
        info.revert(); // <- ¡Importante!
        return;
      }

      if (event?.isFinished) {
        this.messageService.add({
          text: 'No puedes mover un evento que ya ha terminado.',
          severity: 'error',
          summary: 'Error al mover evento'
        });
        info.revert(); // <- ¡Importante!
        return;
      }

      if(!event || !newStart || !newEnd) {
        info.revert(); // <- Por si acaso
        return;
      }

      const updatedEvent: CondominiumEvent = {
        ...event,
        start: newStart,
        end: newEnd,
      };

      this.eventService.update(updatedEvent, this.condominium?.id!).subscribe({
        next: res=> {
        if (!res.isSuccess) {
          this.messageService.add({
            text: 'Error al actualizar el evento',
            severity: 'error',
            summary: 'Error al actualizar evento'
          });
          info.revert(); // <- Si falla la actualización
        } else {
          const formattedDate = this.formatDateToInput(new Date(updatedEvent.start));
          this.messageService.add({
            text: `Inicio del evento movido correctamente para el ${formattedDate}`,
            severity: 'success',
            summary: 'Evento actualizado correctamente'
          });
        }
      }
    , error: err => {
        this.messageService.add({
          text: 'Error de red al actualizar el evento',
          severity: 'error',
          summary: 'Error'
        });
        info.revert(); // <- Si falla la petición
      }
    });
    }
  }
  user: User|null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly eventService: EventService,
    private readonly chatService: ChatService,
    private readonly authenticationService: AuthenticationService) {
    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Actualizar la opción de chat actual
      if (options?.condominium) {
        this.condominium = options?.condominium;
      }

      if (this.condominium?.id) {
        this.eventService.get(this.condominium?.id).subscribe(); // Esto solo dispara el fetch inicial

        this.eventService.events$
          .pipe(takeUntil(this.destroy$))
          .subscribe(events => {
            this.events = events;
            // this.isLoading = false;
          });
      }
    });

    this.authenticationService.userData$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      if (user?.data) this.user = user?.data!;
    });

    this.form = this.fb.group({
      title: [{ value: '', disabled: true}, [Validators.required, Validators.maxLength(100)]],
      description: [{ value: '', disabled: true }, [Validators.required, Validators.minLength(20), Validators.maxLength(500)]],
      start: [{ value: '', disabled: true }, [this.noPastDateValidator.bind(this), this.minimumOneMinuteValidator.bind(this)]],
      end: [{ value: '', disabled: true }]
    }, {
      validators: [this.validateDateRange]
    });
  }

  ngOnInit() {
    if(this.condominium?.id) this.eventService.get(this.condominium?.id).subscribe((res) => {
      this.events = [...res.data];
      this.isLoading = false;
    });
  }

  subscribeToEvent(eventId: string){
    const event = this.events?.filter((event) => event.id === eventId)[0];

    if(!event) return;

    this.eventService.addSubscription(eventId, this.condominium?.id!).pipe(takeUntil(this.destroy$)).subscribe(
      {
        next: res => {
          this.messageService.add({
            text: `Te has suscrito al evento ${event?.title}`,
            severity: 'success',
            summary: 'Suscripción exitosa'
          });
          this.visible = false;
        },
        error: err => {
          this.messageService.add({
            text: `Error al suscribirse al evento ${event?.title}`,
            severity: 'error',
            summary: 'Error'
          });
          this.visible = false;
        }
      }
    )
  }

  unsubscribeFromEvent(eventId: string){
    const event = this.events?.filter((event) => event.id === eventId)[0];

    if(!event) return;

    this.eventService.removeSubscription(eventId, this.condominium?.id!).pipe(takeUntil(this.destroy$)).subscribe(
      {
        next: res => {
          this.messageService.add({
            text: `Has cancelado la subscripción al evento ${event?.title}`,
            severity: 'success',
            summary: 'Cancelación de subscripción exitosa'
          });
          this.visible = false;
        },
        error: err => {
          this.messageService.add({
            text: `Error al cancelar la subscripción al evento ${event?.title}`,
            severity: 'error',
            summary: 'Error'
          });
          this.visible = false;
        }
      }
    )
  }

  validateDateRange(group: AbstractControl): ValidationErrors | null {
    const start = group.get('start')?.value;
    const end = group.get('end')?.value;

    if (!start || !end) return null;

    const startDate = new Date(start);
    const endDate = new Date(end);

    if (endDate <= startDate) {
      return { invalidDateRange: true };
    }

    return null;
  }

  noPastDateValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const selected = new Date(value);
    const now = new Date();
    if (selected <= now) {
      return { pastDate: true };
    }

    return null;
  }

  minimumOneMinuteValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const selectedDate = new Date(value);
    const now = new Date();

    // Calcular la diferencia en milisegundos
    const differenceInMilliseconds = selectedDate.getTime() - now.getTime();

    // Convertir a minutos
    const differenceInMinutes = differenceInMilliseconds / (1000 * 60);

    if (differenceInMinutes < 1) {
      return { minimumOneMinute: true };
    }

    return null;
  }

  showDialog(selectedEvent: CondominiumEvent) {
    this.selectedEvent = selectedEvent;
    this.visible = true;

    // Asegúrate de que las fechas son objetos Date
    const startDate = selectedEvent.start ? new Date(selectedEvent.start) : null;
    const endDate = selectedEvent.end ? new Date(selectedEvent.end) : null;

    this.form.patchValue({
      title: selectedEvent.title,
      description: selectedEvent.description,
      start: startDate,
      end: endDate
    });

    this.form.disable();
    this.isEditable = false;
  }

  toggleEdit() {
    this.isEditable = !this.isEditable;

    if (this.isEditable) {
      // Antes de habilitar, formatea las fechas para los inputs
      const start = this.form.get('start')?.value;
      const end = this.form.get('end')?.value;

      this.form.enable();

      // Actualiza los valores con el formato correcto para datetime-local
      this.form.patchValue({
        start: this.formatDateToInput(new Date(start)),
        end: this.formatDateToInput(new Date(end))
      });
    } else {
      this.form.disable();
    }
  }

  // Utilidad para dar formato YYYY-MM-DDTHH:mm
  formatDateToInput(date?: Date): string {
    if (!date) return '';
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().slice(0, 16);
  }

  isDue(date?: Date){
    if (!date) return false;
    return new Date(date) < new Date();
  }

  saveEvent(){
    const formData = this.form.value;

    if(this.modalMode === 'create') {
      const event: PartialEvent = {
        id: '',
        title: formData.title,
        description: formData.description,
        start: new Date(formData.start),
        end: new Date(formData.end)
      }
      this.eventService.save(event, this.condominium?.id!).subscribe(res => {
        const formattedDate = this.formatDateToInput(new Date(event.start));
        const text = res.isSuccess ? `Evento creado para el ${formattedDate}` : 'Error al actualizar el evento';
        const severity = res.isSuccess ? 'success' : 'error';
        const summary = res.isSuccess ? 'Evento creado correctamente' : 'Error al crear evento';

        this.messageService.add({
          text,
          severity,
          summary
        })
      });
    } else if(this.modalMode === 'edit') {
      const event: CondominiumEvent = {
        id: this.selectedEvent?.id!,
        title: formData.title,
        description: formData.description,
        start: new Date(formData.start),
        end: new Date(formData.end),
        isStarted: this.selectedEvent?.isStarted!,
        isFinished: this.selectedEvent?.isFinished!,
        isToday: this.selectedEvent?.isToday!,
        createdBy: this.selectedEvent?.createdBy!,
        suscribers: this.selectedEvent?.suscribers!,
        createdAt: this.selectedEvent?.createdAt!,
        updatedAt: this.selectedEvent?.updatedAt!,
        isSuscribed: true
      }

      this.eventService.update(event, this.condominium?.id!).subscribe(res => {
        const formattedDate = this.formatDateToInput(new Date(this.selectedEvent?.start!));
        const text = res.isSuccess ? `Evento actualizado correctamente para el ${formattedDate}` : 'Error al actualizar el evento';
        const severity = res.isSuccess ? 'success' : 'error';
        const summary = res.isSuccess ? 'Evento actualizado correctamente' : 'Error al actualizar evento';

        this.messageService.add({
          text,
          severity,
          summary
        })
      });
    }

    this.visible = false;
    this.form.reset();
    this.form.disable();
    this.isEditable = false;
  }

  deleteEvent(){
    if(!this.selectedEvent) return;
    this.isDeleteModalOpen = false
    this.eventService.delete(this.selectedEvent.id, this.condominium?.id!).subscribe({
      next: res => {
        this.visible = false;
        const text = res.isSuccess ? `Evento "${this.selectedEvent?.title}" eliminado correctamente` : 'Error al eliminar el evento';
        const severity = res.isSuccess ? 'success' : 'error';
        const summary = res.isSuccess ? 'Evento eliminado correctamente' : 'Error al eliminar evento';

        this.messageService.add({
          text,
          severity,
          summary
        });
      },
      error: err => {
        const text = 'No se pudo eliminar el evento.';
        const summary = "Error al eliminar evento";
        const severity = 'error';

        this.messageService.add({
          text,
          severity,
          summary
        });
      }
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
