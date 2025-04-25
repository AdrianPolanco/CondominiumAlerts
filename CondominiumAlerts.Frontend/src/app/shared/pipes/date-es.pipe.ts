import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateEs'
})
export class DateEsPipe implements PipeTransform {

  transform(value: Date | string): string {
    if (!value) return '';

    const options: Intl.DateTimeFormatOptions = {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    };

    return new Date(value).toLocaleString('es-ES', options);
  }

}
