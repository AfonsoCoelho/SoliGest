import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface SolarPanel {
  id: number;
  name: string;
  priority?: string;
  status: string;
  statusClass: string;
  latitude?: number;
  longitude?: number;
  description?: string;
  phone?: string;
  email?: string;
}

declare var google: any;

@Component({
  selector: 'app-paineis-solares',
  templateUrl: './paineis-solares.component.html',
  styleUrls: ['./paineis-solares.component.css'],
  standalone: false
})
export class PaineisSolaresComponent implements OnInit {
  sortBy: string = 'id';
  sortDirection: string = 'asc';

  panelsData: SolarPanel[] = [];
  sortedPanels: SolarPanel[] = [];

  map: any;

  showCreateModal: boolean = false;
  showEditModal: boolean = false;
  showDeleteModal: boolean = false;
  showViewModal: boolean = false;

  newPanel: SolarPanel = this.createEmptyPanel();
  editingPanel: SolarPanel = this.createEmptyPanel();
  viewingPanel: SolarPanel | null = null;
  panelToDelete: number | null = null;

  constructor(private http: HttpClient) { }

  private createEmptyPanel(): SolarPanel {
    return {
      id: 0,
      name: '',
      priority: 'Média',
      status: 'Verde',
      statusClass: 'status-green',
      latitude: undefined,
      longitude: undefined,
      description: '',
      phone: '',
      email: ''
    };
  }

  ngOnInit(): void {
    this.loadPanels();
  }

  loadPanels(): void {
    this.panelsData = [
      {
        id: 1,
        name: "Rua D. Afonso Henriques, Lisboa",
        priority: "Alta",
        status: "Vermelho",
        statusClass: "status-red",
        latitude: 38.7223,
        longitude: -9.1393,
        description: "Painel próximo ao centro",
        phone: "912345678",
        email: "contato@empresa.com"
      },
      {
        id: 4,
        name: "Avenida Principal, Almada",
        priority: "Média",
        status: "Verde",
        statusClass: "status-green",
        latitude: 38.6790,
        longitude: -9.1569,
        description: "Painel na filial sul",
        phone: "987654321",
        email: "suporte@empresa.com"
      }
    ];

    this.sortPanels();
    this.initMap();
  }

  initMap(): void {
    const center = { lat: 39.3999, lng: -8.2245 };

    this.map = new google.maps.Map(document.getElementById("map"), {
      zoom: 7,
      center: center,
      mapTypeId: "terrain"
    });

    this.addPanelMarkers();
  }

  addPanelMarkers(): void {
    this.sortedPanels.forEach(panel => {
      if (panel.latitude && panel.longitude) {
        new google.maps.Marker({
          position: { lat: panel.latitude, lng: panel.longitude },
          map: this.map,
          title: `Painel ID: ${panel.id}`,
          icon: this.getMarkerIcon(panel.status)
        });
      }
    });
  }

  openViewPanelModal(panel: SolarPanel): void {
    this.viewingPanel = panel;
    this.showViewModal = true;
  }

  closeViewPanelModal(): void {
    this.viewingPanel = null;
    this.showViewModal = false;
  }

  openEditPanelModal(panel: SolarPanel): void {
    this.editingPanel = { ...panel };
    this.showEditModal = true;
  }

  closeEditPanelModal(): void {
    this.editingPanel = this.createEmptyPanel();
    this.showEditModal = false;
  }

  updatePanel(): void {
    if (!this.editingPanel.name.trim()) {
      alert('O nome do painel é obrigatório!');
      return;
    }

    const index = this.panelsData.findIndex(p => p.id === this.editingPanel.id);
    if (index !== -1) {
      this.panelsData[index] = { ...this.editingPanel };
      this.sortPanels();
    }
    this.closeEditPanelModal();
  }

  openCreatePanelModal(): void {
    this.newPanel = this.createEmptyPanel();
    this.showCreateModal = true;
  }

  closeCreatePanelModal(): void {
    this.showCreateModal = false;
  }

  createPanel(): void {
    if (!this.newPanel.name.trim()) {
      alert('O nome do painel é obrigatório!');
      return;
    }

    const newId = this.panelsData.length > 0 ? Math.max(...this.panelsData.map(p => p.id)) + 1 : 1;
    const newPanelEntry: SolarPanel = {
      ...this.newPanel,
      id: newId,
      statusClass: this.getStatusClass(this.newPanel.status)
    };

    this.panelsData.push(newPanelEntry);
    this.sortPanels();
    this.closeCreatePanelModal();
  }

  confirmDeletePanel(panelId: number): void {
    this.panelToDelete = panelId;
    this.showDeleteModal = true;
  }

  cancelDelete(): void {
    this.panelToDelete = null;
    this.showDeleteModal = false;
  }

  deletePanel(): void {
    if (this.panelToDelete !== null) {
      this.panelsData = this.panelsData.filter(p => p.id !== this.panelToDelete);
      this.sortPanels();
    }
    this.cancelDelete();
  }

  sortPanels(): void {
    this.sortedPanels = [...this.panelsData].sort((a, b) => {
      let valueA: any, valueB: any;

      switch (this.sortBy) {
        case 'priority':
          const priorityOrder: { [key: string]: number } = { 'Alta': 3, 'Média': 2, 'Baixa': 1 };
          valueA = priorityOrder[a.priority || 'Média'];
          valueB = priorityOrder[b.priority || 'Média'];
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
          valueA = statusOrder[a.status];
          valueB = statusOrder[b.status];
          break;
        default:
          valueA = a.id;
          valueB = b.id;
      }

      return this.sortDirection === 'asc' ? valueA - valueB : valueB - valueA;
    });

    if (this.map) {
      this.addPanelMarkers();
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

  getMarkerIcon(status: string): string {
    const iconBase = 'https://maps.google.com/mapfiles/ms/icons/';
    switch (status.toLowerCase()) {
      case 'vermelho': return iconBase + 'red-dot.png';
      case 'verde': return iconBase + 'green-dot.png';
      case 'amarelo': return iconBase + 'yellow-dot.png';
      default: return iconBase + 'blue-dot.png';
    }
  }
}
