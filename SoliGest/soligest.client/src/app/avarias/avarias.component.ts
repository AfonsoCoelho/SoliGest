import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AssistanceRequestsService, AssistanceRequest, AssistanceRequestCreateModel } from '../services/assistance-requests.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SolarPanel, SolarPanelsService } from '../services/solar-panels.service';
import { formatDate } from '@angular/common';
import { UsersService, User } from '../services/users.service';

// Declaração do objeto google para TypeScript
declare var google: any;

@Component({
  selector: 'app-avarias',
  templateUrl: './avarias.component.html',
  styleUrls: ['./avarias.component.css'],
  standalone: false
})
export class AvariasComponent implements OnInit {
  // Variáveis para ordenação
  sortBy: string = 'id';
  sortDirection: string = 'asc';
  searchTerm: string = '';
  public assistanceRequestsData: AssistanceRequest[] = [];
  filteredAvarias: AssistanceRequest[] = [];
  selectedAvaria: AssistanceRequest | null = null;
  selectedMarker: any = null;
  infoWindow: any = null;
  markers: any[] = [];
  public users: User[] = []; // Array para armazenar usuários
  autoAllocationCandidate: User | null = null;
  availableTechnicians: User[] = [];


  // Modais
  showModal: boolean = false;
  showEditModal: boolean = false;
  showDeleteConfirm: boolean = false;
  isManualAllocateModalOpen: boolean = false;
  isAutoAllocateModalOpen: boolean = false;

  // Formulários
  selectedPanelId: number = 0;
  selectedPriority: string = '';
  selectedStatus: string = '';
  newAvariaDescription: string = ''; // Adicionado para o formulário de criação


  constructor(
    private http: HttpClient,
    private aRService: AssistanceRequestsService,
    private sPService: SolarPanelsService,
    private uService: UsersService,
    private router: Router
  ) { }

  panels: SolarPanel[] = [];
  avariasData: AssistanceRequest[] = [];
  sortedAvarias: AssistanceRequest[] = [];
  map: any;

  openCreateModal(): void {
    this.selectedPanelId = 0;
    this.selectedPriority = '';
    this.selectedStatus = '';
    this.newAvariaDescription = '';
    this.showModal = true;
  }

  onCloseModal(): void {
    this.showModal = false;
  }

  openEditModal(avaria: AssistanceRequest): void {
    this.selectedAvaria = { ...avaria }; // Cria uma cópia para evitar alterações diretas
    this.selectedPanelId = avaria.solarPanel.id;
    this.selectedPriority = avaria.priority || '';
    this.selectedStatus = avaria.status;
    this.showEditModal = true;
  }

  onCloseEditModal(): void {
    this.showEditModal = false;
    this.selectedAvaria = null; // Limpar selectedAvaria ao fechar o modal
  }

  openDeleteConfirm(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;
    this.showDeleteConfirm = true;
  }

  onCloseDeleteConfirm(): void {
    this.showDeleteConfirm = false;
    this.selectedAvaria = null; // Limpar selectedAvaria ao fechar o modal
  }

  // Restante dos métodos existentes...
  loadSolarPanels(): void {
    this.sPService.getSolarPanels().subscribe(
      (result) => {
        this.panels = result;
      },
      (error) => {
        console.error(error);
      }
    );
    this.initMap();
  }

  loadAssistanceRequests(): void {
    this.aRService.getAll().subscribe(
      (result) => {
        this.avariasData = result;
        this.sortAvarias();
        this.filterAvarias();
      },
      (error) => {
        console.error(error);
      }
    );
    this.initMap();
  }

