import { Component, OnInit } from '@angular/core';
import { User, UsersService } from '../services/users.service';
import { Router } from '@angular/router';

declare var google: any;

@Component({
  selector: 'app-funcionario',
  templateUrl: './funcionario.component.html',
  styleUrls: ['./funcionario.component.css'],
  standalone: false
})
export class FuncionarioComponent implements OnInit {
  public users: User[] = [];
  selectedUsers: User[] = [];
  isBulkDeleteModalOpen: boolean = false;
  isDeleteModalOpen: boolean = false;
  isDetailsModalOpen: boolean = false;
  selectedUser: User | null = null;
  imagepath: string = "/assets/img/plus-18.png";
  private map: any;

  constructor(private service: UsersService, private router: Router) { }

  ngOnInit() {
    this.getUsers();
    this.initMap();
  }

  initMap() {
    // Verifica se a API do Google Maps está carregada
    if (typeof google !== 'undefined') {
      const center = { lat: 38.7223, lng: -9.1393 }; // Coordenadas de Lisboa

      this.map = new google.maps.Map(document.getElementById("map"), {
        zoom: 12,
        center: center,
        mapTypeId: "roadmap",
        styles: [
          {
            featureType: "poi",
            stylers: [{ visibility: "off" }]
          }
        ]
      });

      // Adiciona marcador central
      new google.maps.Marker({
        position: center,
        map: this.map,
        title: "Sede Principal"
      });
    }
  }

  getUsers() {
    this.service.getUsers().subscribe(
      (result) => {
        this.users = result;
        this.addUserMarkers();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  addUserMarkers() {
    if (!this.map || !this.users.length) return;

    // Limpa marcadores existentes (se necessário)

    // Adiciona marcadores para cada usuário (exemplo)
    this.users.forEach(user => {
      // Simula coordenadas aleatórias próximas à sede
      const lat = 38.7223 + (Math.random() * 0.1 - 0.05);
      const lng = -9.1393 + (Math.random() * 0.1 - 0.05);

      new google.maps.Marker({
        position: { lat, lng },
        map: this.map,
        title: user.name,
        label: user.name.charAt(0)
      });
    });
  }

  isSelected(user: User): boolean {
    return this.selectedUsers.includes(user);
  }

  onSelectUser(user: User, event: any): void {
    if (event.target.checked) {
      this.selectedUsers.push(user);
    } else {
      this.selectedUsers = this.selectedUsers.filter(u => u !== user);
    }
  }

  onDetails(user: User): void {
    this.selectedUser = user;
    this.isDetailsModalOpen = true;
  }

  onBulkDelete(): void {
    this.isBulkDeleteModalOpen = true;
  }

  onDelete(user: User): void {
    this.selectedUser = user;
    this.isDeleteModalOpen = true;
  }

  closeModal(): void {
    this.isDeleteModalOpen = false;
    this.isBulkDeleteModalOpen = false;
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

    if (this.selectedUsers.length > 0) {
      this.selectedUsers.forEach(user => {
        this.service.deleteUser(user.id).subscribe(
          (result) => {
            console.log(result);
            this.getUsers();
          },
          (error) => {
            console.error(error);
          }
        );
      });
      this.closeModal();
    }
  }

  closeDetailsModal(): void {
    this.isDetailsModalOpen = false;
  }
}
