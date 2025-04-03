import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AssistanceRequestsService, AssistanceRequest } from '../services/assistance-requests.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SolarPanel, SolarPanelsService } from '../services/solar-panels.service';

// Declaração do objeto google para TypeScript
declare var google: any;

@Component({
  selector: 'app-avarias',
  templateUrl: './avarias.component.html', // Updated template URL
  styleUrls: ['./avarias.component.css'],
  standalone: false
})
export class AvariasComponent implements OnInit {
  // Variáveis para ordenação
  sortBy: string = 'id';
  sortDirection: string = 'asc';
  searchTerm: string = ''; // Property to hold the search term
  public assistanceRequestsData: AssistanceRequest[] = [];
  filteredAvarias: AssistanceRequest[] = []; // Property to hold filtered avarias
  selectedAvaria: AssistanceRequest | null = null;
  selectedMarker: any = null; // Variável para armazenar o marcador selecionado
  infoWindow: any = null; // Variável global para o InfoWindow
  markers: any[] = []; // Armazena os marcadores no mapa

  showModal: boolean = false; // create popup
  showEditModal: boolean = false; // edit popup
  showDeleteConfirm: boolean = false; // delete popup

  selectedPanel: string = '';
  selectedPriority: string = '';
  selectedStatus: string = '';

  constructor(private http: HttpClient, private aRService: AssistanceRequestsService, private sPService: SolarPanelsService, private router: Router) { }

  //panels = ['Painel 1', 'Painel 2', 'Painel 3']; //quem for fazer backend que saque os paines da bd e meta no array
  panels: SolarPanel[] = [];

  // Dados das avarias com coordenadas geográficas
  avariasData: AssistanceRequest[] = [
    //{
    //  id: 1,
    //  name: "Rua D. Afonso Henriques, Lisboa",
    //  priority: "Alta",
    //  status: "Vermelho",
    //  statusClass: "status-red",
    //  latitude: 38.7223,
    //  longitude: -9.1393
    //},
    //{
    //  id: 4,
    //  name: "Avenida Principal, Almada",
    //  priority: "Média",
    //  status: "Vermelho",
    //  statusClass: "status-red",
    //  latitude: 38.6790,
    //  longitude: -9.1569
    //},
    //{
    //  id: 3,
    //  name: "Avenida da Liberdade, Porto",
    //  priority: "Baixa",
    //  status: "Amarelo",
    //  statusClass: "status-yellow",
    //  latitude: 41.1579,
    //  longitude: -8.6291
    //}
  ];

  sortedAvarias: AssistanceRequest[] = [];
  map: any;

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

  ngOnInit(): void {
    this.loadSolarPanels();
    this.loadAssistanceRequests();
  }

  // Inicializa o mapa do Google
  initMap(): void {
    // Centro do mapa (coordenadas de Portugal)
    const center = { lat: 39.3999, lng: -8.2245 };

    this.map = new google.maps.Map(document.getElementById("map"), {
      zoom: 7,
      center: center,
      mapTypeId: "terrain",
      styles: [
        {
          "featureType": "administrative",
          "elementType": "labels.text.fill",
          "stylers": [
            {
              "color": "#444444"
            }
          ]
        },
        {
          "featureType": "landscape",
          "elementType": "all",
          "stylers": [
            {
              "color": "#f2f2f2"
            }
          ]
        },
        {
          "featureType": "poi",
          "elementType": "all",
          "stylers": [
            {
              "visibility": "off"
            }
          ]
        },
        {
          "featureType": "road",
          "elementType": "all",
          "stylers": [
            {
              "saturation": -100
            },
            {
              "lightness": 45
            }
          ]
        },
        {
          "featureType": "road.highway",
          "elementType": "all",
          "stylers": [
            {
              "visibility": "simplified"
            }
          ]
        },
        {
          "featureType": "road.arterial",
          "elementType": "labels.icon",
          "stylers": [
            {
              "visibility": "off"
            }
          ]
        },
        {
          "featureType": "transit",
          "elementType": "all",
          "stylers": [
            {
              "visibility": "off"
            }
          ]
        },
        {
          "featureType": "water",
          "elementType": "all",
          "stylers": [
            {
              "color": "#46bcec"
            },
            {
              "visibility": "on"
            }
          ]
        }
      ]
    });

    // Adiciona marcadores para cada avaria
    this.addAvariaMarkers();
  }

  // Adiciona marcadores no mapa para cada avaria
  addAvariaMarkers(): void {
    this.sortedAvarias.forEach(avaria => {
      if (avaria.solarPanel.latitude && avaria.solarPanel.longitude) {
        const marker = new google.maps.Marker({
          position: { lat: avaria.solarPanel.latitude, lng: avaria.solarPanel.longitude },
          map: this.map,
          title: `Avaria ID: ${avaria.id}`,
          icon: this.getMarkerIcon(avaria.status)
        });

        // Adiciona o marcador à lista
        this.markers.push(marker);

        // InfoWindow com detalhes da avaria
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

        // Abre o InfoWindow ao clicar no marcador
        marker.addListener('click', () => {
          infoWindow.open(this.map, marker);
        });
      }
    });
  }

