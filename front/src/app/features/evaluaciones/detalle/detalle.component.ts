import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../../shared/data.service';
import { ExportService } from '../../../shared/export.service';
import { Evaluacion } from '../../../shared/models';

type ItemEval = {
  id: string;
  componenteId: string | null;
  descripcion: string;
  complejidadId: string | null;
};

@Component({
  selector: 'app-evaluaciones-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './detalle.component.html',
  styleUrls: ['./detalle.component.scss'],
})
export class DetalleComponent implements OnInit {
  evaluacion: Evaluacion | null = null;
  isEditing = false;
  touched = false;
  mostrarModalEliminar = false;
  
  // Math para usar en template
  Math = Math;

  // Datos editables
  nombreProyecto = '';
  deltaRiesgoPctStr = '';
  items: ItemEval[] = [];

  // Catálogo desde DataService
  componentes: any[] = [];
  complejidades: any[] = [];

  constructor(
    private dataService: DataService,
    private exportService: ExportService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEvaluacion(id);
    }
    this.loadCatalogos();
  }

  private loadCatalogos() {
    this.componentes = this.dataService.getComponentes();
    this.complejidades = this.dataService.getComplejidades();
  }

  private loadEvaluacion(id: string) {
    this.evaluacion = this.dataService.getEvaluacion(id) || null;
    if (this.evaluacion) {
      this.nombreProyecto = this.evaluacion.nombreProyecto;
      this.deltaRiesgoPctStr = this.evaluacion.deltaRiesgoPct?.toString() || '';
      this.items = this.evaluacion.detalles.map(d => ({
        id: d.id,
        componenteId: d.componenteId,
        descripcion: d.descripcionTarea,
        complejidadId: d.complejidadId
      }));
    }
  }

  // Riesgo como string y getter normalizado
  get deltaRiesgoPct(): number | undefined {
    const n = Number(this.deltaRiesgoPctStr);
    return Number.isFinite(n) ? Math.max(0, Math.min(100, Math.trunc(n))) : undefined;
  }

  // Estados UI
  get disableGuardar(): boolean {
    const nombreOk = this.nombreProyecto.trim().length > 0;
    const filasOk =
      this.items.length > 0 &&
      this.items.every(it =>
        it.componenteId &&
        it.descripcion.trim().length > 0 &&
        it.complejidadId
      );
    return !(nombreOk && filasOk);
  }

  // Agregar al final una fila vacía
  addItem(): void {
    this.items.push({ 
      id: `temp_${Date.now()}_${Math.random()}`, // ID temporal único
      componenteId: null, 
      descripcion: '', 
      complejidadId: null 
    });
    this.recalcularResumen(); // Actualizar resumen inmediatamente
  }

  // Insertar debajo de la fila i
  addItemBelow(i: number): void {
    this.items.splice(i + 1, 0, { 
      id: `temp_${Date.now()}_${Math.random()}`, // ID temporal único
      componenteId: null, 
      descripcion: '', 
      complejidadId: null 
    });
    this.recalcularResumen(); // Actualizar resumen inmediatamente
  }

  // Eliminar (dejando al menos una)
  removeItem(i: number): void {
    if (this.items.length > 1) this.items.splice(i, 1);
    this.recalcularResumen(); // Actualizar resumen inmediatamente
  }

  trackByIdx = (i: number) => i;

  // ===== Cálculos =====
  get totalHoras(): number {
    let total = 0;
    for (const it of this.items) {
      if (!it.componenteId || !it.complejidadId) continue;
      const base = this.dataService.horasDePar(it.componenteId, it.complejidadId);
      total += base;
    }
    return Math.round(total);
  }

  get totalConRiesgo(): number | null {
    if (this.deltaRiesgoPct === undefined) return null;
    return Math.round(this.totalHoras * (1 + this.deltaRiesgoPct / 100));
  }

  get diasEstimados(): number {
    // Si hay riesgo, usar horas con riesgo; sino usar horas base
    const horasParaCalcular = this.totalConRiesgo || this.totalHoras;
    return Math.ceil(horasParaCalcular / 6); // 6 horas = 1 día laboral
  }

  // Recalcular resumen para actualizar la vista
  recalcularResumen(): void {
    // Este método fuerza la recalculación de los getters
    console.log('🔄 Resumen recalculado en tiempo real');
  }

  // Obtener nombre del componente
  getNombreComponente(componenteId: string | null): string {
    if (!componenteId) return '';
    const componente = this.componentes.find(c => c.id === componenteId);
    return componente ? componente.nombre : '';
  }

  // Obtener nombre de la complejidad
  getNombreComplejidad(complejidadId: string | null): string {
    if (!complejidadId) return '';
    const complejidad = this.complejidades.find(c => c.id === complejidadId);
    return complejidad ? complejidad.nombre : '';
  }

  // Obtener clase CSS para el badge de complejidad basado en el orden
  getClaseComplejidad(complejidadId: string | null): string {
    if (!complejidadId) return '';
    const complejidad = this.complejidades.find(c => c.id === complejidadId);
    if (!complejidad) return '';
    
    // Mapear el orden de la complejidad a la clase CSS correspondiente
    switch (complejidad.orden) {
      case 1: return 'complexity-1'; // Muy Baja
      case 2: return 'complexity-2'; // Baja
      case 3: return 'complexity-3'; // Media
      case 4: return 'complexity-4'; // Alta
      case 5: return 'complexity-5'; // Muy Alta
      default: return 'complexity-3'; // Default a Media
    }
  }

  // Obtener horas de una tarea específica
  getHorasTarea(item: ItemEval): number {
    if (!item.componenteId || !item.complejidadId) return 0;
    return this.dataService.horasDePar(item.componenteId, item.complejidadId);
  }

  // Obtener días estimados de una tarea específica
  getDiasTarea(item: ItemEval): number {
    const horas = this.getHorasTarea(item);
    return Math.ceil(horas / 6); // 6 horas = 1 día laboral
  }

  // ===== Acciones =====
  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      // Restaurar datos originales
      if (this.evaluacion) {
        this.loadEvaluacion(this.evaluacion.id);
      }
    }
  }

  guardar(formRef: NgForm): void {
    this.touched = true;
    if (this.disableGuardar) return;

    try {
      if (!this.evaluacion) {
        throw new Error('No hay evaluación para actualizar');
      }

      // Preparar los items para la actualización
      const itemsParaActualizar = this.items
        .filter(it => it.componenteId && it.descripcion.trim() && it.complejidadId)
        .map(it => ({
          componenteId: it.componenteId!,
          complejidadId: it.complejidadId!,
          descripcionTarea: it.descripcion.trim()
        }));

      // Llamar al servicio para actualizar la evaluación
      this.dataService.updateEvaluacion(
        this.evaluacion.id,
        this.nombreProyecto.trim(),
        this.deltaRiesgoPct,
        itemsParaActualizar
      );
      
      console.log('✅ Evaluación actualizada correctamente');
      this.isEditing = false;
      
      // Recargar la evaluación para mostrar los cambios
      this.loadEvaluacion(this.evaluacion.id);
    } catch (error) {
      console.error('Error al guardar evaluación:', error);
      alert('Error al guardar la evaluación: ' + error);
    }
  }

  eliminar(): void {
    this.mostrarModalEliminar = true;
  }

  cerrarModal(): void {
    this.mostrarModalEliminar = false;
  }

  confirmarEliminar(): void {
    try {
      if (this.evaluacion) {
        this.dataService.deleteEvaluacion(this.evaluacion.id);
        console.log('✅ Evaluación eliminada correctamente');
        
        // Cerrar modal y navegar al inicio
        this.mostrarModalEliminar = false;
        this.router.navigate(['/']);
      }
    } catch (error) {
      console.error('Error al eliminar evaluación:', error);
    }
  }

  volver(): void {
    this.router.navigate(['/']);
  }

  // ===== Exportación =====
  descargarPDF(): void {
    if (this.evaluacion) {
      try {
        this.exportService.exportToPDF(this.evaluacion);
        console.log('✅ PDF descargado correctamente');
      } catch (error) {
        console.error('Error al descargar PDF:', error);
        alert('Error al generar el PDF');
      }
    }
  }
}
