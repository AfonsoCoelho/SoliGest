import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-pwrecovery',
  standalone: false,
  
  templateUrl: './pwrecovery.component.html',
  styleUrl: './pwrecovery.component.css'
})
export class PwrecoveryComponent implements OnInit {
  pwRecoveryForm!: FormGroup;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router) {  }

  ngOnInit(): void {
    this.pwRecoveryForm = this.formBuilder.group(
      {
        email: ['', [Validators.required, Validators.email]]
      });
  }

  public pwRecovery(): void {
    if (!this.pwRecoveryForm.valid) {
      alert("Por favor corriga os erros do formulário!");
      return;
    }

    const email = this.pwRecoveryForm.get('email')?.value;

    this.authService.pwRecovery(email).subscribe({
      next: (response) => {
        if (response) {
          //this.router.navigateByUrl("/");
          alert("Email enviado com sucesso!!"); // Redireciona para a página inicial
        }
      },
      error: () => {
        alert("Ocorreu um erro! Por favor tente novamente mais tarde.");
      }
    });
  }

}
