import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-funcionario-create',
  templateUrl: './funcionario-create.component.html',
  styleUrls: ['./funcionario-create.component.css'],
  standalone: false
})
export class FuncionarioCreateComponent {
  user: any = {
    role: '' // Cargo do utilizador
  };

  folgaDia: Date | null = null; // Dia de folga selecionado
  folgasMes: Date[] = []; // Lista de dias de folga do mês

  feriasInicio: Date | null = null; // Data de início das férias
  feriasAno: { inicio: Date, fim: Date }[] = []; // Lista de períodos de férias

  constructor(private router: Router) { }

  // Adiciona um dia de folga à lista
  adicionarFolga() {
    if (this.folgaDia && this.folgasMes.length < 5) {
      this.folgasMes.push(this.folgaDia);
      this.folgaDia = null; // Limpa o campo após adicionar
    }
  }

  // Limpa a lista de dias de folga
  limparFolgas() {
    this.folgasMes = [];
  }

  // Adiciona um período de férias à lista
  adicionarFerias() {
    if (this.feriasInicio && this.feriasAno.length < 2) {
      const inicio = new Date(this.feriasInicio);
      const fim = new Date(inicio);
      fim.setDate(fim.getDate() + 13); // Adiciona 13 dias para totalizar 2 semanas
      this.feriasAno.push({ inicio, fim });
      this.feriasInicio = null; // Limpa o campo após adicionar
    }
  }

  // Limpa a lista de períodos de férias
  limparFerias() {
    this.feriasAno = [];
  }

  // Método chamado ao submeter o formulário
  onSubmit() {
    const userData = {
      ...this.user,
      folgasMes: this.folgasMes,
      feriasAno: this.feriasAno
    };

    console.log('Dados do utilizador:', userData);
    // Aqui você pode enviar os dados para o backend ou realizar outras ações
    this.router.navigate(['/funcionarios']); // Redireciona para a lista de utilizadores
  }
}
