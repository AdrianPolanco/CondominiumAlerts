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

  constructor(private router: Router){

  }
  users = [{}]
goHome(){
 // this.router.navigate([""])
}

}
