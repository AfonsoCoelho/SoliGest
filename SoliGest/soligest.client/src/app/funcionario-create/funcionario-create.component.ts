import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UsersService, User } from '../services/users.service'; // Importe o serviço e a interface User

@Component({
  selector: 'app-funcionario-create',
  templateUrl: './funcionario-create.component.html',
  styleUrls: ['./funcionario-create.component.css'],
  standalone: false
})
export class FuncionarioCreateComponent {
  // Objeto para armazenar os dados do formulário
  user: User = {
    id: '',
    name: '',
    //address1: '',
    //address2: '',
    email: '',
    birthDate: undefined,
    //password: '',
    //confirmPassword: '',
    role: '',
    phoneNumber: 0 // Adicione outros campos conforme necessário
  };

  constructor(
    private usersService: UsersService, // Injete o serviço
    private router: Router // Injete o Router para navegação
  ) { }

  // Método chamado ao submeter o formulário
  /*
  onSubmit() {
    if (this.user.password !== this.user.confirmPassword) {
      alert('As passwords não coincidem!');
      return;
    } 

    
    // Chama o serviço para criar o utilizador
    this.usersService.createUser(this.user).subscribe(
      (response) => {
        console.log('Utilizador criado com sucesso:', response);
        this.router.navigate(['/funcionarios']); // Redireciona para a lista de utilizadores
      },
      (error) => {
        console.error('Erro ao criar utilizador:', error);
      }
    );
  }
  */
}
