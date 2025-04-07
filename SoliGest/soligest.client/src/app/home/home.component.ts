import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

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

  constructor(private auth: AuthorizeService, private router: Router) { }

  ngOnInit() {
    this.auth.onStateChanged().subscribe((state: boolean) => {
      this.isSignedIn = state;
    });
  }

  ngAfterViewInit(): void {
    this.initMap();
    this.loadSolarPanels();
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
    const panels = [
      { id: 1, lat: 38.736946, lng: -9.142685, status: 'operational' },
      { id: 2, lat: 38.726946, lng: -9.152685, status: 'maintenance' },
      { id: 3, lat: 38.746946, lng: -9.132685, status: 'faulty' }
    ];

    panels.forEach(panel => {
      const marker = new google.maps.Marker({
        position: new google.maps.LatLng(panel.lat, panel.lng),
        map: this.map,
        title: `Painel Solar #${panel.id}`,
        icon: this.getMarkerIcon(panel.status)
      });
      this.markers.push(marker);
    });

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
}
