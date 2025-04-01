import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Toolbar } from 'primeng/toolbar';
import { NgOptimizedImage } from '@angular/common';
import { RouterModule, RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, Toolbar, NgOptimizedImage, RouterModule, RouterLink], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent { }
