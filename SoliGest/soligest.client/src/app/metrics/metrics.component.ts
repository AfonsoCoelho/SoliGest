import { Component, OnInit } from '@angular/core';
import { MetricsService } from "../services/metrics.service";
import { ChartOptions, ChartType } from 'chart.js';

@Component({
  selector: 'app-metrics',
  standalone: false,
  templateUrl: './metrics.component.html',
  styleUrl: './metrics.component.css'
})

export class MetricsComponent implements OnInit {
  averageRepairTime: number = 0;
  totalPanels: number = 0;
  totalUsers: number = 0;
  totalAssistanceRequests: number = 0;
  requestsPerStatus: any;

  doughnutChartLabels: any[] = [];
  doughnutChartData: number[] = [];
  doughnutChartType: ChartType = 'doughnut';
  doughnutChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top',
      }
    }
  };
  doughnutChartColors = [
    {
      backgroundColor: ['#D61F1F', '#FFD301', '#006B3D']
    }
  ];
  pieChartLegend = true;


  constructor(private metricsService: MetricsService) { }

  ngOnInit(): void {
    this.loadMetrics();
  }

  loadMetrics(): void {

    this.metricsService.getAverageRepairTime().subscribe(
      (response) => {
        this.averageRepairTime = response.averageRepairTime;
      },
      (error) => {
        console.error('Error fetching average response time:', error);
      }
    );

    this.metricsService.getTotalPanels().subscribe(
      (response) => {
        this.totalPanels = response.totalPanels;
      },
      (error) => {
        console.error('Error fetching total panels:', error);
      }
    );

    this.metricsService.getTotalUsers().subscribe(
      (response) => {
        this.totalUsers = response.totalUsers;
      },
      (error) => {
        console.error('Error fetching total users:', error);
      }
    );

    this.metricsService.getAssistanceRequestPerStatus().subscribe(
      (response) => {
        this.requestsPerStatus = response;
        this.doughnutChartLabels = Object.keys(response);
        this.doughnutChartData = Object.values(response);
      },
      (error) => {
        console.error('Error fetching requests per status:', error);
      }
    );

  }
}
