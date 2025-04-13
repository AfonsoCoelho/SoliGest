import { Component, OnInit, ViewChild } from '@angular/core';
import { MetricsService } from "../services/metrics.service";
import {
  ApexNonAxisChartSeries,
  ApexResponsive,
  ApexChart,
  ChartComponent,
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
  title: any;
};

@Component({
  selector: 'app-metrics',
  templateUrl: './metrics.component.html',
  styleUrl: './metrics.component.css',
  standalone: false
})
export class MetricsComponent implements OnInit {
  @ViewChild("chart") chart!: ChartComponent;

  public chartOptions: ChartOptions = {
    title: {
      text: ""
    },
      series: [],
      chart: {
          height: 350,
          type: "pie",
      },
      responsive: [
          {
              breakpoint: 480,
              options: {
                  chart: {
                      width: 200,
                  },
                  legend: {
                      position: "bottom",
                  },
              },
          },
      ],
      labels: [],
  };

  averageRepairTime: number = 0;
  totalPanels: number = 0;
  totalUsers: number = 0;
  totalAssistanceRequests: number = 0;
  requestsPerPriority: any;

  constructor(private metricsService: MetricsService) { }

  ngOnInit(): void {
    this.loadMetrics();
  }

  loadMetrics(): void {
    this.metricsService.getAverageRepairTime().subscribe({
      next: (response) => {
        this.averageRepairTime = response.averageRepairTime;
      },
      error: (error) => {
        console.error('Error fetching average response time:', error);
      }
    });

    this.metricsService.getTotalPanels().subscribe({
      next: (response) => {
        this.totalPanels = response.totalPanels;
      },
      error: (error) => {
        console.error('Error fetching total panels:', error);
      }
    });

    this.metricsService.getTotalUsers().subscribe({
      next: (response) => {
        this.totalUsers = response.totalUsers;
      },
      error: (error) => {
        console.error('Error fetching total users:', error);
      }
    });

    this.metricsService.getTotalAssistanceRequests().subscribe({
      next: (response) => {
        this.totalAssistanceRequests = response.totalAssistanceRequests;
      },
      error: (error) => {
        console.error('Error fetching total requests', error);
      }
    });

    this.metricsService.getAssistanceRequestPerPriority().subscribe({
      next: (response) => {
        this.requestsPerPriority = response;
        this.buildChart();
      },
      error: (error) => {
        console.error('Error fetching requests per priority:', error);
      }
    });
  }

  buildChart() {
    if (this.requestsPerPriority) {
      this.chartOptions = {
        title: {
          text: "Número de Pedidos de Assistência por Prioridade"
        },
        series: Object.values(this.requestsPerPriority),
        chart: {
          height: 350,
          type: "pie",
        },
        labels: Object.keys(this.requestsPerPriority),
        responsive: [
          {
            breakpoint: 480,
            options: {
              chart: {
                width: 200,
              },
              legend: {
                position: "bottom",
              },
            },
          },
        ],
      };
    }
  }
}
