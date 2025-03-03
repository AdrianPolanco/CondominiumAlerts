import { Component, OnInit } from '@angular/core';
import { NgFor, CommonModule, NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';
import { Toolbar } from 'primeng/toolbar';
import { Button } from 'primeng/button';
import { PostService } from '../../../posts/services/post.service';

@Component({
  selector: 'app-condominum-index',
  imports: [Toolbar, NgFor, CommonModule, Button, NgOptimizedImage],
  templateUrl: './condominum-index.component.html',
  styleUrls: ['./condominum-index.component.css'],
})
export class CondominumIndexComponent implements OnInit {
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

  publications: any[] = [];

  constructor(
    private router: Router,
    private postService: PostService
  ) { }

  ngOnInit(): void {
    this.loadPosts();
  }


  loadPosts(): void {
    this.postService.getPosts().subscribe({
      next: (data) => {
        console.log('Publicaciones recibidas:', data); 
        this.publications = data;
      },
      error: (err) => {
        console.error('Error al cargar las publicaciones:', err);
      },
    });
  }

  goHome(): void {
    this.router.navigate(['']);
  }

  openCreatePostModal(): void {
    console.log('Abrir modal de creación de publicaciones');
  }
}
