import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('api/Users');
  }

  getUser(id: string): Observable<User> {
    return this.http.get<User>('api/Users/by-id/' + id);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>('api/Users', user);
  }

  updateUser(id: string, name: string, address1: string, address2: string, phoneNumber: string, birthDate: Date, email: string, role: string, dayOff: string, startHoliday: string, endHoliday: string): Observable<User> {
    console.log(email);
    return this.http.put<User>('api/Users/' + id, { id, name, email, birthDate, address1, address2, phoneNumber, role, dayOff, startHoliday, endHoliday });
  }

  deleteUser(id: string): Observable<User> {
    return this.http.delete<User>('api/Users/' + id);
  }

  saveDaysOff(userId: string, dates: Date[]): Observable<any> {
    return this.http.post(`api/DaysOff/${userId}`, dates);
  }

  saveHolidays(userId: string, holidays: { inicio: Date; fim: Date }[]): Observable<any> {
    return this.http.post(`api/Holidays/${userId}`, holidays);
  }

  getUserByEmail(email: string): Observable<User> {
    return this.http.get<User>('api/Users/by-email/' + email);
  }

  setUserAsActive(userId: string): Observable<User> {
    return this.http.put<User>('api/Users/set-user-as-active/' + userId, userId);
  }

  setUserAsInactive(userId: string): Observable<User> {
    return this.http.put<User>('api/Users/set-user-as-inactive/' + userId, userId);
  }

  updateUserLocation(userId: string, latitude: number, longitude: number): Observable<User> {
    const params = new HttpParams()
      .set('latitude', latitude)
      .set('longitude', longitude);
    return this.http.put<User>('api/Users/update-location/' + userId + '/?latitude=' + latitude + '&longitude=' + longitude, userId);
  }

  saveProfilePicture(userId: string, file: File): Observable<Object> {
    const formData = new FormData();
    formData.append('file', file);

    const url = `api/Users/save-profile-picture/${userId}`;

    return this.http.put(url, formData, {
      responseType: 'text' as 'json'
    });
  }

}

export interface User {
  id: string;
  profilePictureUrl: string;
  name: string;
  address1: string;
  address2: string;
  birthDate: string;
  role: string;
  email: string;
  phoneNumber: number;
  dayOff: string;
  startHoliday: string;
  endHoliday: string;
  latitude: number;
  longitude: number;
  isActive: boolean;
}

