/*
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
*/

import { Component } from '@angular/core';

@Component({
  selector: 'app-funcionario-details',
  templateUrl: './funcionario-details.component.html',
  styleUrls: ['./funcionario-details.component.css'],
  standalone: false
})
export class FuncionarioDetailsComponent {
  user = {
    id: 1,
    name: 'João Silva',
    role: 'Administrador',
    email: 'joao.silva@example.com',
    birthDate: new Date(1990, 5, 15), // 15/06/1990
    phoneNumber: '+351 912345678',
    feriasAno: [
      { inicio: new Date(2024, 6, 1), fim: new Date(2024, 6, 15) }, // 01/07/2024 - 15/07/2024
      { inicio: new Date(2024, 11, 20), fim: new Date(2025, 0, 5) } // 20/12/2024 - 05/01/2025
    ],
    folgasMes: [
      new Date(2024, 2, 10), // 10/03/2024
      new Date(2024, 2, 17)  // 17/03/2024
    ]
  };

  onEdit() {
    console.log('Editar usuário:', this.user);
    alert('Funcionalidade de edição não implementada!');
  }
}
