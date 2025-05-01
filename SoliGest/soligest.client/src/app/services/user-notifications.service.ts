import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User, UsersService } from './users.service';
import { Notification } from './notifications.service'
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { UserInfo } from '../../api-authorization/authorize.dto';
import { not } from 'rxjs/internal/util/not';

@Injectable({
  providedIn: 'root'
})
export class UserNotificationsService {

  constructor(private http: HttpClient, private authService: AuthorizeService, private userService: UsersService) { }

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

  getByLoggedInUser(): any {
    var loggedUserId = localStorage.getItem('loggedUserId');
    if (loggedUserId) {
      return this.http.get<UserNotification[]>('api/UserNotifications/ByUserId/' + loggedUserId);
    }
    else {
      return null;
    }
  }
}

export interface UserNotification {
  userNotificationId: number;
  user: User;
  userId: number;
  notification: Notification;
  notificationId: number;
  receivedDate: Date;
  isRead: boolean;
}

export interface UserNotificationUpdateModel {
  userNotificationId: number;
  userId: number;
  notificationId: number;
  receivedDate: Date;
  isRead: boolean
}
