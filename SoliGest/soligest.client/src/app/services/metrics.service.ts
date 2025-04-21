import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MetricsService {

  constructor(private http: HttpClient) { }

  getTotalUsers(): Observable<any> {
    return this.http.get<any>(`api/Metrics/total-usuarios`);
  }

  getTotalPanels(): Observable<any> {
    return this.http.get<any>(`api/Metrics/total-paineis`);
  }

  getTotalAssistanceRequests(): Observable<any> {
    return this.http.get<any>(`api/Metrics/total-pedidos-assistencia`);
  }

  getAssistanceRequestPerPriority(): Observable<any> {
    return this.http.get<any>(`api/Metrics/avarias-priority`);
  }

  getAverageRepairTime(): Observable<any> {
    return this.http.get<any>(`api/Metrics/tempo-medio-reparacao`);
  }
}
