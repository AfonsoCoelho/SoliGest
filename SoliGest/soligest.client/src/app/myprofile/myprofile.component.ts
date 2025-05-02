import { Component, OnInit } from '@angular/core';
import { User, UsersService } from '../services/users.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-myprofile',
  standalone: false,
  templateUrl: './myprofile.component.html',
  styleUrl: './myprofile.component.css'
})
export class MyprofileComponent implements OnInit {
  user: User | null = null;
  userRole: string | null = null;


  constructor(private uService: UsersService) { }
  ngOnInit() {
    const loggedUserId = localStorage.getItem('loggedUserId');
    if (loggedUserId) {
      this.uService.getUser(loggedUserId).subscribe(
        (user) => {
          this.user = user;
          this.userRole = user.role;
        },
        (err) => {
          console.error('Error fetching user in funcionarios:', err);
        }
      );
    }
  }
}

