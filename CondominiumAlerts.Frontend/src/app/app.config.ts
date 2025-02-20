import {
  ApplicationConfig,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { routes } from './app.routes';
import { initializeApp, provideFirebaseApp } from '@angular/fire/app';
import { getAuth, provideAuth } from '@angular/fire/auth';
import { environment } from '../enviroments/environment';
import {MyPreset} from './theme';
import {provideHttpClient, withFetch} from '@angular/common/http';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideFirebaseApp(() => initializeApp(environment.firebase)),
    provideAuth(() => getAuth()),
    provideAnimationsAsync(),
    providePrimeNG({
      ripple: true,
      theme: {
          preset: MyPreset,
          options: {
              prefix: 'p',
              darkModeSelector: 'system',
              cssLayer: true
          }
      }
  }),
    provideHttpClient(withFetch()),
    MessageService
  ],
};

