import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DataService } from '../../../shared/data.service';
import { Componente, Complejidad, RelacionCC, Proyecto } from '../../../shared/models';
import { Observable, BehaviorSubject, combineLatest, of, Subject } from 'rxjs';
import { map, startWith, switchMap, catchError, tap, takeUntil } from 'rxjs/operators';

interface ItemEval {
  id: number; // Unique ID for trackBy
  componenteId: string | null;
  descripcion: string;
  complejidadId: string | null;
}

interface Summary {
  totalHoras: number;
  totalConRiesgo: number | null;
  diasEstimados: number;
}

interface ViewModel {
  items: ItemEval[];
  selectedProyectoNombre: string | null;
  deltaRiesgoPct: number | undefined;
  touched: boolean;
  proyectos: Proyecto[];
  componentes: Componente[];
  complejidades: Complejidad[];
  relaciones: RelacionCC[];
  summary: Summary;
  isFormInvalid: boolean;
  loading: boolean;
  error: string | null;
  showCrearProyectoModal: boolean;
}

@Component({
  selector: 'app-evaluaciones-crear',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './crear.component.html',
  styleUrls: ['./crear.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CrearComponent {
  private dataService = inject(DataService);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  private state = {
    items$: new BehaviorSubject<ItemEval[]>([this.createEmptyItem()]),
    selectedProyectoNombre$: new BehaviorSubject<string | null>(null),
    deltaRiesgoPct$: new BehaviorSubject<number | undefined>(undefined),
    touched$: new BehaviorSubject<boolean>(false),
    proyectos$: new BehaviorSubject<Proyecto[]>([]),
    showCrearProyectoModal$: new BehaviorSubject<boolean>(false),
  };

  vm$: Observable<ViewModel>;

  constructor() {
    // cargar proyectos
    this.dataService.getProyectos().pipe(
      tap(proyectos => this.state.proyectos$.next(proyectos)),
      catchError(() => of([] as Proyecto[])),
      takeUntil(this.destroy$)
    ).subscribe();

    const componentes$ = this.dataService.getComponentes().pipe(catchError(() => of([] as Componente[])));
    const complejidades$ = this.dataService.getComplejidades().pipe(catchError(() => of([] as Complejidad[])));
    const relaciones$ = this.dataService.getRelaciones().pipe(catchError(() => of([] as RelacionCC[])));

    const summary$ = combineLatest([
      this.state.items$,
      this.state.deltaRiesgoPct$,
      relaciones$
    ]).pipe(
      map(([items, riesgo, relaciones]) => this.calculateSummary(items, riesgo, relaciones)),
      startWith({ totalHoras: 0, totalConRiesgo: null, diasEstimados: 0 })
    );

    this.vm$ = combineLatest({
      items: this.state.items$,
      selectedProyectoNombre: this.state.selectedProyectoNombre$,
      deltaRiesgoPct: this.state.deltaRiesgoPct$,
      touched: this.state.touched$,
      proyectos: this.state.proyectos$,
      componentes: componentes$,
      complejidades: complejidades$,
      relaciones: relaciones$,
      summary: summary$,
      showCrearProyectoModal: this.state.showCrearProyectoModal$,
    }).pipe(
      map(vm => ({
        ...vm,
        isFormInvalid: this.isFormInvalid(vm.selectedProyectoNombre, vm.items),
        loading: false,
        error: null,
      }))
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onProyectoSeleccionado(nombre: string | null) { this.state.selectedProyectoNombre$.next(nombre); }
  onDeltaRiesgoChange(value: string) {
    const n = Number(value);
    const riesgo = Number.isFinite(n) ? Math.max(0, Math.min(100, Math.trunc(n))) : undefined;
    this.state.deltaRiesgoPct$.next(riesgo);
  }

  onItemChange() {
    this.state.items$.next(this.state.items$.getValue());
  }

  addItem(i: number): void {
    const currentItems = this.state.items$.getValue();
    currentItems.splice(i + 1, 0, this.createEmptyItem());
    this.state.items$.next(currentItems);
  }

  removeItem(i: number): void {
    const currentItems = this.state.items$.getValue();
    if (currentItems.length > 1) {
      currentItems.splice(i, 1);
      this.state.items$.next(currentItems);
    }
  }

  // Modal crear proyecto
  abrirModalCrearProyecto(): void {
    this.nuevoNombreProyecto = '';
    this.state.showCrearProyectoModal$.next(true);
  }

  cerrarModalCrearProyecto(): void {
    this.state.showCrearProyectoModal$.next(false);
  }

  nuevoNombreProyecto: string = '';

  guardarNuevoProyecto(): void {
    const nombre = (this.nuevoNombreProyecto || '').trim();
    if (!nombre || nombre.length < 3) {
      alert('El nombre del proyecto debe tener al menos 3 caracteres.');
      return;
    }
    this.dataService.createProyecto(nombre).pipe(
      tap(nuevo => {
        const curr = this.state.proyectos$.getValue();
        this.state.proyectos$.next([...curr, nuevo].sort((a,b) => a.nombre.localeCompare(b.nombre)));
        this.state.selectedProyectoNombre$.next(nuevo.nombre);
        this.cerrarModalCrearProyecto();
      }),
      catchError(err => { console.error('Error al crear proyecto', err); alert(`Error: ${err.error?.message || err.message}`); return of(null); }),
      takeUntil(this.destroy$)
    ).subscribe();
  }

  crear(): void {
    this.state.touched$.next(true);
    const nombreProyecto = this.state.selectedProyectoNombre$.getValue();
    const items = this.state.items$.getValue();

    if (this.isFormInvalid(nombreProyecto, items)) return;

    if (!nombreProyecto) { console.error('Proyecto no seleccionado'); return; }

    const commandItems = items
      .filter(it => it.componenteId && it.descripcion.trim() && it.complejidadId)
      .map(it => ({
        componenteId: it.componenteId!,
        complejidadId: it.complejidadId!,
        descripcionTarea: it.descripcion.trim()
      }));

    this.dataService.createEvaluacion(
      nombreProyecto.trim(),
      this.state.deltaRiesgoPct$.getValue(),
      commandItems
    ).pipe(
      tap(evaluacionId => this.router.navigate(['/evaluaciones', evaluacionId])),
      catchError(err => {
        console.error('Error al crear evaluación:', err);
        alert(`Error al guardar la evaluación: ${err.error?.title || err.error?.message || err.message}`);
        return of(null);
      })
    ).subscribe();
  }

  trackById = (i: number, item: ItemEval) => item.id;

  // --- Template Helpers ---
  getHorasTarea(item: ItemEval, relaciones: RelacionCC[]): number {
    if (!item.componenteId || !item.complejidadId) return 0;
    const rel = relaciones.find(r => r.componenteId === item.componenteId && r.complejidadId === item.complejidadId);
    return rel ? rel.horas : 0;
  }

  getDiasTarea(item: ItemEval, relaciones: RelacionCC[]): number {
    return Math.ceil(this.getHorasTarea(item, relaciones) / 6);
  }

  getNombreComponente(componenteId: string | null, componentes: Componente[]): string {
    const comp = componentes.find(c => c.id === componenteId);
    return comp ? comp.nombre : '';
  }

  getNombreComplejidad(complejidadId: string | null, complejidades: Complejidad[]): string {
    const compx = complejidades.find(c => c.id === complejidadId);
    return compx ? compx.nombre : '';
  }

  getClaseComplejidad(complejidadId: string | null, complejidades: Complejidad[]): string {
    const compx = complejidades.find(c => c.id === complejidadId);
    if (!compx) return '';
    return `complexity-${compx.orden}`;
  }

  // --- Private Helpers ---
  private createEmptyItem(): ItemEval {
    return { id: Date.now() + Math.random(), componenteId: null, descripcion: '', complejidadId: null };
  }

  private isFormInvalid(selectedProyectoNombre: string | null, items: ItemEval[]): boolean {
    const isProyectoInvalid = !selectedProyectoNombre;
    const areItemsInvalid = !items.length || items.some(it => {
      const d = it.descripcion.trim();
      return !it.componenteId || !it.complejidadId || !d || d.length < 10;
    });
    return isProyectoInvalid || areItemsInvalid;
  }

  private calculateSummary(items: ItemEval[], riesgo: number | undefined, relaciones: RelacionCC[]): Summary {
    const totalHoras = items.reduce((total, it) => total + this.getHorasTarea(it, relaciones), 0);
    const totalConRiesgo = riesgo !== undefined ? Math.round(totalHoras * (1 + riesgo / 100)) : null;
    const diasEstimados = Math.ceil((totalConRiesgo ?? totalHoras) / 6);
    return { totalHoras: Math.round(totalHoras), totalConRiesgo, diasEstimados };
  }
}