import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard.component';
import { PanelModule } from 'primeng/panel';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { PdfViewerModule } from 'ng2-pdf-viewer';

@NgModule({
  declarations: [DashboardComponent],
  imports: [
    CommonModule,
    PanelModule,
    ScrollPanelModule,
    PdfViewerModule
  ]
})
export class DashboardModule { }
