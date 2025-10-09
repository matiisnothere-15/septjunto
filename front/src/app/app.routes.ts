import { Routes } from '@angular/router';
// pon aquí tu componente de inicio
import { InicioComponent } from './features/inicio/inicio.component';

export const routes: Routes = [
  { path: '', component: InicioComponent },
  { path: 'configuracion', loadChildren: () => import('./features/configuracion/configuracion.routes') },
  { path: 'evaluaciones',  loadChildren: () => import('./features/evaluaciones/evaluaciones.routes') },
];
