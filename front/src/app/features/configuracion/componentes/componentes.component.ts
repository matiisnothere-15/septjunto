import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../shared/data.service';
import { Componente } from '../../../shared/models';

@Component({
  selector: 'app-componentes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './componentes.component.html',
  styleUrls: ['./componentes.component.scss']
})
export class ComponentesComponent implements OnInit {
  componentes: Componente[] = [];
  componentesPaginados: Componente[] = [];
  nuevoComponente = '';
  componenteEditando: any = null;
  error = '';
  success = '';
  mostrarFormulario = false;
  editando = false;
  
  // Paginación
  paginaActual = 1;
  elementosPorPagina = 6;
  totalPaginas = 0;
  
  // Math para usar en template
  Math = Math;

  constructor(private dataService: DataService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.dataService.getComponentes(true).subscribe({
      next: (items: Componente[]) => {
        this.componentes = items;
        this.paginaActual = 1;
        this.calcularPaginacion();
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'No se pudieron cargar los componentes';
      }
    });
  }

  calcularPaginacion() {
    this.totalPaginas = Math.ceil(this.componentes.length / this.elementosPorPagina);
    const inicio = (this.paginaActual - 1) * this.elementosPorPagina;
    const fin = inicio + this.elementosPorPagina;
    this.componentesPaginados = this.componentes.slice(inicio, fin);
  }

  cambiarPagina(pagina: number) {
    if (pagina >= 1 && pagina <= this.totalPaginas) {
      this.paginaActual = pagina;
      this.calcularPaginacion();
    }
  }

  get paginasVisibles(): number[] {
    const paginas: number[] = [];
    const inicio = Math.max(1, this.paginaActual - 2);
    const fin = Math.min(this.totalPaginas, this.paginaActual + 2);
    
    for (let i = inicio; i <= fin; i++) {
      paginas.push(i);
    }
    return paginas;
  }

  agregarComponente() {
    if (!this.nuevoComponente.trim()) {
      this.error = 'El nombre del componente es requerido';
      return;
    }

    if (this.editando && this.componenteEditando) {
      // Actualizar
      const nombre = this.nuevoComponente.trim();
      // Si el UI no soporta seleccionar proyecto aún, mantener el proyecto actual del componente si existe
      const proyectoId = (this.componenteEditando as any)?.proyectoId;
      this.dataService.updateComponente(this.componenteEditando.id, nombre, undefined, proyectoId).subscribe({
        next: () => {
          this.success = 'Componente actualizado correctamente';
          this.limpiarFormulario();
          this.refresh();
          this.error = '';
          setTimeout(() => this.success = '', 3000);
        },
        error: (err: unknown) => {
          console.error(err);
          this.error = this.formatHttpError(err) || 'Error al actualizar componente';
          this.success = '';
        }
      });
    } else {
      // Crear
      const nombre = this.nuevoComponente.trim();
      this.dataService.addComponente(nombre).subscribe({
        next: () => {
          this.success = 'Componente agregado correctamente';
          this.limpiarFormulario();
          this.refresh();
          this.error = '';
          setTimeout(() => this.success = '', 3000);
        },
        error: (err: unknown) => {
          console.error(err);
          this.error = this.formatHttpError(err) || 'Error al agregar componente';
          this.success = '';
        }
      });
    }
  }

  cancelarFormulario() {
    this.limpiarFormulario();
  }

  limpiarFormulario() {
    this.mostrarFormulario = false;
    this.editando = false;
    this.componenteEditando = null;
    this.nuevoComponente = '';
    this.error = '';
  }

  editarComponente(componente: any) {
    this.componenteEditando = componente;
    this.nuevoComponente = componente.nombre;
    this.editando = true;
    this.mostrarFormulario = true;
    this.error = '';
    this.success = '';
  }

  eliminarComponente(id: string) {
    const componente = this.componentes.find(c => c.id === id);
    const nombreComponente = componente?.nombre || 'este componente';
    
    if (confirm(`¿Estás seguro de que quieres eliminar "${nombreComponente}"?\n\nEsto también eliminará:\n• Todas las relaciones asociadas\n• Las evaluaciones que usen este componente\n\nEsta acción no se puede deshacer.`)) {
      this.dataService.deleteComponente(id).subscribe({
        next: () => {
          this.refresh();
          this.success = `Componente "${nombreComponente}" eliminado correctamente`;
          this.error = '';
          setTimeout(() => this.success = '', 3000);
        },
        error: (err: unknown) => {
          console.error(err);
          this.error = this.formatHttpError(err) || 'Error al eliminar componente';
          this.success = '';
        }
      });
    }
  }

  toggleMenu(id: string, event: MouseEvent) {
    // Cerrar cualquier menú abierto
    const existingMenus = document.querySelectorAll('.dropdown-menu');
    existingMenus.forEach(menu => menu.remove());

    // Calcular posición del menú
    const button = event.currentTarget as HTMLElement;
    const rect = button.getBoundingClientRect();
    
    // Posicionar el menú debajo del botón
    const menu = document.createElement('div');
    menu.className = 'dropdown-menu';
    menu.style.position = 'fixed';
    menu.style.left = `${rect.left}px`;
    menu.style.top = `${rect.bottom + 5}px`;
    menu.style.zIndex = '9999';
    
    // Agregar contenido del menú
    menu.innerHTML = `
      <button class="dropdown-item" data-action="edit" data-id="${id}">
        <svg class="icon" viewBox="0 0 24 24" aria-hidden="true">
          <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
        </svg>
        Editar
      </button>
      <button class="dropdown-item dropdown-item--danger" data-action="delete" data-id="${id}">
        <svg class="icon" viewBox="0 0 24 24" aria-hidden="true">
          <path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/>
        </svg>
        Eliminar
      </button>
    `;
    
    // Agregar event listeners
    menu.addEventListener('click', (e) => {
      const target = e.target as HTMLElement;
      const action = target.closest('[data-action]')?.getAttribute('data-action');
      const componentId = target.closest('[data-action]')?.getAttribute('data-id');
      
      if (action === 'edit' && componentId) {
        const componente = this.componentes.find(c => c.id === componentId);
        if (componente) this.editarComponente(componente);
      } else if (action === 'delete' && componentId) {
        this.eliminarComponente(componentId);
      }
      
      document.body.removeChild(menu);
    });
    
    // Cerrar menú al hacer click fuera
    const closeMenu = () => {
      if (document.body.contains(menu)) {
        document.body.removeChild(menu);
      }
      document.removeEventListener('click', closeMenu);
    };
    
    setTimeout(() => {
      document.addEventListener('click', closeMenu);
    }, 100);
    
    // Agregar menú al body
    document.body.appendChild(menu);
  }

  limpiarMensajes() {
    this.error = '';
    this.success = '';
  }

  private formatHttpError(err: any): string {
    // Angular HttpErrorResponse common shapes
    if (!err) return '';
    const anyErr = err as any;
    if (typeof anyErr.error === 'string') return anyErr.error;
    if (anyErr.error?.message) return anyErr.error.message;
    if (anyErr.message) return anyErr.message;
    try {
      return JSON.stringify(anyErr.error ?? anyErr);
    } catch {
      return '';
    }
  }
}
