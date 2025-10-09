import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgForOf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../shared/data.service';
import { Proyecto } from '../../shared/models';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [RouterLink, NgIf, NgForOf, FormsModule],
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.scss'],
})
export class InicioComponent {
  proyectos: Proyecto[] = [];

  // estado UI
  q = '';            // búsqueda
  vista = 'grid';    // grid | list (por si luego quieres)

  constructor(private dataService: DataService) {
    this.loadProyectos();
  }

  private loadProyectos() {
    this.proyectos = this.dataService.getProyectos();
  }

  // lista filtrada
  get items() {
    const term = this.q.trim().toLowerCase();
    return this.proyectos
      .filter(p => term ? p.nombre.toLowerCase().includes(term) : true);
  }

  crearNuevo() {
    // navegación directa a crear evaluación
    location.href = '/evaluaciones/crear';
  }

  // Formatear fecha para mostrar
  formatearFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
