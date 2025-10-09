import { Injectable } from '@angular/core';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { Evaluacion } from './models';

@Injectable({
  providedIn: 'root'
})
export class ExportService {

  constructor() { }

  // Exportar a PDF con formato de mini carta Gantt mejorado
  exportToPDF(evaluacion: Evaluacion): void {
    const doc = new jsPDF();
    
    // ===== ENCABEZADO PRINCIPAL =====
    // Fondo azul para el encabezado
    doc.setFillColor(41, 128, 185);
    doc.rect(0, 0, 210, 40, 'F');
    
    // Título principal en blanco
    doc.setFontSize(24);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(255, 255, 255);
    doc.text('EVALUACIÓN DE PROYECTO', 105, 25, { align: 'center' });
    
    // ===== INFORMACIÓN DEL PROYECTO =====
    doc.setTextColor(0, 0, 0);
    doc.setFontSize(16);
    doc.setFont('helvetica', 'bold');
    doc.text('INFORMACIÓN GENERAL', 20, 60);
    
    // Línea separadora
    doc.setDrawColor(41, 128, 185);
    doc.setLineWidth(0.5);
    doc.line(20, 65, 190, 65);
    
    // Datos del proyecto en formato tabla visual
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.text('Proyecto:', 25, 80);
    doc.setFont('helvetica', 'normal');
    doc.text(evaluacion.nombreProyecto, 80, 80);
    
    doc.setFont('helvetica', 'bold');
    doc.text('Fecha:', 25, 90);
    doc.setFont('helvetica', 'normal');
    doc.text(new Date(evaluacion.fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    }), 80, 90);
    
    // ===== MÉTRICAS DEL PROYECTO =====
    doc.setFontSize(16);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text('MÉTRICAS DEL PROYECTO', 20, 115);

    // Separador
    doc.setDrawColor(41, 128, 185);
    doc.setLineWidth(0.5);
    doc.line(20, 120, 190, 120);

    // ---- Cálculos ----
    const horasBase   = evaluacion.horasTotales ?? 0;
    const riesgoPct   = Number(evaluacion.deltaRiesgoPct) || 0;
    
    // Calcular horas con riesgo de manera consistente
    let horasRiesgo = horasBase;
    if (riesgoPct > 0) {
      horasRiesgo = Math.round(horasBase * (1 + riesgoPct / 100));
    }
    
    // Calcular días estimados (6 horas = 1 día laboral)
    const diasEstimados = Math.ceil(horasRiesgo / 6);

    // ---- Helper para dibujar tiles ----
    const drawTile = (
      x: number, y: number, w: number, h: number,
      fill: [number, number, number],
      title: string, value: string
    ) => {
      doc.setFillColor(...fill);
      doc.rect(x, y, w, h, 'F');
      doc.setTextColor(255, 255, 255);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(10);
      doc.text(title, x + w / 2, y + 10, { align: 'center' });
      doc.setFontSize(16);
      doc.text(value, x + w / 2, y + 20, { align: 'center' });
      doc.setTextColor(0, 0, 0); // reset
    };

    // ---- Layout centrado (4 tiles) ----
    const PAGE_W = 210;
    const TILE_W = 48;       // ancho de cada tile
    const TILE_H = 28;       // alto
    const GAP    = 3;        // espacio entre tiles (reducido de 14 a 8)
    const GROUP_W = 4 * TILE_W + 3 * GAP;
    const START_X = (PAGE_W - GROUP_W) / 2; // centrado en la página
    const BASE_Y  = 130;

    // 1) TOTAL DE HORAS (azul)
    drawTile(START_X + 0 * (TILE_W + GAP), BASE_Y, TILE_W, TILE_H, [52, 152, 219], 'TOTAL DE HORAS', `${horasBase} hrs`);

    // 2) CON RIESGO (amarillo)
    drawTile(START_X + 1 * (TILE_W + GAP), BASE_Y, TILE_W, TILE_H, [241, 196, 15], `CON RIESGO (${riesgoPct}%)`, `${horasRiesgo} hrs`);

    // 3) DÍAS ESTIMADOS (verde)
    drawTile(START_X + 2 * (TILE_W + GAP), BASE_Y, TILE_W, TILE_H, [46, 204, 113], 'DÍAS ESTIMADOS', `${diasEstimados} días`);

    // 4) RIESGO (naranja)
    drawTile(START_X + 3 * (TILE_W + GAP), BASE_Y, TILE_W, TILE_H, [230, 126, 34], 'RIESGO', `${riesgoPct}%`);

    // Próxima sección (debajo de los tiles)
    const nextSectionY = BASE_Y + TILE_H + 35;
    
