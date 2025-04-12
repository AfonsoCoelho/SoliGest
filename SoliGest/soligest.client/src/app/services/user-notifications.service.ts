import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from './users.service';
import { Notification } from './notifications.service'

@Injectable({
  providedIn: 'root'
})
export class UserNotificationsService {

  constructor(private http: HttpClient) { }

  getAll(): Observable<UserNotification[]> {
    return this.http.get<UserNotification[]>('api/UserNotifications');
  }

  getById(id: number): Observable<UserNotification> {
    return this.http.get<UserNotification>('api/UserNotifications/' + id);
  }

  create(request: UserNotification): Observable<UserNotification> {
    return this.http.post<UserNotification>('api/UserNotifications/', request);
  }

  update(id: number, request: UserNotificationUpdateModel): Observable<UserNotification> {
    return this.http.put<UserNotification>('api/UserNotifications/' + id, request);
  }

  delete(id: number): Observable<UserNotification> {
    return this.http.delete<UserNotification>('api/UserNotifications/' + id);
  }
}

export interface UserNotification {
  id: number;
  user: User;
  userId: number;
  notification: Notification;
  notificationId: number;
  receivedDate: Date;
  isRead: boolean;
}

export interface UserNotificationUpdateModel {
  id: number;
  userId: number;
  notificationId: number;
  receivedDate: Date;
  isRead: boolean
}
