import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { NgFor,CommonModule, NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';
import { Toolbar } from 'primeng/toolbar';
import { Button } from 'primeng/button';

@Component({
  selector: 'app-condominum-index',
  imports: [Toolbar, NgFor,CommonModule, Button, NgOptimizedImage],
  templateUrl: './condominum-index.component.html',
  styleUrl: './condominum-index.component.css',
  schemas:[CUSTOM_ELEMENTS_SCHEMA]
})
export class CondominumIndexComponent {
  users = [
    { name: 'Juan Pérez', status: 'En línea', avatar: 'https://via.placeholder.com/40' },
    { name: 'María López', status: 'Ausente', avatar: 'https://via.placeholder.com/40' },
    { name: 'Carlos Gómez', status: 'En línea', avatar: 'https://via.placeholder.com/40' },
  ];
  notifications = [
    { message: 'Nuevo mensaje de Juan', time: 'Hace 5 minutos' },
    { message: 'Carlos ha publicado algo nuevo', time: 'Hace 1 hora' },
    { message: 'María ha reaccionado a tu publicación', time: 'Hace 2 horas' },
  ];

  publications = [
    {
      title: 'Título de la publicación 1',
      content: 'Esta es una publicación de ejemplo con una imagen. Aquí iría el contenido de la publicación.',
      image: 'https://via.placeholder.com/600x300',
      time: 'Hace 2 horas',
    },
    {
      title: 'Título de la publicación 2',
      content: 'Otra publicación de ejemplo. Aquí se puede incluir más contenido relevante.',
      image: null, // Sin imagen
      time: 'Hace 5 horas',
    },
    {
      title: 'Título de la publicación 3',
      content: 'Última publicación de ejemplo. Aquí se puede agregar más texto o imágenes.',
      image: 'https://via.placeholder.com/600x200',
      time: 'Hace 1 día',
    },
  ];
  constructor(private router: Router){

  }

goHome(){
// // this.router.navigate([""])
  }
  openCreatePostModal() {
        console.log('Abrir modal de creación de publicaciones');
        // Aquí puedes implementar la lógica para abrir un modal o redirigir a otra página
  }

}
