import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo',
  standalone: true
})
export class TimeAgoPipe implements PipeTransform {

  transform(value: string | Date): string {
    const now = new Date();
    const date = new Date(value);
    const diff = (now.getTime() - date.getTime()) / 1000; // en segundos

    if (diff < 60) return `Hace ${Math.floor(diff)} segundos`;
    if (diff < 3600) return `Hace ${Math.floor(diff / 60)} min`;
    if (diff < 86400) return `Hace ${Math.floor(diff / 3600)} horas`;
    if (diff < 604800) return `Hace ${Math.floor(diff / 86400)} días`;
    if (diff < 2592000) return `Hace ${Math.floor(diff / 604800)} semanas`;
    if (diff < 31536000) return `Hace ${Math.floor(diff / 2592000)} meses`;

    return `Hace ${Math.floor(diff / 31536000)} años`;
  }

}
