import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgForOf, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../shared/data.service';
import { Proyecto } from '../../shared/models';
import { Observable, BehaviorSubject, combineLatest } from 'rxjs';
import { map, startWith, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [RouterLink, NgIf, NgForOf, FormsModule, AsyncPipe],
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InicioComponent {

  private searchTerm = new BehaviorSubject<string>('');

  proyectos$: Observable<Proyecto[]>;

  q = ''; // ngModel for search input
  vista = 'grid';

  constructor(private dataService: DataService) {
    const allProyectos$ = this.dataService.getProyectos();

    const searchTerm$ = this.searchTerm.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      startWith('')
    );

    this.proyectos$ = combineLatest([allProyectos$, searchTerm$]).pipe(
      map(([proyectos, term]) =>
        proyectos.filter(p => term ? p.nombre.toLowerCase().includes(term.toLowerCase()) : true)
      )
    );
  }

  onSearchChange(term: string): void {
    this.searchTerm.next(term);
  }

  crearNuevo() {
    location.href = '/evaluaciones/crear';
  }

  formatearFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}