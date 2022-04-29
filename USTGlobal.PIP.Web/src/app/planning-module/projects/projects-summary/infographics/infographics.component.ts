import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { Label, MultiDataSet, Color } from 'ng2-charts';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-infographics',
  templateUrl: './infographics.component.html'
})
export class InfographicsComponent implements OnInit {
  Infoghraphics: any;
  Revenue: any;
  Ebitda: any;
  Location: any;
  public barChartOptions: ChartOptions = {
    responsive: true,
  };
  public barChartLabels: Label[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'Jul', 'Sep', 'Oct', 'Nov', 'Dec'];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartPlugins = [];

  public barChartData: ChartDataSets[] = [
    { data: [65, 59, 80, 81, 56, 55, 40, -20, -48, -40, -19, -86, -27, -90], label: 'Series A' },
    { data: [28, 48, 40, 19, 86, 27, 90, -65, -59, -80, -81, -56, -55, -40], label: 'Series B' }
  ];
  // Doughnut
  public doughnutChartLabels: Label[] = ['Sl 1', 'Sl 2', 'Sl 3'];
  public doughnutChartData: MultiDataSet = [
    [350, 450, 100],
  ];
  public doughnutChartType: ChartType = 'doughnut';
  public scatterChartOptions: ChartOptions = {
    responsive: true,
  };

  public scatterChartData: ChartDataSets[] = [
    {
      data: [
        { x: 1, y: 1 },
        { x: 2, y: 3 },
        { x: 3, y: -2 },
        { x: 4, y: 4 },
        { x: 5, y: -3, r: 20 },
      ],
      label: 'Series A',
      pointRadius: 10,
    },
  ];
  public scatterChartType: ChartType = 'scatter';
  public lineChartData: ChartDataSets[] = [
    { data: [65, 59, 80, 81, 56, 55, 40], label: 'Series A' },
  ];
  public lineChartLabels: Label[] = ['January', 'February', 'March', 'April', 'May', 'June', 'July'];

  public lineChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.3)',
    },
  ];
  public lineChartType = 'line';
  public lineChartPlugins = [];
  constructor(
    private translateService: TranslateService,
  ) {}

  ngOnInit() {
    this.translateService.get('Infographics.Ebitda').subscribe(ebitda => {
      this.Ebitda = ebitda;
    });
    this.translateService.get('Infographics.Revenue').subscribe(revenue => {
      this.Revenue = revenue;
    });
    this.translateService.get('Infographics.Location').subscribe(location => {
      this.Location = location;
    });
  }

}
