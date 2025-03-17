import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'app-changepw',
  standalone: false,
  templateUrl: './changepw.component.html',
  styleUrl: './changepw.component.css'
})
export class ChangepwComponent implements OnInit{
  resetPasswordForm!: FormGroup;
  authFailed: boolean = false;
  signedIn: boolean = false;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router) {
    this.signedIn = this.authService.isSignedIn();
  }

  ngOnInit(): void {
    this.authFailed = false;
    this.resetPasswordForm = this.formBuilder.group(
      {
        newPassword: ['', [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
        confirmPassword: ['', [Validators.required]]
      }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null | object => {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');
    if (!password || !confirmPassword) {
      return null;
    }
    return password.value !== confirmPassword.value
      ? { passwordMismatch: true }
      : null;
  }

  public resetPasssword(): void {
    //event.preventDefault(); // Previne o comportamento padrão do formulário
    if (!this.resetPasswordForm.valid) {
      return;
    }

    const password = this.resetPasswordForm.get('newPassword')?.value;

    this.authService.resetPassword(password).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigateByUrl("/");
          alert("Palavra-passe reposta com sucesso!"); // Redireciona para a página inicial
        }
      },
      error: () => {
        this.authFailed = true; // Mostra mensagem de erro se falhar
      }
    });
  }
}
