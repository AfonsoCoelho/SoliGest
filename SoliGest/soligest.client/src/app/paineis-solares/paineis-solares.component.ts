import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SolarPanel, SolarPanelsService } from '../services/solar-panels.service';

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
  searchTerm: string = '';
  popupMessage: string = '';
  popupType: 'success' | 'error' = 'success';

  public panelsData: SolarPanel[] = [];
  sortedPanels: SolarPanel[] = [];
  filteredPanels: SolarPanel[] = [];
  markers: any[] = [];
  infoWindow: any = null;
  selectedPanel: SolarPanel | null = null;

  map: any;
  timerInterval!: any;
  timerWidth = 100;

  showCreateModal: boolean = false;
  showEditModal: boolean = false;
  showDeleteModal: boolean = false;
  popupVisible: boolean = false;

  newPanel: SolarPanel = this.createEmptyPanel();
  editingPanel: SolarPanel = this.createEmptyPanel();

  panelToDelete: number | null = null;


  constructor(private http: HttpClient, private service: SolarPanelsService, private router: Router) { }

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
      phoneNumber: 0,
      email: ''
    };
  }

  ngOnInit(): void {
    this.loadPanels();
  }

  loadPanels(): void {
    this.service.getSolarPanels().subscribe(
      (result) => {
        this.panelsData = result;
        this.filterPanels();
      },
      (error) => {
        console.error(error);
      }
    );

    this.initMap();
  }

  initMap(): void {
    const center = { lat: 39.3999, lng: -8.2245 };

    this.map = new google.maps.Map(document.getElementById("map"), {
      zoom: 7,
      center: center,
      mapTypeId: "terrain"
    });
  }

  addPanelMarkers(): void {
    this.clearMarkers();
    this.markers = [];

    this.filteredPanels.forEach(panel => {
      if (panel.latitude && panel.longitude) {
        const marker = new google.maps.Marker({
          position: { lat: panel.latitude, lng: panel.longitude },
          map: this.map,
          title: `Painel ID: ${panel.id}`,
          icon: this.getMarkerIcon(panel.status)
        });

        this.markers.push(marker);

        const infoWindowContent = `
          <div style="padding: 10px;">
            <h3 style="margin: 0 0 5px 0;">Painel ID: ${panel.id}</h3>
            <p style="margin: 0 0 5px 0;"><strong>Nome:</strong> ${panel.name}</p>
            <p style="margin: 0 0 5px 0;"><strong>Estado:</strong> ${panel.status}</p>
          </div>
        `;

        const infoWindow = new google.maps.InfoWindow({
          content: infoWindowContent
        });

        marker.addListener('click', () => {
          if (this.infoWindow) {
            this.infoWindow.close();
          }
          infoWindow.open(this.map, marker);
          this.infoWindow = infoWindow;
          this.selectedPanel = panel;
        });
      }
    });
  }

  filterPanels(): void {
    if (!this.searchTerm) {
      this.filteredPanels = [...this.panelsData];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredPanels = this.panelsData.filter(panel =>
        panel.name.toLowerCase().includes(term) ||
        (panel.address && panel.address.toLowerCase().includes(term)) ||
        (panel.description && panel.description.toLowerCase().includes(term))
      );
    }
    this.sortPanels();
  }

  clearMarkers(): void {
    this.markers.forEach(marker => {
      marker.setMap(null);
    });
  }

  flyToPanel(panel: SolarPanel): void {
    if (panel.latitude && panel.longitude && this.map) {
      this.map.setCenter({ lat: panel.latitude, lng: panel.longitude });
      this.map.setZoom(15);

      const marker = this.markers.find(m =>
        m.getPosition().lat() === panel.latitude &&
        m.getPosition().lng() === panel.longitude
      );

      if (marker && this.infoWindow) {
        this.infoWindow.close();
        const infoWindowContent = `
          <div style="padding: 10px;">
            <h2>Detalhes do Painel ID: ${panel.id}</h2>
            <p><strong>Nome/Localização:</strong> ${panel.name ?? 'N/A'}</p>
            <p><strong>Prioridade:</strong> ${panel.priority ?? 'N/A'}</p>
            <p><strong>Estado:</strong> ${panel.status ?? 'N/A'}</p>
            <p><strong>Descrição:</strong> ${panel.description ?? 'N/A'}</p>
            <p><strong>Telefone:</strong> ${panel.phoneNumber ?? 'N/A'}</p>
            <p><strong>Email:</strong> ${panel.email ?? 'N/A'}</p>
          </div>
        `;
        this.infoWindow.setContent(infoWindowContent);
        this.infoWindow.open(this.map, marker);
      } else if (marker) {
        const infoWindowContent = `
          <div style="padding: 10px;">
            <h2>Detalhes do Painel ID: ${panel.id}</h2>
            <p><strong>Nome/Localização:</strong> ${panel.name ?? 'N/A'}</p>
            <p><strong>Prioridade:</strong> ${panel.priority ?? 'N/A'}</p>
            <p><strong>Estado:</strong> ${panel.status ?? 'N/A'}</p>
            <p><strong>Descrição:</strong> ${panel.description ?? 'N/A'}</p>
            <p><strong>Telefone:</strong> ${panel.phoneNumber ?? 'N/A'}</p>
            <p><strong>Email:</strong> ${panel.email ?? 'N/A'}</p>
          </div>
        `;
        this.infoWindow = new google.maps.InfoWindow({ content: infoWindowContent });
        this.infoWindow.open(this.map, marker);
      }
      this.selectedPanel = panel;
    }
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
      //alert('O nome do painel é obrigatório!');
      this.showPopup('error', `O nome do painel é obrigatório!`);
      return;
    }

    const id = this.editingPanel.id;
    const name = this.editingPanel.name;
    const priority = this.editingPanel.priority;
    const status = this.editingPanel.status;
    const statusClass = this.getStatusClass(status);
    const latitude = this.editingPanel.latitude;
    const longitude = this.editingPanel.longitude;
    const description = this.editingPanel.description;
    const phone = this.editingPanel.phoneNumber;
    const email = this.editingPanel.email;
    const address = "";

    if (id) {
      this.service.updateSolarPanel(id, name, priority, status, statusClass, latitude, longitude, description, phone, email, address).subscribe(
        (result) => {
          //alert("Painel solar atualizado com sucesso!");
          this.showPopup('success', `Painel solar atualizado com sucesso!`);
          this.closeEditPanelModal();
          this.loadPanels();
        },
        (error) => {
          //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          this.showPopup('error', `Ocorreu um erro. Por favor tente novamente mais tarde.`);
          console.error(error);
        }
      );
    }
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
      //alert('O nome do painel é obrigatório!');
      this.showPopup('error', `O nome do painel é obrigatório!`);
      return;
    }

    const solarPanel: SolarPanel = {
      id: 0,
      name: this.newPanel.name,
      priority: this.newPanel.priority,
      status: this.newPanel.status,
      statusClass: this.getStatusClass(this.newPanel.status),
      latitude: this.newPanel.latitude,
      longitude: this.newPanel.longitude,
      description: this.newPanel.description,
      phoneNumber: this.newPanel.phoneNumber,
      email: this.newPanel.email,
      address: "a"
    };

    this.service.createSolarPanel(solarPanel).subscribe(
      (result) => {
        //alert("Novo painel solar criado com sucesso!");
        this.showPopup('success', `Novo painel solar criado com sucesso!`);
        this.closeCreatePanelModal();
        this.loadPanels();
      },
      (error) => {
        //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
        this.showPopup('error', `Ocorreu um erro. Por favor tente novamente mais tarde.`);
        console.error(error);
      }
    );
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
    if (this.panelToDelete) {
      this.service.deleteSolarPanel(this.panelToDelete).subscribe(
        (result) => {
          //alert("Painel solar removido com sucesso!");
          this.showPopup('success', `Painel solar removido com sucesso!`);
          this.cancelDelete();
          this.loadPanels();
        },
        (error) => {
          //alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
          this.showPopup('error', `Ocorreu um erro. Por favor tente novamente mais tarde.`);
          console.error(error);
        }
      );
    }
  }

  sortPanels(): void {
    this.sortedPanels = [...this.filteredPanels].sort((a, b) => {
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

  selectPanel(panel: SolarPanel): void {
    this.flyToPanel(panel);
  }

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
  }
}
