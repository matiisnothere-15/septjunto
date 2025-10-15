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
    this.dataService.getComponentes(true).subscribe({
      next: (items) => this.componentes = items,
      error: (err) => console.error(err)
    });
    this.dataService.getComplejidades(true).subscribe({
      next: (items) => this.complejidades = items.sort((a: any, b: any) => a.orden - b.orden),
      error: (err) => console.error(err)
    });
    this.dataService.getRelaciones(true).subscribe({
      next: (items) => this.relaciones = items,
      error: (err) => console.error(err)
    });
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

    this.dataService.upsertRelacion(
      this.componenteSeleccionado!,
      this.complejidadSeleccionada!,
      this.horas
    ).subscribe({
      next: () => {
        this.limpiarFormulario();
        this.refresh();
        this.success = 'Relación guardada correctamente';
        this.error = '';
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        console.error(err);
        this.error = 'Error al guardar relación';
        this.success = '';
      }
    });
  }

  eliminarRelacion(id: string) {
    this.dataService.deleteRelacion(id).subscribe({
      next: () => {
        this.refresh();
        this.success = 'Relación eliminada correctamente';
        this.error = '';
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        console.error(err);
        this.error = 'Error al eliminar relación';
        this.success = '';
      }
    });
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
    return this.componentes.find(c => c.id === id)?.nombre || 'N/A';
  }

  nombreComplejidad(id: string): string {
    return this.complejidades.find(c => c.id === id)?.nombre || 'N/A';
  }
}
