import { Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { Button } from 'primeng/button';

@Component({
  selector: 'shared-back-arrow',
  imports: [Button],
  templateUrl: './back-arrow.component.html',
  styleUrl: './back-arrow.component.css'
})
export class BackArrowComponent {
  private router = inject(Router);
  url = input.required<string>();
  parameters = input<string>();
  rounded = input<boolean>(true);
  color = input<'primary'|'contrast'|'transparent'>('primary');

  goBack(): void {
    if(!this.url()) {
      this.router.navigate(['/condominiums']);
      throw new Error('No se ha proporcionado una URL para redirigir al usuario');
    }
    if (this.parameters) this.router.navigate([this.url()], { queryParams: { parameters: this.parameters() } });
    else this.router.navigate([this.url()]);
  }

}
