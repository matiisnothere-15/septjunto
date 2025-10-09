import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { v4 as uuid } from 'uuid';
import { Componente, Complejidad, RelacionCC, Evaluacion, Proyecto } from './models';

const LS = {
  comp:  'sept_componentes',
  compx: 'sept_complejidades',
  rel:   'sept_relaciones',
  evals: 'sept_evaluaciones'
};

@Injectable({ providedIn: 'root' })
export class DataService {
  private componentes: Componente[] = [];
  private complejidades: Complejidad[] = [];
  private relaciones: RelacionCC[] = [];
  private evaluaciones: Evaluacion[] = [];

  // memoria en SSR (fallback cuando no hay localStorage)
  private memStore = new Map<string, string>();

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.load();

    // Siempre asegurar que los componentes básicos estén disponibles
    this.asegurarComponentesBasicos();
    
    // Migrar evaluaciones existentes si es necesario
    this.migrarEvaluacionesExistentes();
    
    this.save();
  }

  // Método para limpiar y recrear todos los datos básicos
  limpiarYRecrearDatos(): void {
    console.log('🧹 Limpiando y recreando todos los datos básicos...');
    
    // Limpiar arrays
    this.componentes = [];
    this.complejidades = [];
    this.relaciones = [];
    
    // Limpiar localStorage
    if (this.isBrowser) {
      localStorage.removeItem(LS.comp);
      localStorage.removeItem(LS.compx);
      localStorage.removeItem(LS.rel);
    }
    
    // Recrear desde cero
    this.asegurarComponentesBasicos();
    this.save();
    
    console.log('✅ Datos básicos recreados correctamente');
  }

  // Método para asegurar que siempre estén los componentes básicos
  private asegurarComponentesBasicos(): void {
    console.log('🔄 Verificando componentes básicos...');
    
    const componentesBasicos = [
      { nombre: 'Function',      descripcion: 'Azure Function',  activo: true },
      { nombre: 'Data Factory',  descripcion: 'Pipeline ADF',    activo: true },
      { nombre: 'API REST',      descripcion: 'Web API',         activo: true },
      { nombre: 'Base de Datos', descripcion: 'SQL/NoSQL',       activo: true },
      { nombre: 'Frontend',      descripcion: 'Angular/React',    activo: true },
      { nombre: 'Logic App',     descripcion: 'Azure Logic App', activo: true },
      { nombre: 'Power BI',      descripcion: 'Reportes BI',     activo: true },
      { nombre: 'Data Lake',     descripcion: 'Azure Data Lake', activo: true },
      { nombre: 'Event Hub',     descripcion: 'Azure Event Hub', activo: true },
      { nombre: 'Service Bus',   descripcion: 'Azure Service Bus', activo: true },
      { nombre: 'Key Vault',     descripcion: 'Azure Key Vault', activo: true },
      { nombre: 'App Service',   descripcion: 'Azure App Service', activo: true },
      { nombre: 'Container',     descripcion: 'Docker/Kubernetes', activo: true },
      { nombre: 'DevOps',        descripcion: 'CI/CD Pipeline',   activo: true },
      { nombre: 'Testing',       descripcion: 'Unit/Integration Tests', activo: true },
      { nombre: 'Documentación', descripcion: 'Técnica y Usuario', activo: true },
    ];

    const complejidadesBasicas = [
      { nombre: 'Muy Baja',  orden: 1, activo: true },
      { nombre: 'Baja',      orden: 2, activo: true },
      { nombre: 'Media',     orden: 3, activo: true },
      { nombre: 'Alta',      orden: 4, activo: true },
      { nombre: 'Muy Alta',  orden: 5, activo: true },
    ];

    let componentesAgregados = 0;
    let complejidadesAgregadas = 0;

    // Limpiar duplicados de componentes
    this.componentes = this.componentes.filter((comp, index, arr) => 
      arr.findIndex(c => c.nombre.toLowerCase() === comp.nombre.toLowerCase()) === index
    );

    // Limpiar duplicados de complejidades
    this.complejidades = this.complejidades.filter((comp, index, arr) => 
      arr.findIndex(c => c.nombre.toLowerCase() === comp.nombre.toLowerCase()) === index
    );

    // Agregar componentes faltantes
    componentesBasicos.forEach(compBasico => {
      if (!this.componentes.find(c => c.nombre.toLowerCase() === compBasico.nombre.toLowerCase())) {
        this.componentes.push({
          id: uuid(),
          ...compBasico
        });
        componentesAgregados++;
      }
    });

    // Agregar complejidades faltantes
    complejidadesBasicas.forEach(compBasica => {
      if (!this.complejidades.find(c => c.nombre.toLowerCase() === compBasica.nombre.toLowerCase())) {
        this.complejidades.push({
          id: uuid(),
          ...compBasica
        });
        complejidadesAgregadas++;
      }
    });

    // Limpiar relaciones duplicadas o inválidas
    this.relaciones = this.relaciones.filter(rel => 
      this.componentes.some(c => c.id === rel.componenteId) &&
      this.complejidades.some(c => c.id === rel.complejidadId)
    );

    // Recrear relaciones si es necesario
    if (this.relaciones.length === 0) {
      console.log('🔄 Creando relaciones entre componentes y complejidades...');
      this.relaciones = [];
      this.componentes.forEach(componente => {
        this.complejidades.forEach(complejidad => {
          // Horas base según orden de complejidad: 1=2h, 2=4h, 3=8h, 4=16h, 5=32h
          const horas = Math.pow(2, complejidad.orden);
          this.relaciones.push({
            id: uuid(),
            componenteId: componente.id,
            complejidadId: complejidad.id,
            horas
          });
        });
      });
    }

    console.log(`✅ Componentes: ${this.componentes.length} (${componentesAgregados} nuevos)`);
    console.log(`✅ Complejidades: ${this.complejidades.length} (${complejidadesAgregadas} nuevas)`);
    console.log(`✅ Relaciones: ${this.relaciones.length}`);
    
    // Verificar que todas las relaciones estén correctas
    const relacionesEsperadas = this.componentes.length * this.complejidades.length;
    if (this.relaciones.length !== relacionesEsperadas) {
      console.warn(`⚠️ Relaciones inconsistentes: ${this.relaciones.length} vs ${relacionesEsperadas} esperadas`);
      console.log('🔄 Recreando todas las relaciones...');
      this.relaciones = [];
      this.componentes.forEach(componente => {
        this.complejidades.forEach(complejidad => {
          const horas = Math.pow(2, complejidad.orden);
          this.relaciones.push({
            id: uuid(),
            componenteId: componente.id,
            complejidadId: complejidad.id,
            horas
          });
        });
      });
      this.save();
    }
    
    // Mostrar información de debug
    this.mostrarDebugInfo();
  }
  
  // Método para mostrar información de debug
  mostrarDebugInfo(): void {
    console.log('🔍 === DEBUG INFO ===');
    console.log('Componentes:', this.componentes.map(c => ({ id: c.id, nombre: c.nombre })));
    console.log('Complejidades:', this.complejidades.map(c => ({ id: c.id, nombre: c.nombre, orden: c.orden })));
    console.log('Relaciones:', this.relaciones.map(r => ({
      id: r.id,
      componente: this.nombreComponente(r.componenteId),
      complejidad: this.nombreComplejidad(r.complejidadId),
      horas: r.horas
    })));
    console.log('===================');
  }

  // ===== Helpers SSR-safe =====
  private get isBrowser(): boolean {
    return isPlatformBrowser(this.platformId) && typeof localStorage !== 'undefined';
  }
  private lsGet(key: string): string | null {
    try {
      return this.isBrowser ? localStorage.getItem(key) : (this.memStore.get(key) ?? null);
    } catch { return null; }
  }
  private lsSet(key: string, val: string): void {
    try {
      if (this.isBrowser) localStorage.setItem(key, val);
      else this.memStore.set(key, val);
    } catch {}
  }

  // ---- persistencia
  private load() {
    try {
      console.log('📥 Cargando datos desde almacenamiento...');
      
      const compData = this.lsGet(LS.comp);
      const compxData = this.lsGet(LS.compx);
      const relData = this.lsGet(LS.rel);
      const evalsData = this.lsGet(LS.evals);
      
      this.componentes = compData ? JSON.parse(compData) : [];
      this.complejidades = compxData ? JSON.parse(compxData) : [];
      this.relaciones = relData ? JSON.parse(relData) : [];
      this.evaluaciones = evalsData ? JSON.parse(evalsData) : [];
      
      console.log(`✅ Datos cargados: ${this.componentes.length} componentes, ${this.complejidades.length} complejidades, ${this.relaciones.length} relaciones`);
      
    } catch (error) {
      console.error('❌ Error al cargar datos:', error);
      console.log('🔄 Inicializando arrays vacíos...');
      this.componentes = [];
      this.complejidades = [];
      this.relaciones = [];
      this.evaluaciones = [];
    }
  }
  private save() {
    try {
      console.log('💾 Guardando datos en almacenamiento...');
      
      this.lsSet(LS.comp,  JSON.stringify(this.componentes));
      this.lsSet(LS.compx, JSON.stringify(this.complejidades));
      this.lsSet(LS.rel,   JSON.stringify(this.relaciones));
      this.lsSet(LS.evals, JSON.stringify(this.evaluaciones));
      
      console.log(`✅ Datos guardados: ${this.componentes.length} componentes, ${this.complejidades.length} complejidades, ${this.relaciones.length} relaciones`);
      
    } catch (error) {
      console.error('❌ Error al guardar datos:', error);
    }
  }

  // ---- Componentes
  getComponentes(): Componente[] { return [...this.componentes]; }
  addComponente(nombre: string, descripcion = '') {
    const nuevo: Componente = { id: uuid(), nombre, descripcion, activo: true };
    this.componentes.push(nuevo); this.save();
  }
  updateComponente(id: string, nombre: string, descripcion?: string) {
    const componente = this.componentes.find(c => c.id === id);
    if (componente) {
      componente.nombre = nombre;
      if (descripcion !== undefined) {
        componente.descripcion = descripcion;
      }
      this.save();
    } else {
      throw new Error('Componente no encontrado');
    }
  }
  deleteComponente(id: string) {
    // Eliminar el componente
    this.componentes = this.componentes.filter(c => c.id !== id);
    
    // Eliminar todas las relaciones asociadas a este componente
    this.relaciones = this.relaciones.filter(r => r.componenteId !== id);
    
    // Eliminar evaluaciones que usen este componente
    this.evaluaciones = this.evaluaciones.filter(evaluacion => {
      evaluacion.detalles = evaluacion.detalles.filter(detalle => detalle.componenteId !== id);
      return evaluacion.detalles.length > 0; // Mantener solo evaluaciones que tengan detalles
    });
    
    this.save();
  }

  // ---- Complejidades
  getComplejidades(): Complejidad[] {
    return [...this.complejidades].sort((a, b) => a.orden - b.orden);
  }
  addComplejidad(nombre: string, orden: number) {
    if (!nombre.trim()) return;
    if (this.complejidades.some(c => c.nombre.toLowerCase() === nombre.toLowerCase())) {
      throw new Error('Ya existe una complejidad con ese nombre.');
    }
    this.complejidades.push({ id: uuid(), nombre, orden, activo: true });
    this.save();
  }
  deleteComplejidad(id: string) {
    if (this.relaciones.some(r => r.complejidadId === id)) {
      throw new Error('No se puede eliminar: complejidad en uso en relaciones.');
    }
    this.complejidades = this.complejidades.filter(c => c.id !== id); this.save();
  }
  // ---- Relaciones (Componente × Complejidad → Horas)
  getRelaciones(): RelacionCC[] { return [...this.relaciones]; }
  upsertRelacion(componenteId: string, complejidadId: string, horas: number) {
    if (!horas || horas <= 0) throw new Error('Horas debe ser > 0');
    const idx = this.relaciones.findIndex(r => r.componenteId === componenteId && r.complejidadId === complejidadId);
    if (idx >= 0) this.relaciones[idx].horas = horas;
    else this.relaciones.push({ id: uuid(), componenteId, complejidadId, horas });
    this.save();
  }
  deleteRelacion(id: string) {
    this.relaciones = this.relaciones.filter(r => r.id !== id); this.save();
  }
  horasDePar(componenteId: string, complejidadId: string): number {
    return this.relaciones.find(r => r.componenteId === componenteId && r.complejidadId === complejidadId)?.horas ?? 0;
  }
  nombreComponente(id: string) { 
    if (!id) return '—';
    const componente = this.componentes.find(c => c.id === id);
    return componente ? componente.nombre : 'Componente no encontrado';
  }
  
  nombreComplejidad(id: string) { 
    if (!id) return '—';
    const complejidad = this.complejidades.find(c => c.id === id);
    return complejidad ? complejidad.nombre : 'Complejidad no encontrada';
  }

  // ---- Evaluaciones
  getEvaluaciones(): Evaluacion[] {
    return [...this.evaluaciones].sort((a, b) => b.fecha.localeCompare(a.fecha));
  }
  
  createEvaluacion(
    nombreProyecto: string,
    deltaRiesgoPct: number | undefined,
    items: { componenteId: string; complejidadId: string; descripcionTarea: string; }[]
  ) {
    const detalles = items.map(it => ({
      id: uuid(),
      componenteId: it.componenteId,
      complejidadId: it.complejidadId,
      horasBase: this.horasDePar(it.componenteId, it.complejidadId),
      descripcionTarea: it.descripcionTarea
    }));
    
    const horasTotales = detalles.reduce((s, d) => s + d.horasBase, 0);
    
    // Calcular horas con riesgo de manera consistente
    let horasTotalesConRiesgo: number | undefined;
    if (deltaRiesgoPct && deltaRiesgoPct > 0) {
      // Redondear al entero más cercano para consistencia con el PDF
      horasTotalesConRiesgo = Math.round(horasTotales * (1 + deltaRiesgoPct / 100));
    }

    const ev: Evaluacion = {
      id: uuid(),
      fecha: new Date().toISOString(),
      nombreProyecto,
      deltaRiesgoPct,
      detalles,
      horasTotales,
      horasTotalesConRiesgo
    };
    this.evaluaciones.unshift(ev); this.save();
    return ev.id;
  }
  
  getEvaluacion(id: string) { return this.evaluaciones.find(e => e.id === id); }

  // Eliminar evaluación
  deleteEvaluacion(id: string): void {
    const index = this.evaluaciones.findIndex(e => e.id === id);
    if (index !== -1) {
      this.evaluaciones.splice(index, 1);
      this.save();
      console.log(`✅ Evaluación eliminada: ${id}`);
    }
  }

  // Actualizar evaluación existente
  updateEvaluacion(
    id: string,
    nombreProyecto: string,
    deltaRiesgoPct: number | undefined,
    items: { componenteId: string; complejidadId: string; descripcionTarea: string; }[]
  ): void {
    const index = this.evaluaciones.findIndex(e => e.id === id);
    if (index === -1) {
      throw new Error('Evaluación no encontrada');
    }

    // Mantener la evaluación existente y solo actualizar los campos necesarios
    const evaluacionExistente = this.evaluaciones[index];
    
    const detalles = items.map(it => ({
      id: uuid(), // Nuevo ID para cada detalle
      componenteId: it.componenteId,
      complejidadId: it.complejidadId,
      horasBase: this.horasDePar(it.componenteId, it.complejidadId),
      descripcionTarea: it.descripcionTarea
    }));

    const horasTotales = detalles.reduce((s, d) => s + d.horasBase, 0);
    
    // Calcular horas con riesgo de manera consistente
    let horasTotalesConRiesgo: number | undefined;
    if (deltaRiesgoPct && deltaRiesgoPct > 0) {
      // Redondear al entero más cercano para consistencia con el PDF
      horasTotalesConRiesgo = Math.round(horasTotales * (1 + deltaRiesgoPct / 100));
    }

    // Actualizar solo los campos que cambian, manteniendo ID y fecha original
    this.evaluaciones[index] = {
      id: evaluacionExistente.id, // Mantener ID original
      fecha: evaluacionExistente.fecha, // Mantener fecha original
      nombreProyecto,
      deltaRiesgoPct,
      detalles,
      horasTotales,
      horasTotalesConRiesgo
    };

    this.save();
    console.log(`✅ Evaluación actualizada: ${id}`);
  }

    // ---- Proyectos (convertidos desde evaluaciones)
  getProyectos(): Proyecto[] {
    return this.evaluaciones.map(evaluacion => {
      // Calcular días de manera consistente
      const horasBase = evaluacion.horasTotales || 0;
      const riesgo = evaluacion.deltaRiesgoPct || 0;
      
      // Calcular horas con riesgo (solo si hay riesgo)
      let horasConRiesgo = horasBase;
      if (riesgo > 0) {
        horasConRiesgo = Math.round(horasBase * (1 + riesgo / 100));
      }
      
      // Calcular días estimados (6 horas = 1 día laboral)
      const diasEstimados = Math.ceil(horasConRiesgo / 6);
      
      // Mapear componentes con manejo de errores
      const componentes = evaluacion.detalles.map(d => {
        const nombre = this.nombreComponente(d.componenteId);
        return nombre !== '—' ? nombre : 'Componente no encontrado';
      });
      
      return {
        id: evaluacion.id,
        nombre: evaluacion.nombreProyecto,
        descripcion: evaluacion.detalles.map(d => d.descripcionTarea).join(', '),
        fecha: evaluacion.fecha,
        horasTotales: horasBase,
        diasEstimados,
        riesgo,
        componentes
      };
    }).sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime());
  }

  // Método para limpiar datos y forzar recarga
  clearAndReload(): void {
    try {
      console.log('🧹 Limpiando todos los datos...');
      
      if (this.isBrowser) {
        localStorage.removeItem(LS.comp);
        localStorage.removeItem(LS.compx);
        localStorage.removeItem(LS.rel);
        localStorage.removeItem(LS.evals);
        console.log('✅ localStorage limpiado');
      }
      
      this.memStore.clear();
      console.log('✅ Memoria local limpiada');
      
      // Recargar datos
      this.load();
      this.asegurarComponentesBasicos();
      this.save();
      
      console.log('✅ Datos limpiados y recargados desde cero');
    } catch (error) {
      console.error('❌ Error al limpiar datos:', error);
    }
  }

  // Método para recalcular todas las evaluaciones existentes
  recalcularEvaluaciones(): void {
    let recalculadas = 0;
    
    this.evaluaciones.forEach(evaluacion => {
      // Recalcular horas totales
      const horasTotales = evaluacion.detalles.reduce((s, d) => s + d.horasBase, 0);
      
      // Recalcular horas con riesgo
      let horasTotalesConRiesgo: number | undefined;
      if (evaluacion.deltaRiesgoPct && evaluacion.deltaRiesgoPct > 0) {
        horasTotalesConRiesgo = Math.round(horasTotales * (1 + evaluacion.deltaRiesgoPct / 100));
      }
      
      // Actualizar si hay cambios
      if (evaluacion.horasTotales !== horasTotales || evaluacion.horasTotalesConRiesgo !== horasTotalesConRiesgo) {
        evaluacion.horasTotales = horasTotales;
        evaluacion.horasTotalesConRiesgo = horasTotalesConRiesgo;
        recalculadas++;
      }
    });
    
    if (recalculadas > 0) {
      this.save();
      console.log(`✅ Recalculadas ${recalculadas} evaluaciones`);
    } else {
      console.log('✅ Todas las evaluaciones ya están actualizadas');
    }
  }

  // Método para forzar la recarga de componentes básicos
  forzarRecargaComponentesBasicos(): void {
    console.log('🔄 Forzando recarga de componentes básicos...');
    this.asegurarComponentesBasicos();
    this.save();
    console.log('✅ Componentes básicos recargados');
  }

  // Método para forzar recarga completa (más agresivo)
  forzarRecargaCompleta(): void {
    console.log('🔄 Forzando recarga completa de datos básicos...');
    
    // Limpiar arrays existentes
    this.componentes = [];
    this.complejidades = [];
    this.relaciones = [];
    
    // Forzar recreación completa
    this.asegurarComponentesBasicos();
    this.save();
    
    console.log('✅ Recarga completa realizada');
  }

  // Método para verificar integridad de datos
  verificarIntegridadDatos(): void {
    console.log('🔍 Verificando integridad de datos...');
    console.log(`- Componentes en memoria: ${this.componentes.length}`);
    console.log(`- Complejidades en memoria: ${this.complejidades.length}`);
    console.log(`- Relaciones en memoria: ${this.relaciones.length}`);
    
    // Verificar localStorage
    if (this.isBrowser) {
      const compLS = localStorage.getItem(LS.comp);
      const compxLS = localStorage.getItem(LS.compx);
      const relLS = localStorage.getItem(LS.rel);
      
      console.log(`- Componentes en localStorage: ${compLS ? JSON.parse(compLS).length : 0}`);
      console.log(`- Complejidades en localStorage: ${compxLS ? JSON.parse(compxLS).length : 0}`);
      console.log(`- Relaciones en localStorage: ${relLS ? JSON.parse(relLS).length : 0}`);
    }
    
    // Verificar que las relaciones sean válidas
    const relacionesInvalidas = this.relaciones.filter(r => 
      !this.componentes.find(c => c.id === r.componenteId) ||
      !this.complejidades.find(c => c.id === r.complejidadId)
    );
    
    if (relacionesInvalidas.length > 0) {
      console.log(`⚠️ Relaciones inválidas encontradas: ${relacionesInvalidas.length}`);
      console.log('Relaciones inválidas:', relacionesInvalidas);
    } else {
      console.log('✅ Todas las relaciones son válidas');
    }
  }

  // Método para migrar evaluaciones existentes con componentes antiguos
  private migrarEvaluacionesExistentes(): void {
    if (this.evaluaciones.length === 0) return;

    let migradas = 0;
    let recalculadas = 0;
    
    this.evaluaciones.forEach(evaluacion => {
      // Migrar componentes si es necesario
      evaluacion.detalles.forEach(detalle => {
        if (!this.componentes.find(c => c.id === detalle.componenteId)) {
          const componenteSimilar = this.componentes.find(c => 
            c.nombre.toLowerCase() === this.nombreComponente(detalle.componenteId).toLowerCase()
          );
          
          if (componenteSimilar) {
            detalle.componenteId = componenteSimilar.id;
            migradas++;
          }
        }
      });
      
      // Recalcular horas totales y con riesgo para consistencia
      const horasTotales = evaluacion.detalles.reduce((s, d) => s + d.horasBase, 0);
      let horasTotalesConRiesgo: number | undefined;
      
      if (evaluacion.deltaRiesgoPct && evaluacion.deltaRiesgoPct > 0) {
        horasTotalesConRiesgo = Math.round(horasTotales * (1 + evaluacion.deltaRiesgoPct / 100));
      }
      
      // Solo actualizar si hay cambios
      if (evaluacion.horasTotales !== horasTotales || evaluacion.horasTotalesConRiesgo !== horasTotalesConRiesgo) {
        evaluacion.horasTotales = horasTotales;
        evaluacion.horasTotalesConRiesgo = horasTotalesConRiesgo;
        recalculadas++;
      }
    });

    if (migradas > 0 || recalculadas > 0) {
      console.log(`✅ Migradas ${migradas} referencias de componentes, recalculadas ${recalculadas} evaluaciones`);
      this.save();
    }
  }
}
