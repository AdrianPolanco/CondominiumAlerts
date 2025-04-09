import { Component } from '@angular/core';
import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import { PartialEvent } from '../../event.type';
import esLocale from '@fullcalendar/core/locales/es';
import { CalendarOptions } from '@fullcalendar/core/index.js';

@Component({
  imports: [FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css',
  selector: 'feature-calendar',
})
export class CalendarComponent {
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

  events: PartialEvent[] = [
    {
      id: '1',
      title: 'Event 1',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86403600),
      description: 'Description for Event 1',
    },
    {
      id: '2',
      title: 'Event 2',
      start: new Date(Date.now() + 86400000 * 2),
      end: new Date(Date.now() + 86400000 * 3),
      description: 'Description for Event 2',
    },
    {
      id: '3',
      title: 'Event 3',
      start: new Date(Date.now() + 86400000 * 4),
      end: new Date(Date.now() + 86400000 * 5),
      description: 'Description for Event 3'
    },
    {
      id: '4',
      title: 'Event 4',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86415877),
      description: 'Description for Event 4',
    },
    {
      id: '5',
      title: 'Event 5',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86415877),
      description: 'Description for Event 5',
    },
    {
      id: '6',
      title: 'Event 6',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86415877),
      description: 'Description for Event 6',
    },
    {
      id: '7',
      title: 'Event 7',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86415877),
      description: 'Description for Event 7',
    },
    {
      id: '8',
      title: 'Event 8',
      start: new Date(Date.now() + 86400000),
      end: new Date(Date.now() + 86415877),
      description: 'Description for Event 8',
    },
  ]
}