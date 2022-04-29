import { Component, OnInit } from '@angular/core';
import { Label, MultiDataSet } from 'ng2-charts';
import { ChartType } from 'chart.js';

@Component({
  selector: 'app-resource-analysis',
  templateUrl: './resource-analysis.component.html',
  styleUrls: ['./resource-analysis.component.scss']
})
export class ResourceAnalysisComponent implements OnInit {
  // Doughnut
  public doughnutChartLabels: Label[] = ['A Band' ];
  public doughnutChartData: MultiDataSet = [
    [350, 450, 100, 250],
  ];
  public doughnutChartType: ChartType = 'doughnut';
  constructor() { }

  ngOnInit() {
  }

}
