import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { RegisterDto } from '../models/register-dto';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})

export class RegistarComponent {
  registerData: RegisterDto = new RegisterDto("","","","","");

  constructor(private authService: AuthService) { }

  register() {
    this.authService.register(this.registerData).subscribe({
      next: () => console.log('User registered successfully'),
      error: (err) => console.error('Registration failed', err)
    });
  }
}
