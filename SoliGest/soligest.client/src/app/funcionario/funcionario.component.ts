import { Component } from '@angular/core';
import { User, UsersService } from '../services/users.service';

@Component({
  selector: 'app-funcionario',
  standalone: false,

  templateUrl: './funcionario.component.html',
  styleUrl: './funcionario.component.css'
})
export class FuncionarioComponent {
  public users: User[] = [];
  selectedUser: User | undefined;
  imagepath: string = "/assets/img/plus-18.png";

  constructor(private service: UsersService) { }

  ngOnInit() {
    this.getUsers();
  }

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

  onSelectUser(user: User) {
    this.selectedUser = user;
  }
}