    // ===== PLANIFICACIÓN DE TAREAS =====
    const startY = nextSectionY; // <- usar el Y calculado dinámicamente según las cajas pintadas
    doc.setFontSize(16);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(0, 0, 0);
    doc.text('PLANIFICACIÓN DE TAREAS', 20, startY);

    // Separador
    doc.setDrawColor(41, 128, 185);
    doc.setLineWidth(0.5);
    doc.line(20, startY + 5, 190, startY + 5);
    
    // === Tabla mejorada con riesgo distribuido y acumulado de horas ===
    const horasBaseTotal = evaluacion.horasTotales ?? 0;
    const factor = 1 + riesgoPct / 100;

    // acumulado de horas (con riesgo) para posicionar los días sin redondeos por tarea
    let horasAcum = 0;

    const tableData = evaluacion.detalles.map((detalle, index) => {
      const horasBase = Number(detalle.horasBase) || 0;

      // Calcular horas con riesgo para esta tarea
      let horasTareaRiesgo = horasBase;
      if (riesgoPct > 0) {
        horasTareaRiesgo = Math.round(horasBase * (1 + riesgoPct / 100));
      }

      // Calcular días de la tarea individual
      const diasTarea = Math.ceil(horasTareaRiesgo / 6);

      // días de inicio: antes de sumar la tarea
      const diaInicio = Math.floor(horasAcum / 6) + 1;

      // acumulo la tarea
      horasAcum += horasTareaRiesgo;

      // días de término: después de sumar la tarea
      const diaFin = Math.ceil(horasAcum / 6);

      return [
        `${index + 1}`,
        detalle.descripcionTarea,
        `${horasBase} hrs`,                    // horas base
        `${diasTarea} días`,                   // días de la tarea individual
        `Día ${diaInicio} - ${diaFin}`        // período acumulado
      ];
    });

    // Al final, el último diaFin será exactamente ceil(totalHorasConRiesgo/6)
    
    autoTable(doc, {
      head: [['#', 'Tarea', 'Horas', 'Días', 'Período de Ejecución']],
      body: tableData,
      startY: startY + 15,
      styles: {
        fontSize: 10,
        cellPadding: 4,
        lineColor: [41, 128, 185],
        lineWidth: 0.1
      },
      headStyles: {
        fillColor: [41, 128, 185],
        textColor: 255,
        fontStyle: 'bold',
        fontSize: 11
      },
      alternateRowStyles: {
        fillColor: [248, 249, 250]
      },
      columnStyles: {
        0: { cellWidth: 15, halign: 'center' },
        1: { cellWidth: 70 },
        2: { cellWidth: 25, halign: 'center' },
        3: { cellWidth: 25, halign: 'center' },
        4: { cellWidth: 35, halign: 'center' }
      }
    });
    
    // ===== PIE DE PÁGINA =====
    // Calcular la posición del pie de página asegurando que esté en la página
    const tableEndY = (doc as any).lastAutoTable.finalY + 20;
    
    // Si el contenido es muy largo, agregar una nueva página para el pie de página
    let footerY: number;
    if (tableEndY > 250) {
      doc.addPage();
      footerY = 30; // Posición fija en la nueva página
    } else {
      footerY = Math.min(tableEndY + 20, 270); // Máximo en Y=270 para asegurar que esté en la página
    }
    
    // Línea separadora del pie de página
    doc.setDrawColor(200, 200, 200);
    doc.line(20, footerY, 190, footerY);
    
    // Línea separadora del pie de página
    doc.setDrawColor(200, 200, 200);
    doc.line(20, footerY, 190, footerY);
    
    // Texto del pie de página
    doc.setFontSize(8);
    doc.setFont('helvetica', 'italic');
    doc.setTextColor(100, 100, 100);
    doc.text('Documento generado automáticamente por el Sistema de Evaluación de Proyectos', 105, footerY + 10, { align: 'center' });
    doc.text(`Generado el: ${new Date().toLocaleString('es-ES')}`, 105, footerY + 20, { align: 'center' });
    
         // Logo de Sura Seguros centrado debajo del texto
     try {
       // Usar la ruta correcta del logo desde assets
       const logoUrl = 'assets/sura_seguros_logotipo_unatinta_negro_positivo.png';
       doc.addImage(logoUrl, 'PNG', 85, footerY + 30, 40, 20, undefined, 'FAST');
     } catch (error) {
       console.warn('No se pudo cargar el logo de Sura Seguros:', error);
     }
    
    // Guardar el PDF
    const fileName = `Evaluacion_${evaluacion.nombreProyecto.replace(/[^a-zA-Z0-9]/g, '_')}_${new Date().toISOString().split('T')[0]}.pdf`;
    doc.save(fileName);
  }
}
