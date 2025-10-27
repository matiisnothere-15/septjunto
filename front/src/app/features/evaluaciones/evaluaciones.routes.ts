import { Routes } from '@angular/router';
import { RenderMode } from '@angular/ssr';

export default [
  { path: '', redirectTo: 'crear', pathMatch: 'full' }, // Redirige directamente a crear
  { path: 'crear', loadComponent: () => import('./crear/crear.component').then(m => m.CrearComponent) },
  { path: 'listar', loadComponent: () => import('./listar/listar.component').then(m => m.ListarComponent) }, // Mover listar a una ruta específica
  // Deshabilitar prerender para ruta dinámica y forzar renderizado en servidor en tiempo de petición
  { path: ':id', loadComponent: () => import('./detalle/detalle.component').then(m => m.DetalleComponent), renderMode: RenderMode.Server },
] as Routes;
