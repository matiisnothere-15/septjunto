export interface Componente { id: string; nombre: string; descripcion?: string; activo: boolean; proyectoId?: string; }
export interface Complejidad { id: string; nombre: string; orden: number; activo: boolean; }
export interface RelacionCC { id: string; componenteId: string; complejidadId: string; horas: number; }
export interface EvaluacionDetalle { 
  id: string; 
  componenteId: string; 
  complejidadId: string; 
  horasBase: number;
  descripcionTarea: string;
}
export interface Evaluacion {
  id: string;
  fecha: string;
  nombreProyecto: string;
  deltaRiesgoPct?: number;
  detalles: EvaluacionDetalle[];
  horasTotales: number;
  horasTotalesConRiesgo?: number;
}

export interface Proyecto {
  id: string;
  nombre: string;
  descripcion: string;
  fecha: string;
  horasTotales: number;
  diasEstimados: number;
  riesgo: number;
  componentes: string[];
}
