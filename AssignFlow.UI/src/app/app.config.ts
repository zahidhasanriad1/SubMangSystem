import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { ConfirmationService, MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { routes } from './app.routes';
import { apiInterceptor } from './core/http/api.interceptor';

const AssignFlowPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#edf9f8',
      100: '#d3f0ee',
      200: '#abe1de',
      300: '#75cbc8',
      400: '#43adae',
      500: '#238d92',
      600: '#197179',
      700: '#175b63',
      800: '#174a52',
      900: '#163e45',
      950: '#08272e'
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'top' })),
    provideHttpClient(withFetch(), withInterceptors([apiInterceptor])),
    provideAnimationsAsync(),
    providePrimeNG({
      ripple: true,
      theme: {
        preset: AssignFlowPreset,
        options: { darkModeSelector: '.assignflow-dark' }
      }
    }),
    MessageService,
    ConfirmationService
  ]
};
