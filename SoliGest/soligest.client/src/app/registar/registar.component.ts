import { Component, OnInit } from "@angular/core";
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import { AuthorizeService } from "../../api-authorization/authorize.service";

@Component({
  selector: 'app-registar-component',
  standalone: false,
  templateUrl: './registar.component.html',
  styleUrl: './registar.component.css'
})
export class RegistarComponent implements OnInit {
  errors: string[] = [];
  registerForm!: FormGroup;
  registerFailed: boolean = false;
  registerSucceeded: boolean = false;
  signedIn: boolean = false;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder) {

    // Verificar se o utilizador já está autenticado
    this.signedIn = this.authService.isSignedIn();
  }

  ngOnInit(): void {
    this.registerFailed = false;
    this.registerSucceeded = false;
    this.errors = [];

    // Inicializar o formulário com validações
    this.registerForm = this.formBuilder.group(
      {
        name: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
        confirmPassword: ['', [Validators.required]]
      }, { validators: this.passwordMatchValidator });
  }

  // Validador para confirmar que password e confirmPassword coincidem
  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null | object => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (!password || !confirmPassword) {
      return null;
    }
    return password.value !== confirmPassword.value
      ? { passwordMismatch: true }
      : null;
  }

  // Método chamado ao submeter o formulário
  public register(): void {
    if (!this.registerForm.valid) {
      alert("Por favor corriga os erros do formulário!");
      return;
    }

    this.registerFailed = false;
    this.errors = [];

    const name = this.registerForm.get('name')?.value;
    const email = this.registerForm.get('email')?.value;
    const password = this.registerForm.get('password')?.value;

    // Chamada ao serviço de registo
    this.authService.register(name, email, password).forEach(
      response => {
        if (response) {
          this.registerSucceeded = true;
          alert("Registo bem sucedido!");
        }
      }).catch(
        error => {
          this.registerFailed = true;
          alert("Ocorreu um erro! Por favor tente novamente mais tarde.");
          if (error.error) {
            const errorObj = JSON.parse(error.error);
            if (errorObj && errorObj.errors) {
              // Processar os erros detalhados
              const errorList = errorObj.errors;
              for (let field in errorList) {
                if (Object.hasOwn(errorList, field)) {
                  let list: string[] = errorList[field];
                  for (let idx = 0; idx < list.length; idx += 1) {
                    this.errors.push(`${field}: ${list[idx]}`);
                  }
                }
              }
            }
          }
        });
  }
}
