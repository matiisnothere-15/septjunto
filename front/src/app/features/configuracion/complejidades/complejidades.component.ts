import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/data.service';

@Component({
  selector: 'app-complejidades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complejidades.component.html',
  styleUrls: ['./complejidades.component.scss']
})
export class ComplejidadesComponent implements OnInit {
  complejidades: any[] = [];
  nuevaComplejidad = '';
  nuevoOrden = 1;
  error = '';
  success = '';

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.complejidades = this.dataService.getComplejidades()
      .sort((a, b) => a.orden - b.orden);
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

    try {
      this.dataService.addComplejidad(this.nuevaComplejidad.trim(), this.nuevoOrden);
      this.nuevaComplejidad = '';
      this.nuevoOrden = this.complejidades.length + 1;
      this.refresh();
      this.success = 'Complejidad agregada correctamente';
      this.error = '';
      
      setTimeout(() => this.success = '', 3000);
    } catch (error: any) {
      this.error = error.message || 'Error al agregar complejidad';
      this.success = '';
    }
  }

  eliminarComplejidad(id: string) {
    try {
      this.dataService.deleteComplejidad(id);
      this.refresh();
      this.success = 'Complejidad eliminada correctamente';
      this.error = '';
      
      setTimeout(() => this.success = '', 3000);
    } catch (error: any) {
      this.error = error.message || 'Error al eliminar complejidad';
      this.success = '';
    }
  }

  limpiarMensajes() {
    this.error = '';
    this.success = '';
  }

  limpiarYRecrearDatos() {
    if (confirm('⚠️ ¿Estás seguro de que quieres limpiar y recrear todos los datos básicos?\n\nEsto eliminará:\n- Todas las complejidades personalizadas\n- Todas las relaciones\n- Todos los componentes personalizados\n\nSe recrearán los datos básicos del sistema.')) {
      try {
        this.dataService.limpiarYRecrearDatos();
        this.refresh();
        this.success = '✅ Datos básicos limpiados y recreados correctamente. Las relaciones ahora deberían funcionar.';
        this.error = '';
        
        setTimeout(() => this.success = '', 5000);
      } catch (error: any) {
        this.error = error.message || 'Error al limpiar y recrear datos';
        this.success = '';
      }
    }
  }
}
