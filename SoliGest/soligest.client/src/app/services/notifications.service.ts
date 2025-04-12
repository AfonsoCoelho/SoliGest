import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  constructor(private http: HttpClient) { }

  getAll(): Observable<Notification[]> {
    return this.http.get<Notification[]>('api/Notifications');
  }

  getById(id: number): Observable<Notification> {
    return this.http.get<Notification>('api/Notifications/' + id);
  }

  create(request: Notification): Observable<Notification> {
    return this.http.post<Notification>('api/Notifications/', request);
  }

  update(id: number, request: Notification): Observable<Notification> {
    return this.http.put<Notification>('api/Notifications/' + id, request);
  }

  delete(id: number): Observable<Notification> {
    return this.http.delete<Notification>('api/Notifications/' + id);
  }
}

export interface Notification {
  id: number;
  type: string;
  message: string;
}
