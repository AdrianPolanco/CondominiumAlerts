import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Toolbar } from 'primeng/toolbar';
import { NgOptimizedImage } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ButtonModule, Toolbar, NgOptimizedImage], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent { }
