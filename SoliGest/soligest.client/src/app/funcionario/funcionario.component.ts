import { Component } from '@angular/core';
import { User, UsersService } from '../services/users.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-funcionario',
  templateUrl: './funcionario.component.html',
  styleUrls: ['./funcionario.component.css'],
  standalone: false
})
export class FuncionarioComponent {
  public users: User[] = []; // Lista de usuários
  selectedUsers: User[] = []; // Lista de usuários selecionados
  imagepath: string = "/assets/img/plus-18.png";

  constructor(private service: UsersService, private router: Router) { }

  ngOnInit() {
    this.getUsers();
  }

  // Obtém a lista de usuários do serviço
  getUsers() {
    this.service.getUsers().subscribe(
      (result) => {
        this.users = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  // Verifica se um usuário está selecionado
  isSelected(user: User): boolean {
    return this.selectedUsers.includes(user);
  }

  // Adiciona ou remove um usuário da lista de selecionados
  onSelectUser(user: User, event: any): void {
    if (event.target.checked) {
      this.selectedUsers.push(user); // Adiciona o usuário à lista
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u !== user); // Remove o usuário da lista
    }
  }

  // Ação do botão Editar
  onEdit(user: User): void {
    console.log('Editar usuário:', user);
    // Implemente a lógica de edição aqui
  }

  // Ação do botão Detalhes
  onDetails(user: User): void {
    console.log('Detalhes do usuário:', user); // Debugging output
    this.router.navigate(['/funcionario-details', user.id]); // Navigate to the details page
  }

  // Ação do botão Apagar
  onDelete(user: User): void {
    console.log('Apagar usuário:', user);
    // Implemente a lógica de exclusão aqui
  }

  // Ação do botão Criar
  onCreate(): void {
    console.log('Botão Criar clicado');
    // Implemente a lógica para criar um novo usuário aqui
  }
}
