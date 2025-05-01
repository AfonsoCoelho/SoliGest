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
  // Cenas do popup
  popupVisible: boolean = false;
  popupMessage: string = '';
  popupType: 'success' | 'error' = 'success';
  timerInterval!: any;
  timerWidth = 100;

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
      //alert("Por favor corriga os erros do formulário!");
      this.showPopup('error', `Por favor corriga os erros do formulário!`);
      return;
    }

    const email = this.loginForm.get('email')?.value;
    const password = this.loginForm.get('password')?.value;

    this.authService.signIn(email, password).subscribe(
      (result) => {
          if (result) {
            //this.router.navigateByUrl("/");
            //alert("Login efetuado com sucesso!"); // Redireciona para a página inicial
            this.showPopup('success', `Login efetuado com sucesso!`);
          } else {
            //alert("Email ou password inválidos!");
            this.showPopup('error', `Email ou password inválidos!`);
          }
          },
          (error) => {
            this.authFailed = true; // Mostra mensagem de erro se falhar
            //alert("Ocorreu um erro! Por favor tente novamente mais tarde.");
            this.showPopup('error', `Ocorreu um erro! Por favor tente novamente mais tarde.`);
          },
        )
  }

  showPopup(type: 'success' | 'error', message: string) {
    // inicializa
    this.popupType = type;
    this.popupMessage = message;
    this.timerWidth = 100;
    this.popupVisible = true;

    // limpa qualquer intervalo anterior
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }

    // diminui 2% a cada 100ms → 50 ciclos → 5s total
    this.timerInterval = setInterval(() => {
      this.timerWidth -= 2;
      if (this.timerWidth <= 0) {
        this.closePopup();
      }
    }, 100);
  }

  closePopup() {
    this.popupVisible = false;
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    if (this.popupMessage == "Login efetuado com sucesso!") {
      this.router.navigateByUrl("/");
    }
  }
}
