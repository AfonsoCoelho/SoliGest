import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SolarPanel } from './solar-panels.service';

@Injectable({
  providedIn: 'root'
})
export class AssistanceRequestsService {
  

  constructor(private http: HttpClient) { }

  getAll(): Observable<AssistanceRequest[]> {
    return this.http.get<AssistanceRequest[]>('api/AssistanceRequests');
  }

  getById(id: number): Observable<AssistanceRequest> {
    return this.http.get<AssistanceRequest>('api/AssistanceRequests/' + id);
  }

  create(request: AssistanceRequestCreateModel): Observable<AssistanceRequest> {
    return this.http.post<AssistanceRequest>('api/AssistanceRequests/', request);
  }

  update(id: number, request: AssistanceRequest): Observable<AssistanceRequest> {
    return this.http.put<AssistanceRequest>('api/AssistanceRequests/' + id, request);
  }

  delete(id: number): Observable<AssistanceRequest> {
    return this.http.delete<AssistanceRequest>('api/AssistanceRequests/' + id);
  }
}

export interface AssistanceRequest {
  id: number;
  requestDate: string;
  priority?: string;
  status: string;
  statusClass: string;
  resolutionDate: string;
  description: string;
  solarPanel: SolarPanel
}

export interface AssistanceRequestCreateModel {
  requestDate: string;
  resolutionDate: string;
  description: string;
  solarPanelId: number;
  priority: string;
  status: string;
  statusClass: string;
}
