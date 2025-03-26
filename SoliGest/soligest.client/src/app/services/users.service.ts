import { HttpClient } from '@angular/common/http';
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
    return this.http.get<User>('api/Users/' + id);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>('api/Users', user);
  }

  updateUser(user: User): Observable<User> {
    return this.http.put<User>('api/Users/' + user.id, user);
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
}

export interface User {
  id: string;
  name: string;
  address1: string;
  address2: string;
  birthDate: undefined;
  role: string;
  email: string;
  phoneNumber: number;
  folgasMes: Date[];
  feriasAno: { inicio: Date, fim: Date }[];
}