  loadUsers(): void {
    this.uService.getUsers().subscribe(
      (result) => {
        this.users = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  ngOnInit(): void {
    this.loadSolarPanels();
    this.loadAssistanceRequests();
    this.loadUsers();
  }

  initMap(): void {
    const center = { lat: 39.3999, lng: -8.2245 };

    this.map = new google.maps.Map(document.getElementById("map"), {
      zoom: 7,
      center: center,
      mapTypeId: "terrain",
      styles: [
        {
          "featureType": "administrative",
          "elementType": "labels.text.fill",
          "stylers": [{ "color": "#444444" }]
        },
        {
          "featureType": "landscape",
          "elementType": "all",
          "stylers": [{ "color": "#f2f2f2" }]
        },
        {
          "featureType": "poi",
          "elementType": "all",
          "stylers": [{ "visibility": "off" }]
        },
        {
          "featureType": "road",
          "elementType": "all",
          "stylers": [
            { "saturation": -100 },
            { "lightness": 45 }
          ]
        },
        {
          "featureType": "road.highway",
          "elementType": "all",
          "stylers": [{ "visibility": "simplified" }]
        },
        {
          "featureType": "road.arterial",
          "elementType": "labels.icon",
          "stylers": [{ "visibility": "off" }]
        },
        {
          "featureType": "transit",
          "elementType": "all",
          "stylers": [{ "visibility": "off" }]
        },
        {
          "featureType": "water",
          "elementType": "all",
          "stylers": [
            { "color": "#46bcec" },
            { "visibility": "on" }
          ]
        }
      ]
    });


    this.addAvariaMarkers();
  }

  addAvariaMarkers(): void {
    this.sortedAvarias.forEach(avaria => {
      if (avaria.solarPanel.latitude && avaria.solarPanel.longitude) {
        const marker = new google.maps.Marker({
          position: { lat: avaria.solarPanel.latitude, lng: avaria.solarPanel.longitude },
          map: this.map,
          title: `Avaria ID: ${avaria.id}`,
          icon: this.getMarkerIcon(avaria.status)
        });

        this.markers.push(marker);

        const infoWindow = new google.maps.InfoWindow({
          content: `
          <div style="padding: 10px;">
            <h3 style="margin: 0 0 5px 0;">Avaria ID: ${avaria.id}</h3>
            <p style="margin: 0 0 5px 0;"><strong>Localização:</strong> ${avaria.solarPanel.name}</p>
            <p style="margin: 0 0 5px 0;"><strong>Prioridade:</strong> ${avaria.priority || 'N/A'}</p>
            <p style="margin: 0; color: ${this.getStatusColor(avaria.status)}; font-weight: bold;">
              <strong>Estado:</strong> ${avaria.status}
            </p>
          </div>
        `
        });

        marker.addListener('click', () => {
          infoWindow.open(this.map, marker);
        });
      }
    });
  }

  getMarkerIcon(status: string): string {
    const iconBase = 'https://maps.google.com/mapfiles/ms/icons/';
    switch (status.toLowerCase()) {
      case 'vermelho': return iconBase + 'red-dot.png';
      case 'verde': return iconBase + 'green-dot.png';
      case 'amarelo': return iconBase + 'yellow-dot.png';
      default: return iconBase + 'blue-dot.png';
    }
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'vermelho': return '#ff0000';
      case 'verde': return '#00aa00';
      case 'amarelo': return '#ffaa00';
      default: return '#000000';
    }
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'vermelho': return 'status-red';
      case 'verde': return 'status-green';
      case 'amarelo': return 'status-yellow';
      default: return '';
    }
  }

  sortAvarias(): void {
    this.sortedAvarias = [...this.avariasData].sort((a, b) => {
      let valueA: any, valueB: any;

      switch (this.sortBy) {
        case 'priority':
          const priorityOrder: { [key: string]: number } = { 'Alta': 3, 'Média': 2, 'Baixa': 1 };
          valueA = a.priority ? priorityOrder[a.priority] : 0;
          valueB = b.priority ? priorityOrder[b.priority] : 0;
          break;
        case 'name':
          valueA = a.solarPanel.name.toLowerCase();
          valueB = b.solarPanel.name.toLowerCase();
          break;
        case 'id':
          valueA = a.id;
          valueB = b.id;
          break;
        case 'status':
          const statusOrder: { [key: string]: number } = { 'Vermelho': 3, 'Amarelo': 2, 'Verde': 1 };
          valueA = statusOrder[a.status] || 0;
          valueB = statusOrder[b.status] || 0;
          break;
        default:
          valueA = a.id;
          valueB = b.id;
      }

      return this.sortDirection === 'asc' ? (valueA < valueB ? -1 : 1) : (valueA > valueB ? -1 : 1);
    });

    this.filterAvarias();
    if (this.map) {
      this.clearMarkers();
      this.addAvariaMarkers();
    }
  }

  clearMarkers(): void {
    this.initMap();
  }

  filterAvarias(): void {
    this.filteredAvarias = this.sortedAvarias.filter(
      avaria => avaria.solarPanel.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  selectAvaria(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;

    if (this.map && avaria.solarPanel.latitude && avaria.solarPanel.longitude) {
      this.map.setCenter({ lat: avaria.solarPanel.latitude, lng: avaria.solarPanel.longitude });
      this.map.setZoom(15);

      if (this.infoWindow) {
        this.infoWindow.close();
      }

      const marker = this.markers.find(m => {
        const position = m.getPosition();
        return position?.lat() !== undefined && position?.lng() !== undefined &&
          Math.abs(position.lat() - avaria.solarPanel.latitude!) < 0.0001 &&
          Math.abs(position.lng() - avaria.solarPanel.longitude!) < 0.0001;
      });

      if (marker) {
        const content = `
          <div style="padding: 10px; max-width: 300px;">
            <h3 style="margin: 0 0 5px 0;">Avaria ID: ${avaria.id}</h3>
            <p><strong>Painel:</strong> ${avaria.solarPanel.name}</p>
            <p><strong>Descrição:</strong> ${avaria.description || 'N/A'}</p>
            <p><strong>Data do pedido:</strong> ${avaria.requestDate}</p>
            <p><strong>Data de resolução:</strong> ${avaria.resolutionDate || 'Ainda não resolvida'}</p>
            <p><strong>Prioridade:</strong> ${avaria.priority || 'N/A'}</p>
            <p style="color: ${this.getStatusColor(avaria.status)}; font-weight: bold;">
              <strong>Estado:</strong> ${avaria.status}
            </p>
          </div>
        `;

        this.infoWindow = new google.maps.InfoWindow({ content });
        this.infoWindow.open(this.map, marker);
      }
    }
  }

  getAvariaClass(avaria: AssistanceRequest): any {
    return {
      [this.getStatusClass(avaria.status)]: true,
      'selected': avaria === this.selectedAvaria
    };
  }

  openManualAllocateModal(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;
    // Filter 
    this.availableTechnicians = this.users.filter(user => this.isUserAvailable(user));
    this.isManualAllocateModalOpen = true;
  }

  openAutoAllocateModal(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;
    // Filter
    const availableTechnicians = this.users.filter(user => this.isUserAvailable(user));
    this.autoAllocationCandidate = availableTechnicians.length > 0 ? availableTechnicians[0] : null;
    this.isAutoAllocateModalOpen = true;
  }

  closeAutoAllocateModal(): void {
    this.isAutoAllocateModalOpen = false;
    this.selectedAvaria = null;
    this.autoAllocationCandidate = null;
  }

  closeManualAllocateModal(): void {
    this.isManualAllocateModalOpen = false;
    this.selectedAvaria = null;
  }

  // fazer backend
  confirmAutoAllocation(): void {
    if (this.selectedAvaria && this.autoAllocationCandidate) {
      alert(`Alocando Avaria ID: ${this.selectedAvaria.id} a ${this.autoAllocationCandidate.name}`);
      this.closeAutoAllocateModal();
    }
  }

  //backend disto
  allocateToUser(user: User): void {
    if (this.selectedAvaria) {
      alert(`Alocando Avaria ID: ${this.selectedAvaria.id} a ${user.name}`);
      this.closeManualAllocateModal();
    }
  }

  isUserAvailable(user: User): boolean {
    // ver se é um técnico
    if (!user.role || user.role.toLowerCase() !== 'técnico') {
      return false;
    }

    const dayNames = ["Domingo", "Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "Sábado"];
    const today = new Date();
    const currentDay = dayNames[today.getDay()];

    // Check if today's day matches the user's day off
    if (user.dayOff && user.dayOff.toLowerCase() === currentDay.toLowerCase()) {
      return false;
    }

    // Check if today falls within the user's vacation period
    if (user.startHoliday && user.endHoliday) {
      const start = new Date(user.startHoliday);
      const end = new Date(user.endHoliday);
      if (today >= start && today <= end) {
        return false;
      }
    }

    return true;
  }


  criarAvaria() {
    if (!this.selectedPanelId) {
      alert('Por favor selecione um painel!');
      return;
    }

    const newAssistanceRequest: AssistanceRequestCreateModel = {
      requestDate: formatDate(new Date(), 'yyyy/MM/dd', 'en'),
      priority: this.selectedPriority,
      status: this.selectedStatus,
      statusClass: this.getStatusClass(this.selectedStatus),
      resolutionDate: "",
      description: this.newAvariaDescription,
      solarPanelId: this.selectedPanelId
    };

    this.aRService.create(newAssistanceRequest).subscribe(
      (result) => {
        alert("Novo pedido de assistência técnica criado com sucesso!");
        this.onCloseModal();
        this.loadAssistanceRequests();
      },
      (error) => {
        alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
        console.error(error);
      }
    );
  }

  editarAvaria() {
    if (this.selectedAvaria) {
      const id = this.selectedAvaria.id;
      const updatedAssistanceRequest: AssistanceRequestCreateModel = {
        requestDate: this.selectedAvaria.requestDate,
        priority: this.selectedPriority,
        status: this.selectedStatus,
        statusClass: this.getStatusClass(this.selectedStatus),
        resolutionDate: this.selectedAvaria.resolutionDate || "",
        description: this.selectedAvaria.description || "",
        solarPanelId: this.selectedPanelId
      };

      this.aRService.update(id, updatedAssistanceRequest).subscribe(
        (result) => {
          alert("Pedido de assistência técnica atualizado com sucesso!");
          this.onCloseEditModal();
          this.loadAssistanceRequests();
        },
        (error) => {
          alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          console.error(error);
        }
      );
    }
  }

  apagarAvaria() {
    if (this.selectedAvaria) {
      this.aRService.delete(this.selectedAvaria.id).subscribe(
        (result) => {
          alert("Pedido de assistência técnica removido com sucesso!");
          this.onCloseDeleteConfirm();
          this.loadAssistanceRequests();
        },
        (error) => {
          alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          console.error(error);
        }
      );
    }
  }
}
