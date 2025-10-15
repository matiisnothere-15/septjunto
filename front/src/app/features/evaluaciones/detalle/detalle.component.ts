import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { DataService } from '../../../shared/data.service';
import { ExportService } from '../../../shared/export.service';
import { Evaluacion, Componente, Complejidad, RelacionCC } from '../../../shared/models';
import { Observable, BehaviorSubject, combineLatest, of } from 'rxjs';
import { map, switchMap, catchError, tap, filter, startWith } from 'rxjs/operators';

interface ItemEval {
  id: string;
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
  evaluacion: Evaluacion;
  items: ItemEval[];
  nombreProyecto: string;
  deltaRiesgoPct: number | undefined;
  isEditing: boolean;
  touched: boolean;
  componentes: Componente[];
  complejidades: Complejidad[];
  relaciones: RelacionCC[];
  summary: Summary;
  isFormInvalid: boolean;
  loading: boolean;
  error: string | null;
  showDeleteModal: boolean;
}

@Component({
  selector: 'app-evaluaciones-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './detalle.component.html',
  styleUrls: ['./detalle.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DetalleComponent {
  private dataService = inject(DataService);
  private exportService = inject(ExportService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  private state = {
    isEditing$: new BehaviorSubject<boolean>(false),
    touched$: new BehaviorSubject<boolean>(false),
    showDeleteModal$: new BehaviorSubject<boolean>(false),
    nombreProyecto$: new BehaviorSubject<string>(''),
    deltaRiesgoPct$: new BehaviorSubject<number | undefined>(undefined),
    items$: new BehaviorSubject<ItemEval[]>([]),
  };

  private evaluacion$ = this.route.paramMap.pipe(
    map(params => params.get('id')),
    filter((id): id is string => !!id),
    switchMap(id => this.dataService.getEvaluacion(id).pipe(
      catchError(err => {
        console.error('Error fetching evaluation:', err);
        this.router.navigate(['/']);
        return of(null);
      })
    )),
    filter((evaluacion): evaluacion is Evaluacion => !!evaluacion),
    tap(evaluacion => this.resetFormState(evaluacion))
  );

  private componentes$ = this.dataService.getComponentes().pipe(catchError(() => of([] as Componente[])));
  private complejidades$ = this.dataService.getComplejidades().pipe(catchError(() => of([] as Complejidad[])));
  private relaciones$ = this.dataService.getRelaciones().pipe(catchError(() => of([] as RelacionCC[])));

  vm$: Observable<ViewModel>;

  constructor() {
    const summary$ = combineLatest([
      this.state.items$,
      this.state.deltaRiesgoPct$,
      this.relaciones$
    ]).pipe(
      map(([items, riesgo, relaciones]) => this.calculateSummary(items, riesgo, relaciones)),
      startWith({ totalHoras: 0, totalConRiesgo: null, diasEstimados: 0 })
    );

    this.vm$ = combineLatest({
      evaluacion: this.evaluacion$,
      isEditing: this.state.isEditing$,
      touched: this.state.touched$,
      showDeleteModal: this.state.showDeleteModal$,
      nombreProyecto: this.state.nombreProyecto$,
      items: this.state.items$,
      deltaRiesgoPct: this.state.deltaRiesgoPct$,
      componentes: this.componentes$,
      complejidades: this.complejidades$,
      relaciones: this.relaciones$,
      summary: summary$
    }).pipe(
      map(vm => ({
        ...vm,
        isFormInvalid: this.isFormInvalid(vm.nombreProyecto, vm.items),
        loading: false,
        error: null,
      }))
    );
  }

  toggleEdit(startEditing: boolean, currentEval: Evaluacion): void {
    this.state.isEditing$.next(startEditing);
    if (!startEditing) {
      this.resetFormState(currentEval);
    }
  }

  guardar(currentEval: Evaluacion): void {
    this.state.touched$.next(true);
    const nombreProyecto = this.state.nombreProyecto$.getValue();
    const items = this.state.items$.getValue();
    if (this.isFormInvalid(nombreProyecto, items)) return;

    const commandItems = this.state.items$.getValue()
      .map(it => ({
        componenteId: it.componenteId!,
        complejidadId: it.complejidadId!,
        descripcionTarea: it.descripcion.trim()
      }));

    this.dataService.updateEvaluacion(
      currentEval.id,
      nombreProyecto.trim(),
      this.state.deltaRiesgoPct$.getValue(),
      commandItems
    ).pipe(
      tap(() => this.state.isEditing$.next(false)),
      catchError(err => {
        console.error('Error al guardar evaluación:', err);
        return of(null);
      })
    ).subscribe();
  }

  showDeleteModal(show: boolean): void {
    this.state.showDeleteModal$.next(show);
  }

  confirmarEliminar(id: string): void {
    this.dataService.deleteEvaluacion(id).pipe(
      tap(() => {
        this.showDeleteModal(false);
        this.router.navigate(['/']);
      }),
      catchError(err => {
        console.error('Error al eliminar evaluación:', err);
        this.showDeleteModal(false);
        return of(null);
      })
    ).subscribe();
  }

  descargarPDF(evaluacion: Evaluacion): void {
    this.exportService.exportToPDF(evaluacion);
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

  onNombreProyectoChange(nombre: string) { this.state.nombreProyecto$.next(nombre); }
  onDeltaRiesgoChange(value: string) {
    const n = Number(value);
    const riesgo = Number.isFinite(n) ? Math.max(0, Math.min(100, Math.trunc(n))) : undefined;
    this.state.deltaRiesgoPct$.next(riesgo);
  }
  onItemChange() {
    this.state.items$.next(this.state.items$.getValue());
  }

  trackById = (i: number, item: ItemEval) => item.id;

  private resetFormState(evaluacion: Evaluacion): void {
    this.state.nombreProyecto$.next(evaluacion.nombreProyecto);
    this.state.deltaRiesgoPct$.next(evaluacion.deltaRiesgoPct);
    this.state.items$.next(evaluacion.detalles.map(d => ({
      id: d.id,
      componenteId: d.componenteId,
      descripcion: d.descripcionTarea,
      complejidadId: d.complejidadId
    })));
    this.state.touched$.next(false);
  }

  private createEmptyItem(): ItemEval {
    return { id: `temp_${Date.now()}`, componenteId: null, descripcion: '', complejidadId: null };
  }

  private isFormInvalid(nombreProyecto: string, items: ItemEval[]): boolean {
    return !nombreProyecto.trim() || !items.length || items.some(it => !it.componenteId || !it.descripcion.trim() || !it.complejidadId);
  }

  private calculateSummary(items: ItemEval[], riesgo: number | undefined, relaciones: RelacionCC[]): Summary {
    const totalHoras = items.reduce((total, it) => total + this.getHorasTarea(it, relaciones), 0);
    const totalConRiesgo = riesgo !== undefined ? Math.round(totalHoras * (1 + riesgo / 100)) : null;
    const diasEstimados = Math.ceil((totalConRiesgo ?? totalHoras) / 6);
    return { totalHoras: Math.round(totalHoras), totalConRiesgo, diasEstimados };
  }

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
}