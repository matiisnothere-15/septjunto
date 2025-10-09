import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DataService } from '../../../shared/data.service';
import { Proyecto } from '../../../shared/models';

@Component({
  selector: 'app-evaluaciones-listar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './listar.component.html',
  styleUrls: ['./listar.component.scss']
})
export class ListarComponent {
  items: Proyecto[] = [];
  constructor(private ds: DataService) { this.items = this.ds.getProyectos(); }
}
