import { ApplicationConfig } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { apiBaseInterceptor } from './shared/api-base.interceptor';

export const appConfigServer: ApplicationConfig = {
  providers: [
    provideServerRendering(),
    provideRouter(routes),
    // Provide HttpClient on the server too (for SSR / dev server pre-render)
    provideHttpClient(
      withFetch(),
      withInterceptors([apiBaseInterceptor])
    ),
  ],
};
