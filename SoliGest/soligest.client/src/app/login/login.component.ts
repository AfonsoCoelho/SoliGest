import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthorizeService } from "../../api-authorization/authorize.service";

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  authFailed: boolean = false;
  signedIn: boolean = false;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router) {
    this.signedIn = this.authService.isSignedIn();
  }

  ngOnInit(): void {
    this.authFailed = false;
    this.loginForm = this.formBuilder.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required]]
      });
  }

  public login(event: Event): void {
    event.preventDefault(); // Previne o comportamento padrão do formulário
    if (!this.loginForm.valid) {
      alert("Por favor corriga os erros do formulário!");
      return;
    }

    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    this.authService.signIn(email, password).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigateByUrl("/");
          alert("Login efetuado com sucesso!"); // Redireciona para a página inicial
        }
      },
      error: () => {
        this.authFailed = true; // Mostra mensagem de erro se falhar
        alert("Email ou password inválidos!");
      }
    });
  }
}
