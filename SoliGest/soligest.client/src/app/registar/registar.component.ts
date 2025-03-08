import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { RegisterDto } from '../models/register-dto';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})

export class RegistarComponent {
  registerData: RegisterDto = new RegisterDto("", "", "", "", "", new Date());
  addressLine1: string = "";
  addressLine2: string = "";

  constructor(private authService: AuthService) { }

  register() {
    this.registerData.address = this.addressLine1 + '\n' + this.addressLine2;

    this.authService.register(this.registerData).subscribe({
      next: () => console.log('User registered successfully'),
      error: (err) => console.error('Registration failed', err)
    });
  }
}