  // Retorna o ícone do marcador baseado no status
  getMarkerIcon(status: string): string {
    const iconBase = 'https://maps.google.com/mapfiles/ms/icons/';

    switch (status.toLowerCase()) {
      case 'vermelho':
        return iconBase + 'red-dot.png';
      case 'verde':
        return iconBase + 'green-dot.png';
      case 'amarelo':
        return iconBase + 'yellow-dot.png';
      default:
        return iconBase + 'blue-dot.png';
    }
  }

  // Retorna a cor do texto baseado no status
  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'vermelho':
        return '#ff0000';
      case 'verde':
        return '#00aa00';
      case 'amarelo':
        return '#ffaa00';
      default:
        return '#000000';
    }
  }

  // Ordena as avarias conforme os critérios selecionados
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

      if (valueA < valueB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (valueA > valueB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });

    // Filter the sorted avarias based on the current search term
    this.filterAvarias(); // Call to filter avarias based on the search term

    // Atualiza os marcadores no mapa após ordenação
    if (this.map) {
      this.clearMarkers();
      this.addAvariaMarkers();
    }
  }


  // Limpa todos os marcadores do mapa (simplificado)
  clearMarkers(): void {
    // Na implementação real, você precisaria manter referência aos marcadores
    // Esta é uma versão simplificada que recarrega o mapa
    this.initMap();
  }

  // Método para filtrar avarias com base no termo de pesquisa
  filterAvarias(): void {
    this.filteredAvarias = this.sortedAvarias.filter(
      avaria =>
        avaria.status.toLowerCase() !== 'verde' && // Remove status "Verde"
        avaria.solarPanel.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  selectAvaria(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;

    if (this.map && avaria.solarPanel.latitude && avaria.solarPanel.longitude) {
      // Centraliza e dá zoom no local da avaria
      this.map.setCenter({ lat: avaria.solarPanel.latitude, lng: avaria.solarPanel.longitude });
      this.map.setZoom(15);

      // Se já houver um InfoWindow aberto, fecha antes de abrir outro
      if (this.infoWindow) {
        this.infoWindow.close();
      }

      // Localiza o marcador correspondente
      const marker = this.markers.find(m => {
        const position = m.getPosition();
        return position?.lat() !== undefined && position?.lng() !== undefined &&
          Math.abs(position.lat() - avaria.solarPanel.latitude!) < 0.0001 &&
          Math.abs(position.lng() - avaria.solarPanel.longitude!) < 0.0001;
      });



      if (marker) {
        this.infoWindow = new google.maps.InfoWindow({
          content: `
          <div style="padding: 10px;">
            <h3 style="margin: 0 0 5px 0;">Avaria ID: ${avaria.id}</h3>
            <p style="margin: 0 0 5px 0;"><strong>Localização:</strong> ${avaria.solarPanel.name}</p>
            <p style="margin: 0 0 5px 0;"><strong>Prioridade:</strong> ${avaria.priority || 'N/A'}</p>
            <p style="margin: 0; color: ${this.getStatusColor(avaria.status)}; font-weight: bold;">
              <strong>Estado:</strong> ${avaria.status}
            </p>
            <div class="modal-buttons">
              <button (click)="autoAllocate(selectedAvaria)" class="auto-btn">Alocação Automática</button>
              <button (click)="manualAllocate(selectedAvaria)" class="manual-btn">Alocação Manual</button>
            </div>
          </div>
        `
        });

        // Abre o InfoWindow no marcador correspondente
        this.infoWindow.open(this.map, marker);
      }
    }
  }

  getAvariaClass(avaria: AssistanceRequest): any {
    return {
      [avaria.statusClass]: true,
      'selected': avaria === this.selectedAvaria
    };
  }


  // Simulação de alocação automática
  autoAllocate(avaria: AssistanceRequest): void {
    alert(`Avaria ID ${avaria.id} - Alocação automática iniciada!`);
  }

  // Simulação de alocação manual
  manualAllocate(avaria: AssistanceRequest): void {
    alert(`Avaria ID ${avaria.id} - Alocação manual iniciada!`);
  }

  openCreateModal(): void {
    this.selectedPanel = ''; // Reset selected values
    this.selectedPriority = ''; // Reset selected priority
    this.selectedStatus = ''; // Reset selected status
    this.showModal = true; // Show the modal
  }

  onCloseModal(): void {
    this.showModal = false; // Hide the modal//
  }

  openEditModal(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria;
    this.selectedPanel = ''; // You can set this to a specific panel if needed
    this.selectedPriority = avaria.priority || '';
    this.selectedStatus = avaria.status;
    this.showEditModal = true; // Show the edit modal
  }

  onCloseEditModal(): void {
    this.showEditModal = false; // Hide the edit modal
  }

  openDeleteConfirm(avaria: AssistanceRequest): void {
    this.selectedAvaria = avaria; // Set the selected avaria for confirmation
    this.showDeleteConfirm = true; // Show the delete confirmation popup
  }

  onCloseDeleteConfirm(): void {
    this.showDeleteConfirm = false; // Hide the delete confirmation popup
  }

  //quem for fazer o backend faça a logica
  criarAvaria() { };
  editarAvaria() { };
  apagarAvaria() { };
}
