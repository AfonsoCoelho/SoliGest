import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

@Component({
  selector: 'app-home',
  standalone: false,
  
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  public isSignedIn: boolean = false;

  constructor(private auth: AuthorizeService, private router: Router) { }

  ngOnInit() {
    this.auth.onStateChanged().subscribe((state: boolean) => {
      this.isSignedIn = state;
    });
  }

  signOut() {
    if (this.isSignedIn) {
      this.auth.signOut();
      this.router.navigateByUrl('');
      alert("Adeus!");
    }
    else {
      this.router.navigateByUrl('login');
    }
  }
}
