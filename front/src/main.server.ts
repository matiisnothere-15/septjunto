import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app';
import { appConfigServer } from './app/app.config.server';

export default function() {
  return bootstrapApplication(AppComponent, appConfigServer);
}
