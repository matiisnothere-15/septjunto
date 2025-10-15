import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/data.service';
import { Complejidad } from '../../../shared/models';

@Component({
  selector: 'app-complejidades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complejidades.component.html',
  styleUrls: ['./complejidades.component.scss']
})
export class ComplejidadesComponent implements OnInit {
  complejidades: Complejidad[] = [];
  nuevaComplejidad = '';
  nuevoOrden = 1;
  error = '';
  success = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.dataService.getComplejidades(true).subscribe({
      next: (items: Complejidad[]) => {
        this.complejidades = items.sort((a: Complejidad, b: Complejidad) => a.orden - b.orden);
        this.nuevoOrden = this.complejidades.length + 1;
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'No se pudieron cargar las complejidades';
      }
    });
  }

  agregarComplejidad() {
    if (!this.nuevaComplejidad.trim()) {
      this.error = 'El nombre de la complejidad es requerido';
      return;
    }

    if (this.nuevoOrden < 1) {
      this.error = 'El orden debe ser mayor a 0';
      return;
    }

    this.dataService.addComplejidad(this.nuevaComplejidad.trim(), this.nuevoOrden).subscribe({
      next: () => {
        this.nuevaComplejidad = '';
        this.nuevoOrden = this.complejidades.length + 1;
        this.refresh();
        this.success = 'Complejidad agregada correctamente';
        this.error = '';
        setTimeout(() => this.success = '', 3000);
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'Error al agregar complejidad';
        this.success = '';
      }
    });
  }

  eliminarComplejidad(id: string) {
    this.dataService.deleteComplejidad(id).subscribe({
      next: () => {
        this.refresh();
        this.success = 'Complejidad eliminada correctamente';
        this.error = '';
        setTimeout(() => this.success = '', 3000);
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'Error al eliminar complejidad';
        this.success = '';
      }
    });
  }

  limpiarMensajes() {
    this.error = '';
    this.success = '';
  }

  limpiarYRecrearDatos() {
    alert('Funcionalidad no disponible en esta versión. Por favor configure Componentes, Complejidades y Relaciones manualmente.');
  }
}
