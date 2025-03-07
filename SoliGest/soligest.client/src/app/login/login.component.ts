import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { LoginDto } from '../models/login-dto';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm!: FormGroup;
  loginData: LoginDto = new LoginDto('', '');
  authFailed: boolean = false;

  constructor(private authService: AuthService, private http: HttpClient,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.authFailed = false;
    this.loginForm = this.formBuilder.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]]
      });
  }

  login(event: Event) {
    //this.http.post("/api/registar", );
    //this.authService.login(this.loginData).subscribe({
    //  next: (response) => {
    //    localStorage.setItem('token', response.token);
    //    console.log('Login successful', response);
    //  },
    //  error: (err) => console.error('Login failed', err)
    //});


    
    event.preventDefault(); // Previne o comportamento padrão do formulário
    //if (!this.loginForm.valid) {
    //  return;
    //}

    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    this.authService.signIn(email, password).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigateByUrl("/registar"); // Redireciona para a página inicial
        }
      },
      error: () => {
        alert("Erro!"); // Mostra mensagem de erro se falhar
      }
    });
  }
}
