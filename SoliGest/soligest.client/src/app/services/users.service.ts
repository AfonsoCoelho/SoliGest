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

  updateUser(id: string, name: string, address1: string, address2: string, phoneNumber: string, birthDate: Date, email: string): Observable<User> {
    return this.http.put<User>('api/Users/' + id, { name, address1, address2, phoneNumber, birthDate, email });
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
  birthDate: string;
  //role: string;
  email: string;
  phoneNumber: number;
  //folgasMes: Date[];
  //feriasAno: { inicio: Date, fim: Date }[];
}

//{
//  "name": "SoliGest Admin",
//    "birthDate": "0001-01-01",
//      "monthlyDaysOff": [],
//        "yearHolidays": [],
//          "id": "82f936ee-f5c3-41ee-b6fb-0e0c250bfa05",
//            "userName": "soligestesa@gmail.com",
//              "normalizedUserName": "SOLIGESTESA@GMAIL.COM",
//                "email": "soligestesa@gmail.com",
//                  "normalizedEmail": "SOLIGESTESA@GMAIL.COM",
//                    "emailConfirmed": false,
//                      "passwordHash": "AQAAAAIAAYagAAAAEKx9VU8F0/GwT9+vZBzkLHnRdHkwH8jZb3btXhynXxUHrsaQV5lF2mCoCsa2CrMkIw==",
//                        "securityStamp": "FU7SHNBABIJYWWOSM255NPG5WILDDPGK",
//                          "concurrencyStamp": "7d0dba15-a9c6-4423-b394-a408a0e44d69",
//                            "phoneNumber": null,
//                              "phoneNumberConfirmed": false,
//                                "twoFactorEnabled": false,
//                                  "lockoutEnd": null,
//                                    "lockoutEnabled": true,
//                                      "accessFailedCount": 0
//}
