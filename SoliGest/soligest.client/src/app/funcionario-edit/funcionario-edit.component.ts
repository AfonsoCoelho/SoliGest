import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService, User } from '../services/users.service'; // Importe o serviço e a interface User
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-funcionario-edit',
  templateUrl: './funcionario-edit.component.html',
  styleUrls: ['./funcionario-edit.component.css'],
  standalone: false
})
export class FuncionarioEditComponent implements OnInit {
  errors: string[] = [];
  funcionarioEditForm!: FormGroup;
  funcionarioEditFailed: boolean = false;
  funcionarioEditSucceeded: boolean = false;
  utilizador: User | undefined;

  

  user: User | undefined;

  folgaDia: Date | null = null; // Dia de folga selecionado
  folgasMes: Date[] = []; // Lista de dias de folga do mês

  feriasInicio: Date | null = null; // Data de início das férias
  feriasAno: { inicio: Date, fim: Date }[] = []; // Lista de períodos de férias

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private usersService: UsersService,
    private formBuilder: FormBuilder
  ) {
    
}

  ngOnInit(): void {
    
    this.getUser();

    this.funcionarioEditFailed = false;
    this.funcionarioEditSucceeded = false;
    this.errors = [];
  }

 

  // Adiciona um dia de folga à lista
  adicionarFolga(): void {
    if (this.folgaDia && this.folgasMes.length < 5) {
      this.folgasMes.push(this.folgaDia);
      this.folgaDia = null; // Limpa o campo após adicionar
    }
  }

  // Limpa a lista de dias de folga
  limparFolgas(): void {
    this.folgasMes = [];
  }

  // Adiciona um período de férias à lista
  adicionarFerias(): void {
    if (this.feriasInicio && this.feriasAno.length < 2) {
      const inicio = new Date(this.feriasInicio);
      const fim = new Date(inicio);
      fim.setDate(fim.getDate() + 13); // Adiciona 13 dias para totalizar 2 semanas
      this.feriasAno.push({ inicio, fim });
      this.feriasInicio = null; // Limpa o campo após adicionar
    }
  }

  // Limpa a lista de períodos de férias
  limparFerias(): void {
    this.feriasAno = [];
  }

  // Método chamado ao submeter o formulário
  //onSubmit(): void {
  //  const userData = {
  //    ...this.user,
  //    folgasMes: this.folgasMes,
  //    feriasAno: this.feriasAno
  //  };

  //  this.usersService.updateUser(userData).subscribe(
  //    (response) => {
  //      console.log('Utilizador atualizado com sucesso:', response);

  //      this.usersService.saveDaysOff(this.user.id, this.folgasMes).subscribe(
  //        (respFolgas) => {
  //          console.log('Folgas salvas', respFolgas);

  //          this.usersService.saveHolidays(this.user.id, this.feriasAno).subscribe(
  //            (respFerias) => {
  //              console.log('Férias salvas', respFerias);

  //              this.router.navigate(['/funcionarios']);
  //            },
  //            (error) => console.error('Erro ao salvar ferias:', error)
  //          );
  //        },
  //        (error) => console.error('Erro ao salvar folgas:', error)
  //      );
  //    },
  //    (error) => console.error('Erro ao atualizar user:', error)
  //  );
  //}

  getUser(): void {
    // Obtém o ID do utilizador da rota
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      // this.carregarUtilizador(+userId);
      this.usersService.getUser(userId).subscribe({
        next: res => {
          this.user = res;
          this.funcionarioEditForm = this.formBuilder.group(
            {
              id: { value: this.user.id, disabled: true },
              name: [this.user.name, Validators.required],
              email: [this.user.email, [Validators.required, Validators.email]],
              address1: [this.user.address1],
              address2: [this.user.address2],
              phoneNumber: [this.user.phoneNumber],
              birthDate: [this.user.birthDate],
              role: [this.user.role],
              dayOff: [this.user.dayOff],
              startHoliday: [this.user.startHoliday],
              endHoliday: [this.user.endHoliday]
            });
        },
        error: err => {
          console.error(err);
        }
      });
    }
  }

  update() {
    const id = this.route.snapshot.paramMap.get('id');
    const name = this.funcionarioEditForm.get('name')?.value;
    const email = this.funcionarioEditForm.get('email')?.value;
    const address1 = this.funcionarioEditForm.get('address1')?.value;
    const address2 = this.funcionarioEditForm.get('address2')?.value;
    const phoneNumber = this.funcionarioEditForm.get('phoneNumber')?.value;
    const birthDate = this.funcionarioEditForm.get('birthDate')?.value;
    const role = this.funcionarioEditForm.get('role')?.value;
    const dayOff = this.funcionarioEditForm.get('dayOff')?.value;
    const startHoliday = this.funcionarioEditForm.get('startHoliday')?.value;
    const endHoliday = this.funcionarioEditForm.get('endHoliday')?.value;
    //if (id) {
    //  this.usersService.updateUser({ id, name, address1, address2, birthDate, email, phoneNumber }).subscribe({
    //    next: res => {
    //      console.log("Utilizador atualizado!");
    //    },
    //    error: err => {
    //      console.error(err);
    //    }
    //  });
    //}

    if (id) {
      this.usersService.updateUser(id, name, address1, address2, phoneNumber, birthDate, email, role, dayOff, startHoliday, endHoliday).subscribe(res => {
        alert('Utilizador atualizado com sucesso!');
        this.router.navigateByUrl('people');
      });
    }
  }
}
