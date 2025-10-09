import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Componente, Complejidad, RelacionCC, Evaluacion, Proyecto } from './models';

@Injectable({ providedIn: 'root' })
export class DataService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api'; // Cambia esto a la URL de tu API

  // Observable-based cache
  private componentesCache$: Observable<Componente[]> | null = null;
  private complejidadesCache$: Observable<Complejidad[]> | null = null;
  private relacionesCache$: Observable<RelacionCC[]> | null = null;

  // ---- Componentes ----
  getComponentes(forceRefresh = false): Observable<Componente[]> {
    if (!this.componentesCache$ || forceRefresh) {
      this.componentesCache$ = this.http.get<Componente[]>(`${this.apiUrl}/componentes`).pipe(
        tap(() => console.log('Componentes fetched from API'))
      );
    }
    return this.componentesCache$;
  }

  addComponente(nombre: string, descripcion = ''): Observable<any> {
    return this.http.post(`${this.apiUrl}/componentes`, { nombre, descripcion }).pipe(
      tap(() => this.componentesCache$ = null) // Invalidate cache
    );
  }

  updateComponente(id: string, nombre: string, descripcion?: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/componentes/${id}`, { id, nombre, descripcion }).pipe(
      tap(() => this.componentesCache$ = null) // Invalidate cache
    );
  }

  deleteComponente(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/componentes/${id}`).pipe(
      tap(() => this.componentesCache$ = null) // Invalidate cache
    );
  }

  // ---- Complejidades ----
  getComplejidades(forceRefresh = false): Observable<Complejidad[]> {
    if (!this.complejidadesCache$ || forceRefresh) {
      this.complejidadesCache$ = this.http.get<Complejidad[]>(`${this.apiUrl}/complejidades`).pipe(
        map(complejidades => complejidades.sort((a, b) => a.orden - b.orden)),
        tap(() => console.log('Complejidades fetched from API'))
      );
    }
    return this.complejidadesCache$;
  }

  addComplejidad(nombre: string, orden: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/complejidades`, { nombre, orden }).pipe(
      tap(() => this.complejidadesCache$ = null) // Invalidate cache
    );
  }

  deleteComplejidad(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/complejidades/${id}`).pipe(
      tap(() => this.complejidadesCache$ = null) // Invalidate cache
    );
  }

  // ---- Relaciones ----
  getRelaciones(forceRefresh = false): Observable<RelacionCC[]> {
    if (!this.relacionesCache$ || forceRefresh) {
      this.relacionesCache$ = this.http.get<RelacionCC[]>(`${this.apiUrl}/relaciones`).pipe(
        tap(() => console.log('Relaciones fetched from API'))
      );
    }
    return this.relacionesCache$;
  }

  upsertRelacion(componenteId: string, complejidadId: string, horas: number): Observable<any> {
    // This logic needs to be adapted based on your backend implementation
    // Assuming a POST/PUT endpoint for relations
    return this.http.post(`${this.apiUrl}/relaciones`, { componenteId, complejidadId, horas }).pipe(
      tap(() => this.relacionesCache$ = null) // Invalidate cache
    );
  }
  
  deleteRelacion(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/relaciones/${id}`).pipe(
      tap(() => this.relacionesCache$ = null) // Invalidate cache
    );
  }

  // ---- Evaluaciones ----
  getEvaluaciones(): Observable<Evaluacion[]> {
    return this.http.get<Evaluacion[]>(`${this.apiUrl}/evaluaciones`).pipe(
        map(evaluaciones => evaluaciones.sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime()))
    );
  }

  getEvaluacion(id: string): Observable<Evaluacion | undefined> {
    return this.http.get<Evaluacion>(`${this.apiUrl}/evaluaciones/${id}`);
  }

  createEvaluacion(
    nombreProyecto: string,
    deltaRiesgoPct: number | undefined,
    items: { componenteId: string; complejidadId: string; descripcionTarea: string; }[]
  ): Observable<string> {
    const command = { nombreProyecto, deltaRiesgoPct, detalles: items };
    return this.http.post<string>(`${this.apiUrl}/evaluaciones`, command);
  }

  updateEvaluacion(
    id: string,
    nombreProyecto: string,
    deltaRiesgoPct: number | undefined,
    items: { componenteId: string; complejidadId: string; descripcionTarea: string; }[]
  ): Observable<any> {
    const command = { id, nombreProyecto, deltaRiesgoPct, detalles: items };
    return this.http.put(`${this.apiUrl}/evaluaciones/${id}`, command);
  }

  deleteEvaluacion(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/evaluaciones/${id}`);
  }

  // ---- Proyectos ----
  getProyectos(): Observable<Proyecto[]> {
    return this.http.get<Proyecto[]>(`${this.apiUrl}/proyectos`).pipe(
        map(proyectos => proyectos.sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime()))
    );
  }
}