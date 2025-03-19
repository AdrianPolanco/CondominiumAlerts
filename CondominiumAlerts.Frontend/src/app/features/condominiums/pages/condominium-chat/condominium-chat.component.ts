import { Component, inject, OnInit } from '@angular/core';
import { CondominiumsLayoutComponent } from "../../../../shared/components/condominiums-layout/condominiums-layout.component";
import { ChatComponent } from "../../../../shared/components/chat/chat.component";
import { AuthenticationService } from '../../../../core/services/authentication.service';
import { ChatOptions } from '../../../../shared/components/chat/chat.type';
import { CondominiumService } from '../../services/condominium.service';
import { Subject } from 'rxjs';
import { AutoUnsubscribe } from '../../../../shared/decorators/autounsuscribe.decorator';
import { ChatService } from '../../../../shared/services/chat.service';

@AutoUnsubscribe()
@Component({
  selector: 'app-condominium-chat',
  imports: [CondominiumsLayoutComponent, ChatComponent],
  templateUrl: './condominium-chat.component.html',
  styleUrl: './condominium-chat.component.css',
})
export class CondominiumChatComponent{


}
