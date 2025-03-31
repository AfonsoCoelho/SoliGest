import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

// Interface para tipagem dos painéis solares
interface SolarPanel {
  id: number;
  name: string;
  priority?: string;
  status: string;
  statusClass: string;
  latitude?: number;
  longitude?: number;
}

// Declaração do objeto google para TypeScript
declare var google: any;

@Component({
  selector: 'app-paineis-solares',
  templateUrl: './paineis-solares.component.html',
  styleUrls: ['./paineis-solares.component.css'],
  standalone: false
})
export class PaineisSolaresComponent implements OnInit {
  // Variáveis para ordenação
  sortBy: string = 'id';
  sortDirection: string = 'asc';

  // Dados dos painéis solares com coordenadas geográficas
  panelsData: SolarPanel[] = [
    {
      id: 1,
      name: "Rua D. Afonso Henriques, Lisboa",
      priority: "Alta",
      status: "Vermelho",
      statusClass: "status-red",
      latitude: 38.7223,
      longitude: -9.1393
    },
    {
      id: 4,
      name: "Avenida Principal, Almada",
      priority: "Média",
      status: "Verde",
      statusClass: "status-green",
      latitude: 38.6790,
      longitude: -9.1569
    },
    {
      id: 3,
      name: "Avenida da Liberdade, Porto",
      priority: "Baixa",
      status: "Amarelo",
      statusClass: "status-yellow",
      latitude: 41.1579,
      longitude: -8.6291
    }
  ];

  sortedPanels: SolarPanel[] = [];
  map: any;

  ngOnInit(): void {
    this.sortPanels();
    this.initMap();
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

    // Adiciona marcadores para cada painel
    this.addPanelMarkers();
  }

  // Adiciona marcadores no mapa para cada painel
  addPanelMarkers(): void {
    this.sortedPanels.forEach(panel => {
      if (panel.latitude && panel.longitude) {
        const marker = new google.maps.Marker({
          position: { lat: panel.latitude, lng: panel.longitude },
          map: this.map,
          title: `Painel ID: ${panel.id}`,
          icon: this.getMarkerIcon(panel.status)
        });

        // InfoWindow com detalhes do painel
        const infoWindow = new google.maps.InfoWindow({
          content: `
            <div style="padding: 10px;">
              <h3 style="margin: 0 0 5px 0;">Painel ID: ${panel.id}</h3>
              <p style="margin: 0 0 5px 0;"><strong>Localização:</strong> ${panel.name}</p>
              <p style="margin: 0 0 5px 0;"><strong>Prioridade:</strong> ${panel.priority || 'N/A'}</p>
              <p style="margin: 0; color: ${this.getStatusColor(panel.status)}; font-weight: bold;">
                <strong>Estado:</strong> ${panel.status}
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

  // Ordena os painéis conforme os critérios selecionados
  sortPanels(): void {
    this.sortedPanels = [...this.panelsData].sort((a, b) => {
      let valueA: any, valueB: any;

      switch (this.sortBy) {
        case 'priority':
          const priorityOrder: { [key: string]: number } = { 'Alta': 3, 'Média': 2, 'Baixa': 1 };
          valueA = a.priority ? priorityOrder[a.priority] : 0;
          valueB = b.priority ? priorityOrder[b.priority] : 0;
          break;
        case 'name':
          valueA = a.name.toLowerCase();
          valueB = b.name.toLowerCase();
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

    // Atualiza os marcadores no mapa após ordenação
    if (this.map) {
      this.clearMarkers();
      this.addPanelMarkers();
    }
  }

  // Limpa todos os marcadores do mapa (simplificado)
  clearMarkers(): void {
    // Na implementação real, você precisaria manter referência aos marcadores
    // Esta é uma versão simplificada que recarrega o mapa
    this.initMap();
  }
}
