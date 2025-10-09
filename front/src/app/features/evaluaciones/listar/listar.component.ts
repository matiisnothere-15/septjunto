import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DataService } from '../../../shared/data.service';
import { Proyecto } from '../../../shared/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-evaluaciones-listar',
  standalone: true,
  imports: [CommonModule, RouterLink, AsyncPipe],
  templateUrl: './listar.component.html',
  styleUrls: ['./listar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ListarComponent {
  private dataService = inject(DataService);
  items$: Observable<Proyecto[]> = this.dataService.getProyectos();

  formatearFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}