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

  //getByLoggedInUser(): any {
  //  var userEmail: string;
  //  var user: User;
  //  var users: User[];
  //  var allNotifications: UserNotification[];
  //  var userNotifications: UserNotification[];
  //  var userInfo: UserInfo;
  //  if (this.authService.isSignedIn()) {
  //    this.authService.getUserInfo().subscribe(
  //      (result) => {
  //        userInfo = result;
  //        userEmail = userInfo.email;
  //        console.log(result);
  //      },
  //      (error) => {
  //        console.error(error);
  //      }
  //    );
  //    this.userService.getUsers().subscribe(
  //      (result) => {
  //        users = result;
  //        users.forEach(u => { if(u.email.includes(userEmail)) { user = u; } });
  //      },
  //      (error) => { console.error(error); }
  //    );
  //    this.getAll().subscribe(
  //      (result) => {
  //        allNotifications = result;
  //        allNotifications.forEach(n => { if (n.user == user) { userNotifications.push(n); } });
  //        return userNotifications;
  //      },
  //      (error) => console.error(error)
  //    );
  //  }
  //}

  getByLoggedInUser(): any {
    var userEmail: string;
    var user: User;
    var users: UserInfo[];
    var allNotifications: UserNotification[];
    var userNotifications: UserNotification[];
    var userInfo: UserInfo;
    if (this.authService.isSignedIn()) {
      this.authService.getUserInfo().forEach(
        (result) => {
          users.push(result);
          console.log(result);
        },
        
      );
    }
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
