import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-sign-in',
    standalone: true,
    imports: [
        CommonModule,
    ],
    template: `<p>sign-in works!</p>`,
    styleUrl: './sign-in.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SignInComponent { }
