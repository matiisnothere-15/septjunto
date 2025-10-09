import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { DataService } from '../../../shared/data.service';

type ItemEval = {
  componenteId: string | null;
  descripcion: string;
  complejidadId: string | null;
};

@Component({
  selector: 'app-evaluaciones-crear',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './crear.component.html',
  styleUrls: ['./crear.component.scss'],
})
export class CrearComponent {
  // Cabecera
  nombreProyecto = '';
  touched = false;
  
  // Math para usar en template
  Math = Math;

  // Riesgo como string y getter normalizado
  deltaRiesgoPctStr = '';
  get deltaRiesgoPct(): number | undefined {
    const n = Number(this.deltaRiesgoPctStr);
    return Number.isFinite(n) ? Math.max(0, Math.min(100, Math.trunc(n))) : undefined;
  }

  // Catálogo desde DataService
  componentes: any[] = [];
  complejidades: any[] = [];

  // Grilla: SIEMPRE iniciamos con 1 fila vacía
  items: ItemEval[] = [
    { componenteId: null, descripcion: '', complejidadId: null }
  ];

  constructor(
    private dataService: DataService,
    private router: Router
  ) {
    this.loadCatalogos();
  }

  private loadCatalogos() {
    console.log('🔄 Cargando catálogos...');
    this.componentes = this.dataService.getComponentes();
    this.complejidades = this.dataService.getComplejidades();
    console.log(`✅ Componentes cargados: ${this.componentes.length}`);
    console.log(`✅ Complejidades cargadas: ${this.complejidades.length}`);
    
    // Si no hay datos, forzar recarga automática
    if (this.componentes.length === 0 || this.complejidades.length === 0) {
      console.log('⚠️ Datos faltantes, forzando recarga automática...');
      this.dataService.forzarRecargaComponentesBasicos();
      this.componentes = this.dataService.getComponentes();
      this.complejidades = this.dataService.getComplejidades();
      console.log(`✅ Recarga automática completada: ${this.componentes.length} componentes, ${this.complejidades.length} complejidades`);
    }
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
    this.items.push({ componenteId: null, descripcion: '', complejidadId: null });
    this.recalcularResumen(); // Actualizar resumen inmediatamente
  }

  // Insertar debajo de la fila i
  addItemBelow(i: number): void {
    this.items.splice(i + 1, 0, { componenteId: null, descripcion: '', complejidadId: null });
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

  // ===== Guardar =====
  crear(formRef: NgForm): void {
    this.touched = true;
    if (this.disableGuardar) return;

    try {
      const items = this.items
        .filter(it => it.componenteId && it.descripcion.trim() && it.complejidadId)
        .map(it => ({
          componenteId: it.componenteId!,
          complejidadId: it.complejidadId!,
          descripcionTarea: it.descripcion.trim()
        }));

      const evaluacionId = this.dataService.createEvaluacion(
        this.nombreProyecto.trim(),
        this.deltaRiesgoPct,
        items
      );

      // Navegar a la evaluación creada
      this.router.navigate(['/evaluaciones', evaluacionId]);
    } catch (error) {
      console.error('Error al crear evaluación:', error);
      // Aquí podrías mostrar un mensaje de error al usuario
    }
  }

  // Debug helper
  get itemsConDatos(): number {
    return this.items.filter(it => it.componenteId && it.complejidadId).length;
  }

  // ===== Resumen en Tiempo Real =====
  recalcularResumen(): void {
    // Forzar la detección de cambios de Angular
    // Esto hará que los getters se ejecuten nuevamente
    console.log('🔄 Resumen recalculado en tiempo real');
  }

  // Método para forzar recarga de componentes básicos
  forzarRecargaComponentes(): void {
    this.dataService.forzarRecargaComponentesBasicos();
    this.loadCatalogos(); // Recargar catálogos
    console.log('✅ Componentes básicos recargados');
  }

  // Método para forzar recarga completa
  forzarRecargaCompleta(): void {
    this.dataService.forzarRecargaCompleta();
    this.loadCatalogos(); // Recargar catálogos
    console.log('✅ Recarga completa realizada');
  }



}
