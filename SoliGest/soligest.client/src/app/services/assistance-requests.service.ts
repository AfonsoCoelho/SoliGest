import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AssistanceRequest {
  id: number;
  requestDate: string;
  resolutionDate: string;
  description: string;
  solarPanel: string;
}


@Injectable({
  providedIn: 'root'
})
export class AssistanceRequestsService {
  

  constructor(private http: HttpClient) { }

  getAll(): Observable<AssistanceRequest[]> {
    return this.http.get<AssistanceRequest[]>('api/AssistanceRequest');
  }

  getById(id: number): Observable<AssistanceRequest> {
    return this.http.get<AssistanceRequest>('api/AssistanceRequest/' + id);
  }

  create(request: AssistanceRequest): Observable<AssistanceRequest> {
    return this.http.post<AssistanceRequest>('api/AssistanceRequest/', request);
  }

  update(id: number, request: AssistanceRequest): Observable<void> {
    return this.http.put<void>('api/AssistanceRequest/' + id, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>('api/AssistanceRequest/' + id);
  }
}
