import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService, User } from '../services/users.service'; // Importe o serviço e a interface User

@Component({
  selector: 'app-funcionario-edit',
  templateUrl: './funcionario-edit.component.html',
  styleUrls: ['./funcionario-edit.component.css'],
  standalone: false
})
export class FuncionarioEditComponent implements OnInit {
  user: User = {
    id: '',
    name: '',
    email: '',
    address1: '', // Morada 1
    address2: '', // Morada 2
    phoneNumber: 0, // Telemóvel
    birthDate: undefined, // Data de Nascimento
    role: '',
    folgasMes: [],
   feriasAno: []
  };

  folgaDia: Date | null = null; // Dia de folga selecionado
  folgasMes: Date[] = []; // Lista de dias de folga do mês

  feriasInicio: Date | null = null; // Data de início das férias
  feriasAno: { inicio: Date, fim: Date }[] = []; // Lista de períodos de férias

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private usersService: UsersService
  ) { }

  ngOnInit(): void {
    // Obtém o ID do utilizador da rota
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
     // this.carregarUtilizador(+userId);
    }
  }

  /*
  // Carrega os dados do utilizador
  carregarUtilizador(id: number): void {
    this.usersService.getUserById(id).subscribe(
      (user) => {
        this.user = user;
        this.folgasMes = user.folgasMes || [];
        this.feriasAno = user.feriasAno || [];
      },
      (error) => {
        console.error('Erro ao carregar utilizador:', error);
      }
    );
  }
  */

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
  onSubmit(): void {
    const userData = {
      ...this.user,
      folgasMes: this.folgasMes,
      feriasAno: this.feriasAno
    };

    this.usersService.updateUser(userData).subscribe(
      (response) => {
        console.log('Utilizador atualizado com sucesso:', response);
        this.router.navigate(['/funcionarios']); // Redireciona para a lista de utilizadores
      },
      (error) => {
        console.error('Erro ao atualizar utilizador:', error);
      }
    );
  }
}
