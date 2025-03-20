import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-funcionario',
  standalone: false,
  templateUrl: './funcionario.component.html',
  styleUrls: ['./funcionario.component.css']
})
export class FuncionarioComponent implements OnInit {
  funcionarios: any[] = [];
  selectedFuncionario: any | null = null;

  constructor() { }

  ngOnInit() {
    // replace this later when connecting to a real backend (simulado por agora)
    this.funcionarios = [
      { id: 1, nome: 'João', cargo: 'Técnico', email: 'joao@email.com', dataNascimento: '1990-01-01', telemovel: '912345678' },
      { id: 2, nome: 'Maria', cargo: 'Administrativo', email: 'maria@email.com', dataNascimento: '1985-06-15', telemovel: '987654321' }
    ];
  }

  onSelectFuncionario(funcionario: any) {
    this.selectedFuncionario = funcionario;
  }

  onSubmit(form: any) {
    if (form.valid) {
      const novoFuncionario = {
        id: this.funcionarios.length + 1,
        ...form.value
      };
      this.funcionarios.push(novoFuncionario);
      form.reset();
    }
  }

  onDelete() {
    if (this.selectedFuncionario) {
      this.funcionarios = this.funcionarios.filter(f => f.id !== this.selectedFuncionario.id);
      this.selectedFuncionario = null;
    }
  }
}
