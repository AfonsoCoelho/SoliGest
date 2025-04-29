import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { BehaviorSubject, Observable, Subject, catchError, map, of } from 'rxjs';
import { UserInfo } from './authorize.dto';
import { ActivatedRoute } from '@angular/router';
import { User, UsersService } from '../app/services/users.service';


@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {  

  private _authStateChanged: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasToken());
  private loggedUserId: string | null;
  private loggedUserEmail: string;
  private loggedUser: any;
  private userLatitude: number;
  private userLongitude: number;

  constructor(private http: HttpClient, private route: ActivatedRoute, private us: UsersService, private injector: Injector) {
    this.loggedUserEmail = "";
    this.loggedUserId = localStorage.getItem('loggedUserId');;
    this.loggedUser = 0;
    this.userLatitude = 0;
    this.userLongitude = 0;
  }


  public onStateChanged() {
    console.log("token " + localStorage.getItem('authToken') + "\n\n\n user:\n" + this.user());
    return this._authStateChanged.asObservable();
  }

  // Verifica se o token está presente no localStorage
  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  // Armazena o token no localStorage
  private saveToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  // Remove o token do localStorage
  private clearToken(): void {
    localStorage.removeItem('authToken');
  }

  // Login baseado em JWT
  public signIn(email: string, password: string): Observable<boolean> {
    return this.http.post<{ token: string }>('/api/Users/signin', { email, password }).pipe(
      map((response) => {
        if (response && response.token) {
          this.saveToken(response.token);
          this._authStateChanged.next(true);

          this.loggedUserEmail = email;
          this.us.getUserByEmail(this.loggedUserEmail).subscribe(
            (result) => {
              this.loggedUser = result;
              localStorage.setItem('loggedUserId', this.loggedUser.id);
              this.loggedUserId = this.loggedUser.id;
              this.us.setUserAsActive(this.loggedUser.id).subscribe(
                (result) => {
                  this.loggedUser = result;
                  if (this.loggedUser.role == "Técnico") {
                    this.getUserLocation();
                  }
                },
                (error) => console.error(error)
              );
            }
          )
          return true;
        }
        return false;
      }),
      catchError(() => {
        return of(false);
      })
    );
  }

  public register(name: string, address1: string, address2: string, phoneNumber: string, birthDate: Date, email: string, password: string, role: string, dayOff: string, startHoliday: string, endHoliday: string): Observable<boolean> {
    return this.http.post('api/Users/signup', { name, address1, address2, phoneNumber, birthDate, email, password, role, dayOff, startHoliday, endHoliday }, { observe: 'response' }).pipe(
      map((res) => res.ok),
      catchError(() => of(false))
    );
  }

  public getUserLocation() {
    var userId = this.loggedUser.id;
    if ("geolocation" in navigator) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const { latitude, longitude } = position.coords;
          this.userLatitude = latitude;
          this.userLongitude = longitude;
          this.us.updateUserLocation(userId, this.userLatitude, this.userLongitude).subscribe(
            (result) => {
              this.loggedUser = result;
            },
            (error) => console.error(error)
          );
        },
        (error) => {
          console.error(error);
        }
      );
    } else {
      console.error("Ocorreu um erro");
    }
  }

  public signOut(): void {
    this.clearToken();
    this._authStateChanged.next(false);
    if (this.loggedUserId) {
      this.us.setUserAsInactive(this.loggedUserId).subscribe(
        (result) => {
          this.loggedUser = result;
        },
        (error) => console.error(error)
      );
      this.us.updateUserLocation(this.loggedUserId, 0, 0).subscribe(
        (result) => {
          this.loggedUser = result;
        },
        (error) => console.error(error)
      );
      this.loggedUserEmail = "";
      this.loggedUser = null;
      localStorage.removeItem('loggedUserId');
    }
  }

  public isSignedIn(): boolean {
    return this.hasToken();
  }

  public user() {
    return this.http.get<UserInfo>('/manage/info', {
      withCredentials: true
    }).pipe(
      catchError((_: HttpErrorResponse, __: Observable<UserInfo>) => {
        console.log(of({} as UserInfo));
        return of({} as UserInfo);
      }));
  }

  public getUserInfo(): Observable<UserInfo> {
    const token = localStorage.getItem('authToken');
    if (!token) {
      return of({} as UserInfo);
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<UserInfo>('/api/userinfo', { headers }).pipe(
      catchError(() => of({} as UserInfo))
    );
  }

  public resetPassword(newPassword: string): Observable<boolean> {
    var email, token;

    this.route.queryParams.subscribe(params => {
      email = params['email'];
      token = params['token'];
    });

    return this.http.post('/api/Users/reset-password', { email, token, newPassword }, { observe: 'response' }).pipe(
      map((res) => res.ok),
      catchError(() => of(false))
    );
  }
  
  public pwRecovery(email: string): Observable<boolean> {
    return this.http.post('api/Users/forgot-password', { email }, { observe: 'response' }).pipe(
      map((res) => res.ok),
      catchError(() => of(false))
    );
  }

  public getToken(): string {
    if (this.hasToken()) {
      return localStorage.getItem('authToken') || 'a';
    }
      return "Not Available";
  }
    
  public getLoggedUserEmail(): any {
    if (this.isSignedIn()) {
      return this.loggedUserEmail;
    }
    else {
      return false;
    }
    return this.loggedUserEmail;
  }

  //public getLoggedUser(): Observable<User> {
  //  if (this.isSignedIn()) {
  //    return this.loggedUserEmail;
  //  }
  //}
}
