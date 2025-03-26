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
  isModalOpen: boolean = false; // Controla a exibição do modal delete
  isDetailsModalOpen: boolean = false; // Controla a exibição do modal de detalhes
  selectedUser: User | null = null; // Usuário selecionado para exibição de detalhes
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
    console.log('Detalhes do usuário:', user);
    this.selectedUser = user;
    this.isDetailsModalOpen = true;
  }

  // Ação do botão Apagar
  onBulkDelete(): void {
    this.isModalOpen = true; // Open the modal
  }

  onDelete(user: User): void {
    this.selectedUser = user;
    this.isModalOpen = true; // Open the modal
  }

  // Ação do botão Criar
  onCreate(): void {
    console.log('Botão Criar clicado');
    // Implemente a lógica para criar um novo usuário aqui
  }

  closeModal(): void {
    this.isModalOpen = false; // Close the modal
    this.selectedUser = null;
    this.selectedUsers = [];
  }

  confirmDelete(): void {
    if (this.selectedUser) {
      this.service.deleteUser(this.selectedUser.id).subscribe(
        (result) => {
          console.log(result);
          this.getUsers();
          this.closeModal();
        },
        (error) => {
          console.error(error);
        }
      );
    }
    for (let i = 0; i < this.selectedUsers.length; i++)
    {
      this.service.deleteUser(this.selectedUsers[i].id).subscribe(
        (result) => {
          console.log(result);
          this.getUsers();
          this.closeModal();
        },
        (error) => {
          console.error(error);
        }
      );
    }
  }

  // Fechar o modal de detalhes
  closeDetailsModal(): void {
    this.isDetailsModalOpen = false;
    this.selectedUser = null;
  }
}
