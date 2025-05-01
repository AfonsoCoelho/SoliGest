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
  selectedTechnicianId: number | null = null; // Altere aqui para number
  popupType: 'success' | 'error' = 'success';
  timerInterval!: any;
  timerWidth = 100;

  // Modais
  showModal: boolean = false;
  showEditModal: boolean = false;
  showDeleteConfirm: boolean = false;
  isManualAllocateModalOpen: boolean = false;
  isAutoAllocateModalOpen: boolean = false;
  popupVisible: boolean = false;

  // Formulários
  selectedPanelId: number = 0;
  selectedPriority: string = '';
  selectedStatus: string = '';
  newAvariaDescription: string = ''; // Adicionado para o formulário de criação
  popupMessage: string = '';


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

  showPopup(type: 'success' | 'error', message: string) {
    // inicializa
    this.popupType = type;
    this.popupMessage = message;
    this.timerWidth = 100;
    this.popupVisible = true;

    // limpa qualquer intervalo anterior
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }

    // diminui 2% a cada 100ms → 50 ciclos → 5s total
    this.timerInterval = setInterval(() => {
      this.timerWidth -= 2;
      if (this.timerWidth <= 0) {
        this.closePopup();
      }
    }, 100);
  }

  closePopup() {
    this.popupVisible = false;
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }

    if (this.popupType == "success") {
      window.location.reload(); //por agora, ja arranjo depois
    }
  }

  openCreateModal(): void {
    this.selectedPanelId = 0;
    this.selectedPriority = '';
    this.selectedStatus = '';
    this.newAvariaDescription = '';
    this.availableTechnicians = this.users.filter(user => this.isUserAvailable(user));
    this.showModal = true;
  }

  onCloseModal(): void {
    this.showModal = false;
  }

  openEditModal(avaria: any): void {
    this.selectedAvaria = avaria;
    this.selectedPanelId = avaria.solarPanel?.id;
    this.selectedPriority = avaria.priority;
    this.selectedStatus = avaria.status;
    this.selectedTechnicianId = avaria.assignedUser?.id ?? null; //pode aparecer n/a se o tecnico alocado tiver no dia de folga mas n da erro, a opcao default fica n/a
    this.availableTechnicians = this.users.filter(user => this.isUserAvailable(user));
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
    //this.initMap();
  }

  loadAssistanceRequests(): void {
    const loggedUserId = localStorage.getItem('loggedUserId');
    if (!loggedUserId) {
      // No user logged-in: clear everything
      this.avariasData = [];
      this.sortedAvarias = [];
      this.filteredAvarias = [];
      this.clearMarkers();
      return;
    }

    // 1) Get the user (to check role)
    this.uService.getUser(loggedUserId).subscribe(
      (user) => {
        // 2) Pull all requests
        this.aRService.getAll().subscribe(
          (all) => {
            // 3) Role-based filter
            if (user.role === 'Administrativo' || user.role === 'Supervisor') {
              this.avariasData = all;
            } else {
              this.avariasData = all.filter(a =>
                a.assignedUser?.id.toString() === loggedUserId
              );
            }

            // 4) Sort & in-memory filter
            this.sortAvarias();
            this.filterAvarias();

            // 5) Clear old markers & InfoWindow, then redraw
            this.clearMarkers();
            this.addAvariaMarkers();

            // 6) (Optional) reset map view to default
            this.map.setCenter({ lat: 39.3999, lng: -8.2245 });
            this.map.setZoom(7);
          },
          (err) => console.error('Error loading avarias:', err)
        );
      },
      (err) => {
        console.error('Error fetching user:', err);
        // Fallback: show only assigned
        this.aRService.getAll().subscribe(
          (all) => {
            this.avariasData = all.filter(a =>
              a.assignedUser?.id.toString() === loggedUserId
            );
            this.sortAvarias();
            this.filterAvarias();
            this.clearMarkers();
            this.addAvariaMarkers();
            this.map.setCenter({ lat: 39.3999, lng: -8.2245 });
            this.map.setZoom(7);
          },
          (error) => console.error('Error loading avarias:', error)
        );
      }
    );
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
    this.loadUsers();
    this.loadAssistanceRequests();
  }

  ngAfterViewInit(): void {
    this.initMap();              
    this.loadSolarPanels();      
    this.loadUsers();            
    this.loadAssistanceRequests();
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
            <p><strong>Técnico:</strong> ${avaria.assignedUser?.name || 'Não atribuído'}</p>
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
    this.markers.forEach(m => m.setMap(null));
    this.markers = [];
    if (this.infoWindow) {
      this.infoWindow.close();
      this.infoWindow = null;
    }
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
            <p><strong>Técnico alocado:</strong> ${avaria.assignedUser?.name || 'Não atribuído'}</p>
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

  // backend auto alocacao
  confirmAutoAllocation(): void {
    if (this.selectedAvaria && this.autoAllocationCandidate) {
      // Build the update model based on the selected assistance request data.
      const updatedAssistanceRequest: AssistanceRequestCreateModel = {
        requestDate: this.selectedAvaria.requestDate,
        resolutionDate: this.selectedAvaria.resolutionDate,
        description: this.selectedAvaria.description,
        solarPanelId: this.selectedAvaria.solarPanel.id,
        priority: this.selectedAvaria.priority ?? '',
        status: this.selectedAvaria.status ?? '',
        statusClass: this.selectedAvaria.statusClass ?? '',
        assignedUserId: this.autoAllocationCandidate.id
      };

      // Call the update endpoint with the auto allocation candidate's id.
      this.aRService.update(this.selectedAvaria.id, updatedAssistanceRequest).subscribe(
        (result) => {
          //alert(`Avaria ID: ${this.selectedAvaria!.id} atribuída a ${this.autoAllocationCandidate!.name} com sucesso.`);
          this.showPopup('success', `Avaria ID: ${this.selectedAvaria!.id} atribuída a ${this.autoAllocationCandidate!.name} com sucesso.`);
          this.closeAutoAllocateModal();
          this.loadAssistanceRequests();
          setTimeout(() => {
            window.location.reload();
          }, 5000);
        },
        (error) => {
          //alert("Ocorreu um erro ao alocar automaticamente o funcionário.");
          this.showPopup('error', 'Ocorreu um erro ao alocar automaticamente o funcionário.');
          console.error(error);
        }
      );
    }
  }


  //backend disto este é o manual alocacao
  allocateToUser(user: User): void {
    if (this.selectedAvaria) {
      // Build the update model based on the existing assistance request data,
      // adding the new assignedUserId.
      const updatedAssistanceRequest: AssistanceRequestCreateModel = {
        requestDate: this.selectedAvaria.requestDate,
        resolutionDate: this.selectedAvaria.resolutionDate,
        description: this.selectedAvaria.description,
        solarPanelId: this.selectedAvaria.solarPanel.id,
        priority: this.selectedAvaria.priority ?? '',
        status: this.selectedAvaria.status,
        statusClass: this.selectedAvaria.statusClass,
        assignedUserId: user.id  // Now using a string-based id
      };

      this.aRService.update(this.selectedAvaria.id, updatedAssistanceRequest).subscribe(
        (result) => {
          //alert(`Avaria ID: ${this.selectedAvaria!.id} atribuída a ${user.name} com sucesso.`);
          this.showPopup('success', `Avaria ID: ${this.selectedAvaria!.id} atribuída a ${user.name} com sucesso.`);
          this.closeManualAllocateModal();
          this.loadAssistanceRequests();
          setTimeout(() => {
            window.location.reload();
          }, 5000);
        },
        (error) => {
          //alert("Ocorreu um erro ao atribuir o técnico.");
          this.showPopup('error', `Ocorreu um erro ao atribuir o técnico.`);
          console.error(error);
        }
      );
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
      //alert('Por favor selecione um painel!');
      this.showPopup('error', 'Por favor selecione um painel!');
      return;
    }

    const newAssistanceRequest: AssistanceRequestCreateModel = {
      requestDate: formatDate(new Date(), 'yyyy/MM/dd', 'en'),
      priority: this.selectedPriority,
      status: this.selectedStatus,
      statusClass: this.getStatusClass(this.selectedStatus),
      resolutionDate: "",
      description: this.newAvariaDescription,
      solarPanelId: this.selectedPanelId,
      assignedUserId: this.selectedTechnicianId ? this.selectedTechnicianId.toString() : undefined // Convert to string if not null

    };

    this.aRService.create(newAssistanceRequest).subscribe(
      (result) => {
        //alert("Novo pedido de assistência técnica criado com sucesso!");
        this.showPopup('success', 'Novo pedido de assistência técnica criado com sucesso!');
        this.onCloseModal();
        this.loadAssistanceRequests();
        setTimeout(() => {
          window.location.reload();
        }, 5000);
      },
      (error) => {
        //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
        this.showPopup('error', 'Ocorreu um erro. Por favor tente novamente mais tarde.');
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
        solarPanelId: this.selectedPanelId,
        assignedUserId: this.selectedTechnicianId != null ? this.selectedTechnicianId.toString() : undefined

      };

      this.aRService.update(id, updatedAssistanceRequest).subscribe(
        (result) => {
          //alert("Pedido de assistência técnica atualizado com sucesso!");
          this.showPopup('success', `Pedido de assistência técnica atualizado com sucesso!`);
          this.onCloseEditModal();
          this.loadAssistanceRequests();
          setTimeout(() => {
            window.location.reload();
          }, 5000);
        },
        (error) => {
          //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          this.showPopup('error', `Ocorreu um erro. Por favor tente novamente mais tarde.`);
          console.error(error);
        }
      );
    }
  }

  apagarAvaria() {
    if (this.selectedAvaria) {
      this.aRService.delete(this.selectedAvaria.id).subscribe(
        (result) => {
          //alert("Pedido de assistência técnica removido com sucesso!");
          this.showPopup('success', `Pedido de assistência técnica removido com sucesso!`);
          this.onCloseDeleteConfirm();
          this.loadAssistanceRequests();
        },
        (error) => {
          //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          this.showPopup('error', `Ocorreu um erro. Por favor tente novamente mais tarde.`);
          console.error(error);
        }
      );
    }
  }
}
