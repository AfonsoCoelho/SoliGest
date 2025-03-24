import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User, UsersService } from '../services/users.service';

@Component({
  selector: 'app-funcionario-details',
  templateUrl: './funcionario-details.component.html',
  styleUrls: ['./funcionario-details.component.css'],
  standalone: false
})
export class FuncionarioDetailsComponent implements OnInit {
  public user: User | undefined;

  constructor(
    private route: ActivatedRoute,
    private service: UsersService
  ) { }

  ngOnInit(): void {
    this.getUser();
  }

  getUser(): void {
    const id = Number(this.route.snapshot.paramMap.get('id')); // Gets ID from URL
    console.log('User ID from URL:', id); // Debugging step

    if (id) {
      this.service.getUser(id).subscribe(
        (user) => {
          this.user = user;
          console.log('Fetched user:', this.user); // Debugging step
        },
        (error) => console.error('Error fetching user:', error)
      );
    }
  }

  onEdit(): void {
    console.log('Editar usuário:', this.user);
    // Adicione navegação para página de edição aqui, se necessário
  }
}
