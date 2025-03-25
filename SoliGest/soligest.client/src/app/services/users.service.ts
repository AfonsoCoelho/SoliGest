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

  getUser(id: number): Observable<User> {
    return this.http.get<User>('api/Users/' + id);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>('api/Users', user);
  }

  updateUser(user: User): Observable<User> {
    return this.http.put<User>('api/Users/' + user.id, user);
  }

  deleteUser(id: number): Observable<User> {
    return this.http.delete<User>('api/Users/' + id);
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
