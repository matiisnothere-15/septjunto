import { Routes } from '@angular/router';

export default [
  { path: '', redirectTo: 'crear', pathMatch: 'full' }, // Redirige directamente a crear
  { path: 'crear', loadComponent: () => import('./crear/crear.component').then(m => m.CrearComponent) },
  { path: 'listar', loadComponent: () => import('./listar/listar.component').then(m => m.ListarComponent) }, // Mover listar a una ruta específica
  { path: ':id', loadComponent: () => import('./detalle/detalle.component').then(m => m.DetalleComponent), prerender: false },
] as Routes;
