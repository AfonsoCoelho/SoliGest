import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'app-funcionario-create',
  templateUrl: './funcionario-create.component.html',
  styleUrls: ['./funcionario-create.component.css'],
  standalone: false
})
export class FuncionarioCreateComponent implements OnInit {
  errors: string[] = [];
  funcionarioCreateForm!: FormGroup;
  funcionarioCreateFailed: boolean = false;
  funcionarioCreateSucceeded: boolean = false;
  signedIn: boolean = false;

  user: any = {
    name: '',
    email: '',
    address1: '',
    address2: '',
    phoneNumber: '',
    birthDate: '',
    password: '',
    confirmPassword: '',
    role: ''
  };

  folgaDia: Date | null = null;
  folgasMes: Date[] = [];
  
  feriasInicio: Date | null = null;
  feriasAno: { inicio: Date, fim: Date }[] = [];
  
  passwordStrength = 0;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router) {
      this.signedIn = this.authService.isSignedIn();
  }
    ngOnInit(): void {
      this.funcionarioCreateFailed = false;
      this.funcionarioCreateSucceeded = false;
      this.errors = [];

      // Inicializar o formulário com validações
      this.funcionarioCreateForm = this.formBuilder.group(
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

  checkPasswordStrength() {
    const password = this.user.password;
    let strength = 0;
    
    // Length check
    if (password.length > 5) strength += 20;
    if (password.length > 8) strength += 20;
    
    // Complexity checks
    if (/[A-Z]/.test(password)) strength += 20;
    if (/[0-9]/.test(password)) strength += 20;
    if (/[^A-Za-z0-9]/.test(password)) strength += 20;
    
    this.passwordStrength = Math.min(strength, 100);
  }

  getPasswordStrengthColor() {
    if (this.passwordStrength < 40) return '#ff4444';
    if (this.passwordStrength < 70) return '#ffbb33';
    return '#00C851';
  }

  adicionarFolga() {
    if (this.folgaDia && this.folgasMes.length < 5) {
      this.folgasMes.push(new Date(this.folgaDia));
      this.folgaDia = null;
    }
  }

  removerFolga(index: number) {
    this.folgasMes.splice(index, 1);
  }

  limparFolgas() {
    this.folgasMes = [];
  }

  adicionarFerias() {
    if (this.feriasInicio && this.feriasAno.length < 2) {
      const inicio = new Date(this.feriasInicio);
      const fim = new Date(inicio);
      fim.setDate(fim.getDate() + 13); // 2 semanas = 14 dias (inclusive)
      this.feriasAno.push({ inicio, fim });
      this.feriasInicio = null;
    }
  }

  removerFerias(index: number) {
    this.feriasAno.splice(index, 1);
  }

  limparFerias() {
    this.feriasAno = [];
  }

  formValido() {
    return this.user.name && 
           this.user.email && 
           this.user.address1 && 
           this.user.phoneNumber && 
           this.user.birthDate && 
           this.user.password && 
           this.user.password === this.user.confirmPassword && 
           this.user.role;
  }

  onSubmit() {
    if (this.formValido()) {
      const userData = {
        ...this.user,
        folgasMes: this.folgasMes,
        feriasAno: this.feriasAno
      };
      
      console.log('Dados do utilizador:', userData);
      // Aqui você enviaria os dados para o backend
      this.router.navigate(['/funcionarios']);
    }
  }
}
