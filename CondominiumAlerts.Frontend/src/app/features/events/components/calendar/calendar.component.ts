import {Component, input, LOCALE_ID, OnDestroy, OnInit} from '@angular/core';
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
import {User} from '../../../../core/auth/layout/auth-layout/user.type';
import {Dialog} from 'primeng/dialog';
import {Button, ButtonDirective, ButtonIcon, ButtonLabel} from 'primeng/button';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {DatePipe, NgClass, NgIf} from '@angular/common';
import {InputText} from 'primeng/inputtext';
import {Textarea} from 'primeng/textarea';

@AutoUnsubscribe()
@Component({
  imports: [FullCalendarModule, Dialog, NgClass, InputText, ReactiveFormsModule, ButtonDirective, ButtonLabel, NgIf, Textarea, DatePipe, ButtonIcon, Button],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css',
  selector: 'feature-calendar',
  providers: [
    { provide: LOCALE_ID, useValue: 'es-ES' }
  ]
})
export class CalendarComponent implements  OnInit, OnDestroy {
  condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null = null;
  destroy$ = new Subject<void>();
  form: FormGroup;
  isEditable = false;

  constructor(private readonly fb: FormBuilder,private readonly eventService: EventService, private readonly chatService: ChatService) {
    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Actualizar la opción de chat actual
      if (options?.condominium) this.condominium = options?.condominium;
    });

    this.form = this.fb.group({
      title: [{ value: '', disabled: true }],
      description: [{ value: '', disabled: true }],
      start: [{ value: '', disabled: true }],
      end: [{ value: '', disabled: true }]
    });
  }

  ngOnInit() {
    if(this.condominium?.id) this.eventService.getEvents(this.condominium?.id).subscribe((res) => {
      console.log("EVENTS", res)
      this.events = [...res.data];
    });
  }

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
    dateClick: () => alert('Date clicked!'),
    eventClick: (info) => {
     const seletectedEvent = this.events?.filter((event) => event.id === info.event.id)[0];
      if (seletectedEvent) {
        this.showDialog(seletectedEvent);
      }
    },
    dayMaxEventRows: 3,
    eventDrop: (info) => {
      console.log("NEW EVENT DATE START", info.event.start);
      console.log("NEW EVENT DATE END", info.event.end);
    }
  }

  events: CondominiumEvent[]|null = null;
  selectedEvent: CondominiumEvent | null = null;

  visible: boolean = false;

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
        start: this.formatDateToInput(start),
        end: this.formatDateToInput(end)
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
    console.log('FORMATTED DATE', localDate.toISOString().slice(0, 16));
    return localDate.toISOString().slice(0, 16);
  }

  isEventEnded(event: CondominiumEvent){
    return new Date(event.end) < new Date();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
