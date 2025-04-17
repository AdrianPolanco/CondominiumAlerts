import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'timeAgo' })
export class TimeAgoPipe implements PipeTransform {
  transform(value: string | Date): string {
    if (!value) return '';
    const date = new Date(value);
    const seconds = Math.floor((Date.now() - date.getTime()) / 1000);

    const intervals = {
      año: 31536000,
      mes: 2592000,
      día: 86400,
      hora: 3600,
      minuto: 60
    };

    for (const [unit, secondsInUnit] of Object.entries(intervals)) {
      const interval = Math.floor(seconds / secondsInUnit);
      if (interval >= 1) {
        return `hace ${interval} ${unit}${interval > 1 ? 's' : ''}`;
      }
    }
    return 'hace unos segundos';
  }
}
