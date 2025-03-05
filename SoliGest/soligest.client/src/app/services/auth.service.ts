import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterDto } from '../models/register-dto';
import { LoginDto } from '../models/login-dto';
import { AuthResponseDto } from '../models/auth-response-dto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:5001/api/account';

  constructor(private http: HttpClient) { }

  register(user: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  login(credentials: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, credentials);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }
}
