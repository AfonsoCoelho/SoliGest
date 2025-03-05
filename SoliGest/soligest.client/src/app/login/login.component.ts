import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { LoginDto } from '../models/login-dto';


@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginData: LoginDto = new LoginDto('', '');

  constructor(private authService: AuthService) { }

  login() {
    this.authService.login(this.loginData).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.token);
        console.log('Login successful', response);
      },
      error: (err) => console.error('Login failed', err)
    });
  }
}
