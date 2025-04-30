import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { UsersService } from '../services/users.service';

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
  profilePic: any;

  user: any = {
    profilePictureUrl: '',
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

  // Cenas do popup
  popupVisible: boolean = false;
  popupMessage: string = '';
  popupType: 'success' | 'error' = 'success';
  timerInterval!: any;
  timerWidth = 100;

  constructor(private authService: AuthorizeService,
    private formBuilder: FormBuilder,
    private router: Router, private usersService: UsersService ) {
  }
    ngOnInit(): void {
      this.funcionarioCreateFailed = false;
      this.funcionarioCreateSucceeded = false;
      this.errors = [];

      // Inicializar o formulário com validações
      this.funcionarioCreateForm = this.formBuilder.group(
        {
          profilePic: [''],
          name: ['', Validators.required],
          email: ['', [Validators.required, Validators.email]],
          address1: [],
          address2: [],
          phoneNumber: [],
          birthDate: [],
          password: ['', [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]],
          confirmPassword: ['', [Validators.required]],
          role: [],
          dayOff: [],
          startHoliday: [],
          endHoliday: []
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

  //onSubmit() {
  //  if (this.formValido()) {


  //    this.authService.register(this.user.name, this.user.email, this.user.password).subscribe(
  //      (response) => {
  //        console.log('Utilizador criado com sucesso:', response);

  //        this.usersService.saveDaysOff(this.user.id, this.folgasMes).subscribe(
  //          (respFolgas) => {
  //            console.log('Folgas salvas', respFolgas);

  //            this.usersService.saveHolidays(this.user.id, this.feriasAno).subscribe(
  //              (respFerias) => {
  //                console.log('Férias salvas', respFerias);

  //                this.router.navigate(['/funcionarios']);
  //              },
  //              (error) => console.error('Erro ao salvar ferias:', error)
  //            );
  //          },
  //          (error) => console.error('Erro ao salvar folgas:', error)
  //        );
  //      },
  //      (error) => console.error('Erro ao criar user:', error)
  //    );
  //  }
  //    this.router.navigate(['/funcionarios']);
  // }
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
    if (this.popupMessage = 'Registo bem sucedido!') {
      this.router.navigateByUrl("/");
    }
  }

  onFileSelected(event: any) {
    this.profilePic = event.target.files[0] as File;
    console.log(this.profilePic);
  }

  public register(): void {
    if (!this.funcionarioCreateForm.valid) {
      //alert("Por favor corriga os erros do formulário!");
      this.showPopup('error', `Por favor corriga os erros do formulário!`)
      return;
    }

    this.funcionarioCreateFailed = false;
    this.errors = [];

    const profilePic = this.funcionarioCreateForm.get('profilePic');
    if (profilePic?.value instanceof File) {
      var profilePicFile = profilePic.value as File;
    }
    //alert(profilePic?.value);
    //this.showPopup('success', `Utilizador criado com sucesso`) // mudar depois, tirar
    const name = this.funcionarioCreateForm.get('name')?.value;
    const email = this.funcionarioCreateForm.get('email')?.value;
    const password = this.funcionarioCreateForm.get('password')?.value;
    const address1 = this.funcionarioCreateForm.get('address1')?.value;
    const address2 = this.funcionarioCreateForm.get('address2')?.value;
    const phoneNumber = this.funcionarioCreateForm.get('phoneNumber')?.value;
    const birthDate = this.funcionarioCreateForm.get('birthDate')?.value;
    const role = this.funcionarioCreateForm.get('role')?.value;
    const dayOff = this.funcionarioCreateForm.get('dayOff')?.value;
    const startHoliday = this.funcionarioCreateForm.get('startHoliday')?.value;
    const endHoliday = this.funcionarioCreateForm.get('endHoliday')?.value;

    // Chamada ao serviço de registo
    this.authService.register(name, address1, address2, phoneNumber, birthDate, email, password, role, dayOff, startHoliday, endHoliday).forEach(
      response => {
        if (response) {
          this.funcionarioCreateSucceeded = true;
          this.usersService.getUserByEmail(email).subscribe(
            (result) => {
              this.usersService.saveProfilePicture(result.id, this.profilePic).subscribe(
                (result) => this.showPopup('success', `Foto de perfil guardada`),
                (error) => this.showPopup('error', `Erro ao guardar a foto de perfil`)
              )
            },
            (error) => this.showPopup('error', `Erro a ir buscar o utilizador após criação`)
          )
          this.showPopup('success', `Registo bem sucedido!`);
          //this.router.navigateByUrl("/");
          //alert("Registo bem sucedido!");
        }
      }).catch(
        error => {
          this.funcionarioCreateFailed = true;
          //alert("Ocorreu um erro! Por favor tente novamente mais tarde.");
          this.showPopup('error', `Ocorreu um erro! Por favor tente novamente mais tarde.`);
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
