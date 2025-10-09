import { Routes } from '@angular/router';
import { ComponentesComponent } from './componentes/componentes.component';
import { ComplejidadesComponent } from './complejidades/complejidades.component';
import { RelacionesComponent } from './relaciones/relaciones.component';

export default [
  { path: '', redirectTo: 'componentes', pathMatch: 'full' },
  { path: 'componentes', component: ComponentesComponent },
  { path: 'complejidades', component: ComplejidadesComponent },
  { path: 'relaciones', component: RelacionesComponent },
] as Routes;
