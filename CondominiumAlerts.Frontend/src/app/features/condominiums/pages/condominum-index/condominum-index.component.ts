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
  users = [{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 100
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 1000
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 0
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 10
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 10
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 0
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 5
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 0
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 20
  },{
    img: "https://img.freepik.com/free-vector/blue-circle-with-white-user_78370-4707.jpg",
    unReadMessages: 50
  },]
goHome(){
 // this.router.navigate([""])
}

}
