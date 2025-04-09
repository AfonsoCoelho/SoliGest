import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'app-about-us',
  standalone: false,
  
  templateUrl: './about-us.component.html',
  styleUrl: './about-us.component.css'
})
export class AboutUsComponent {
  public isSignedIn: boolean = false;
  public isMenuCollapsed: boolean = false;
  constructor(private auth: AuthorizeService, private router: Router) { }

  toggleMenu(): void {
    this.isMenuCollapsed = !this.isMenuCollapsed;
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
}
