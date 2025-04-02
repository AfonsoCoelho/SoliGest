import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SolarPanelsService {
  constructor(private http: HttpClient) { }

  getSolarPanels(): Observable<SolarPanel[]> {
    return this.http.get<SolarPanel[]>('api/SolarPanels');
  }

  getSolarPanel(id: number): Observable<SolarPanel> {
    return this.http.get<SolarPanel>('api/SolarPanels/' + id);
  }

  createSolarPanel(solarPanel: SolarPanel): Observable<SolarPanel> {
    return this.http.post<SolarPanel>('api/SolarPanels', solarPanel);
  }

  updateSolarPanel(id: number, name: string, priority: string | undefined, status: string, statusClass: string, latitude: number | undefined, longitude: number | undefined, description: string | undefined, phone: number | undefined, email: string | undefined, address: string): Observable<SolarPanel> {
    return this.http.put<SolarPanel>('api/SolarPanels/' + id, { id, name, priority, status, statusClass, latitude, longitude, description, phone, email, address });
  }

  deleteSolarPanel(id: number): Observable<SolarPanel> {
    return this.http.delete<SolarPanel>('api/SolarPanels/' + id);
  }
}

export interface SolarPanel {
  id: number;
  name: string;
  priority?: string;
  status: string;
  statusClass: string;
  latitude?: number;
  longitude?: number;
  description?: string;
  phone?: number;
  email?: string;
  address?: string;
}
