import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, tap, switchMap, shareReplay } from 'rxjs/operators';
import { Componente, Complejidad, RelacionCC, Evaluacion, Proyecto } from './models';

@Injectable({ providedIn: 'root' })
export class DataService {
  private http = inject(HttpClient);
  // Usar rutas relativas y dejar que el interceptor prefije apiBaseUrl
  private apiUrl = '/api';

  // Observable-based cache
  private componentesCache$: Observable<Componente[]> | null = null;
  private complejidadesCache$: Observable<Complejidad[]> | null = null;
  private relacionesCache$: Observable<RelacionCC[]> | null = null;

  // ---- Componentes ----
  getComponentes(forceRefresh = false): Observable<Componente[]> {
    if (!this.componentesCache$ || forceRefresh) {
      this.componentesCache$ = this.http.get<Componente[]>(`${this.apiUrl}/componentes`).pipe(
        shareReplay({ bufferSize: 1, refCount: true }),
        tap(() => console.log('Componentes fetched from API'))
      );
    }
    return this.componentesCache$;
  }

  // --- INICIO DEL CAMBIO ---
  // Se modifica para que espere un objeto JSON {id: "..."} del backend
  // y extraiga el ID, en lugar de esperar 'text'.
  addComponente(nombre: string, descripcion = ''): Observable<string> {
    return this.getProyectos().pipe(
      map((proyectos: Proyecto[]) => proyectos[0]?.id),
      switchMap((proyectoId: string | undefined) => {
        const body: any = { nombre, descripcion };
        if (proyectoId) body.proyectoId = proyectoId;
        
        // Espera un JSON <any> (o <{id: string}>) y extrae el id
        return this.http.post<any>(`${this.apiUrl}/componentes`, body).pipe(
          tap(response => console.log('DEBUG DataService: Respuesta API Componente:', response)),
          map(response => response?.id ?? response?.Id ?? response)
        ); 
      }),
      tap(() => (this.componentesCache$ = null))
    );
  }
  // --- FIN DEL CAMBIO ---

  updateComponente(id: string, nombre: string, descripcion?: string, proyectoId?: string): Observable<any> {
    const body: any = { id, nombre, descripcion };
    if (proyectoId) body.proyectoId = proyectoId;
    return this.http.put(`${this.apiUrl}/componentes/${id}`, body).pipe(
      tap(() => this.componentesCache$ = null)
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
        map((c: Complejidad[]) => c.sort((a, b) => a.orden - b.orden)),
        shareReplay({ bufferSize: 1, refCount: true }),
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
        shareReplay({ bufferSize: 1, refCount: true }),
        tap(() => console.log('Relaciones fetched from API'))
      );
    }
    return this.relacionesCache$;
  }

  /**
   * Upsert relación componente-complejidad.
   * Lógica en front: si existe par (componenteId, complejidadId) hace PUT; si no, POST.
   * Nota: el backend también soporta upsert en POST, pero esta variante evita errores de unicidad.
   */
  upsertRelacion(componenteId: string, complejidadId: string, horas: number): Observable<string | void> {
    return this.getRelaciones(true).pipe(
      switchMap((relaciones: RelacionCC[]) => {
        const existente = relaciones.find(r => r.componenteId === componenteId && r.complejidadId === complejidadId);
        if (existente) {
          return this.updateRelacion(existente.id, componenteId, complejidadId, horas).pipe(
            tap(() => this.relacionesCache$ = null)
          );
        }
        return this.http.post<string>(`${this.apiUrl}/relaciones`, { componenteId, complejidadId, horas }).pipe(
          tap(() => this.relacionesCache$ = null)
        );
      })
    );
  }
// Crear relación simple (POST directo)
  createRelacion(componenteId: string, complejidadId: string, horas: number): Observable<string> {
    // 1. Quitar { responseType: 'text' }
    // 2. Esperar <any> (o <{id: string}>) porque el backend devuelve JSON
    // 3. Usar map para extraer el 'id' del objeto de respuesta JSON
    return this.http.post<any>(`${this.apiUrl}/relaciones`, { componenteId, complejidadId, horas }).pipe(
      map(response => response.id), // <-- Añadir map para extraer el ID
      tap(() => (this.relacionesCache$ = null))
    );
  }
  /** Actualizar relación explícitamente por id (opcional si ya usas upsert en POST) */
  updateRelacion(id: string, componenteId: string, complejidadId: string, horas: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/relaciones/${id}`, { id, componenteId, complejidadId, horas }).pipe(
      tap(() => this.relacionesCache$ = null)
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
        map((evaluaciones: Evaluacion[]) => evaluaciones.sort((a: Evaluacion, b: Evaluacion) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime()))
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
        map((proyectos: Proyecto[]) => proyectos.sort((a: Proyecto, b: Proyecto) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime()))
    );
  }
}