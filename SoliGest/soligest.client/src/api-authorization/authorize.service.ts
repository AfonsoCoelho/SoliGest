import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, catchError, map, of } from 'rxjs';
import { UserInfo } from './authorize.dto';
import { ActivatedRoute } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {  

  private _authStateChanged: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  public onStateChanged() {
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
          return true;
        }
        return false;
      }),
      catchError(() => {
        return of(false);
      })
    );
  }

  // Registro de novo utilizador
  public register(name: string, address1: string, address2: string, phoneNumber: string, birthDate: Date, email: string, password: string, role: string, dayOff: string, startHoliday: string, endHoliday: string): Observable<boolean> {
    return this.http.post('api/Users/signup', { name, address1, address2, phoneNumber, birthDate, email, password, role, dayOff, startHoliday, endHoliday }, { observe: 'response' }).pipe(
      map((res) => res.ok),
      catchError(() => of(false))
    );
  }

  // Logout - Remove o token e notifica o estado
  public signOut(): void {
    this.clearToken();
    this._authStateChanged.next(false);
  }

  // Verifica se o utilizador está autenticado
  public isSignedIn(): boolean {
    return this.hasToken();
  }

  
  // check if the user is authenticated. the endpoint is protected so 401 if not.
  public user() {
    return this.http.get<UserInfo>('/manage/info', {
      withCredentials: true
    }).pipe(
      catchError((_: HttpErrorResponse, __: Observable<UserInfo>) => {
        return of({} as UserInfo);
      }));
  }

  // Obter informações do utilizador autenticado
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

  //public getToken(): undefined {
  //  if (this.hasToken()) {
  //    return localStorage.getItem('authToken') || 'a';
  //  }
    
  //}
}
