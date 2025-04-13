import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MetricsService {

  constructor(private http: HttpClient) { }

  getTotalUsers(): Observable<any> {
    return this.http.get<any>(`api/MetricsController/total-usuarios`);
  }

  getTotalPanels(): Observable<any> {
    return this.http.get<any>(`api/MetricsController/total-paineis`);
  }

  getAssistanceRequestPerStatus(): Observable<any> {
    return this.http.get<any>(`api/MetricsController/avarias-status`);
  }

  getAverageRepairTime(): Observable<any> {
    return this.http.get<any>(`api/MetricsController/tempo-medio-reparacao`);
  }
}
