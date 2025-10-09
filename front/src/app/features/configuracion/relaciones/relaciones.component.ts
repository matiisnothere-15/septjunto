import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/data.service';

@Component({
  selector: 'app-relaciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './relaciones.component.html',
  styleUrls: ['./relaciones.component.scss']
})
export class RelacionesComponent implements OnInit {
  relaciones: any[] = [];
  componentes: any[] = [];
  complejidades: any[] = [];
  
  componenteSeleccionado: string | null = null;
  complejidadSeleccionada: string | null = null;
  horas: number = 1;
  
  error = '';
  success = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.componentes = this.dataService.getComponentes();
    this.complejidades = this.dataService.getComplejidades().sort((a, b) => a.orden - b.orden);
    this.relaciones = this.dataService.getRelaciones();
  }

  guardarRelacion() {
    if (!this.componenteSeleccionado) {
      this.error = 'Debe seleccionar un componente';
      return;
    }

    if (!this.complejidadSeleccionada) {
      this.error = 'Debe seleccionar una complejidad';
      return;
    }

    if (this.horas <= 0) {
      this.error = 'Las horas deben ser mayores a 0';
      return;
    }

    try {
      this.dataService.upsertRelacion(
        this.componenteSeleccionado,
        this.complejidadSeleccionada,
        this.horas
      );
      
      this.limpiarFormulario();
      this.refresh();
      this.success = 'Relación guardada correctamente';
      this.error = '';
      
      setTimeout(() => this.success = '', 3000);
    } catch (error: any) {
      this.error = error.message || 'Error al guardar relación';
      this.success = '';
    }
  }

  eliminarRelacion(id: string) {
    try {
      this.dataService.deleteRelacion(id);
      this.refresh();
      this.success = 'Relación eliminada correctamente';
      this.error = '';
      
      setTimeout(() => this.success = '', 3000);
    } catch (error: any) {
      this.error = error.message || 'Error al eliminar relación';
      this.success = '';
    }
  }

  limpiarFormulario() {
    this.componenteSeleccionado = null;
    this.complejidadSeleccionada = null;
    this.horas = 1;
  }

  limpiarMensajes() {
    this.error = '';
    this.success = '';
  }

  nombreComponente(id: string): string {
    return this.dataService.nombreComponente(id) || 'N/A';
  }

  nombreComplejidad(id: string): string {
    return this.dataService.nombreComplejidad(id) || 'N/A';
  }
}
