import {Component, input, OnDestroy, OnInit} from '@angular/core';
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

@AutoUnsubscribe()
@Component({
  imports: [FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css',
  selector: 'feature-calendar',
})
export class CalendarComponent implements  OnInit, OnDestroy {
  condominium: Pick<Condominium, 'id' | 'name' | 'imageUrl'| 'address'> | null = null;
  destroy$ = new Subject<void>();

  constructor(private readonly eventService: EventService, private readonly chatService: ChatService) {
    this.chatService.chatOptions$.pipe(takeUntil(this.destroy$)).subscribe((options) => {
      // Actualizar la opción de chat actual
      if (options?.condominium) this.condominium = options?.condominium;
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
      alert('Event clicked: ' + info.event.title);
    },
    dayMaxEventRows: 3,
   // eventDrop:
  }

  events: CondominiumEvent[]|null = null;

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
