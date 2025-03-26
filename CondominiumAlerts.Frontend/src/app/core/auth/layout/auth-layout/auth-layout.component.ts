import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { UserOptionsComponent } from "../user-options/user-options.component";
import { ChatsDrawerComponent } from "../../../../shared/components/chats-drawer/chats-drawer.component";
import { Condominium } from '../../../../features/condominiums/models/condominium.model';

@Component({
  selector: 'core-auth-layout',
  imports: [
    RouterOutlet,
    UserOptionsComponent,
    ChatsDrawerComponent
],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.css'
})
export class AuthLayoutComponent{
  router = inject(Router);
  onCondominiumSelected(): void {
    this.router.navigate([`/condominium/chat`]);
  }
}
