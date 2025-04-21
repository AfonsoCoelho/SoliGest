import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { SolarPanelsService } from '../services/solar-panels.service';
import { UsersService } from '../services/users.service';

declare var google: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  standalone: false
})
export class HomeComponent implements OnInit, AfterViewInit {
  public isSignedIn: boolean = false;
  public isMenuCollapsed: boolean = false;
  private map: any;
  private markers: any[] = [];
  public showSolarPanels: boolean = true;
  public showTechnicians: boolean = true;

  constructor(private auth: AuthorizeService, private router: Router, private sPService: SolarPanelsService, private uService: UsersService) { }

  ngOnInit() {
    this.auth.onStateChanged().subscribe((state: boolean) => {
      this.isSignedIn = state;
    });
  }

  ngAfterViewInit(): void {
    this.initMap();
    this.filterMap();
  }

  toggleMenu(): void {
    this.isMenuCollapsed = !this.isMenuCollapsed;
    setTimeout(() => {
      if (this.map) {
        google.maps.event.trigger(this.map, 'resize');
        this.adjustMapView();
      }
    }, 300);
  }

  signOut() {
    if (this.isSignedIn) {
      this.auth.signOut();
      this.router.navigateByUrl('');
      alert("Adeus!");
      this.router.navigateByUrl('login');
    } else {
      this.router.navigateByUrl('login');
    }
  }

  private initMap(): void {
    const mapOptions = {
      center: new google.maps.LatLng(38.736946, -9.142685),
      zoom: 12,
      mapTypeId: google.maps.MapTypeId.ROADMAP,
      styles: [
        {
          featureType: "poi",
          elementType: "labels",
          stylers: [{ visibility: "off" }]
        }
      ]
    };

    this.map = new google.maps.Map(document.getElementById('map'), mapOptions);
  }

  private loadSolarPanels(): void {
    var panels;

    this.sPService.getSolarPanels().subscribe(
      (result) => {
        panels = result;
        //this.sortPanels();
        //this.addPanelMarkers(); // Adiciona os markers APÓS carregar e ordenar os painéis
        panels.forEach(panel => {
          const marker = new google.maps.Marker({
            position: new google.maps.LatLng(panel.latitude, panel.longitude),
            map: this.map,
            title: `Painel Solar #${panel.id}`,
            icon: this.getMarkerIcon(panel.status)
          });
          this.markers.push(marker);

          const infoWindow = new google.maps.InfoWindow({
            content: `
                      <div style="padding: 10px;">
                        <h3 style="margin: 0 0 5px 0;">Painel Solar ID: ${panel.id}</h3>
                        <p style="margin: 0 0 5px 0;"><strong>Localização:</strong> ${panel.name}</p>
                      </div>
                    `
          });

          marker.addListener('click', () => {
            infoWindow.open(this.map, marker);
          });
        });
      },
      (error) => {
        console.error(error);
      }
    );   

    this.adjustMapView();
  }

  private loadTechnicians(): void {
    var users;

    this.uService.getUsers().subscribe(
      (result) => {
        users = result;
        users.forEach(user => {
          if (user.role === "Técnico" && user.latitude && user.longitude) {
            const marker = new google.maps.Marker({
              position: new google.maps.LatLng(user.latitude, user.longitude),
              map: this.map,
              title: `Técnico #${user.id}`,
              icon: this.getMarkerIcon('faulty')
            });
            this.markers.push(marker);

            const infoWindow = new google.maps.InfoWindow({
              content: `
                        <div style="padding: 10px;">
                          <h3 style="margin: 0 0 5px 0;">Técnico ID: ${user.id}</h3>
                          <p style="margin: 0 0 5px 0;"><strong>Nome:</strong> ${user.name}</p>
                          <p style="margin: 0 0 5px 0;"><strong>Contacto:</strong> ${user.phoneNumber || 'N/A'}</p>
                          </p>
                        </div>
                      `
            });

            marker.addListener('click', () => {
              infoWindow.open(this.map, marker);
            });
          }
        });
      },
      (error) => {
        console.error(error);
      }
    );

    this.adjustMapView();
  }

  private adjustMapView(): void {
    const bounds = new google.maps.LatLngBounds();
    this.markers.forEach(marker => bounds.extend(marker.getPosition()));
    if (!bounds.isEmpty()) {
      this.map.fitBounds(bounds);
    } else {
      this.map.setCenter(new google.maps.LatLng(38.736946, -9.142685));
      this.map.setZoom(12);
    }
  }

  private getMarkerIcon(status: string): any {
    const baseUrl = 'https://maps.google.com/mapfiles/ms/icons/';
    switch (status) {
      case 'operational': return { url: baseUrl + 'green-dot.png' };
      case 'maintenance': return { url: baseUrl + 'yellow-dot.png' };
      case 'faulty': return { url: baseUrl + 'red-dot.png' };
      default: return { url: baseUrl + 'blue-dot.png' };
    }
  }

  filterMap(): void {
    this.resetMarkers();
    this.initMap();
    if (this.showSolarPanels) {
      this.loadSolarPanels();
    }
    if (this.showTechnicians) {
      this.loadTechnicians();
    }
    this.adjustMapView();
  }

  resetMarkers(): void {
    this.markers = [];
  }
}
