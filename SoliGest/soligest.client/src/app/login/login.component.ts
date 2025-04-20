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

  public async login(event: Event): Promise<void> {
    event.preventDefault(); // Previne o comportamento padrão do formulário
    if (!this.loginForm.valid) {
      alert("Por favor corriga os erros do formulário!");
      return;
    }

    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    await this.authService.signIn(email, password).then(
      (result) => {
        result.subscribe(
          (result) => {
            if (result) {
              this.router.navigateByUrl("/");
              alert("Login efetuado com sucesso!"); // Redireciona para a página inicial
            } else {
              alert("Email ou password inválidos!");
            }
          },
          (error) => {
            this.authFailed = true; // Mostra mensagem de erro se falhar
            alert("Ocorreu um erro! Por favor tente novamente mais tarde.");
          },
        )
      },
      (error) => console.error(error)
      )
  }
}
