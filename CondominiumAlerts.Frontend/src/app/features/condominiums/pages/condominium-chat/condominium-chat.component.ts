import { Component } from '@angular/core';
import { CondominiumsLayoutComponent } from "../../../../shared/components/condominiums-layout/condominiums-layout.component";
import { ChatComponent } from "../../../../shared/components/chat/chat.component";
import { AutoUnsubscribe } from '../../../../shared/decorators/autounsuscribe.decorator';

@AutoUnsubscribe()
@Component({
  selector: 'app-condominium-chat',
  imports: [CondominiumsLayoutComponent, ChatComponent],
  templateUrl: './condominium-chat.component.html',
  styleUrl: './condominium-chat.component.css',
})
export class CondominiumChatComponent{


}
