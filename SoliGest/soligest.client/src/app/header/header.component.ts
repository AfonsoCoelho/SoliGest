import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { UserNotification, UserNotificationsService } from '../services/user-notifications.service';
import { User } from '../services/users.service';

interface Notification {
  id: number;
  title: string;
  message: string;
  type: 'message' | 'alert' | 'system' | 'warning';
  time: Date;
  read: boolean;
  link?: string;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  standalone:false
})
export class HeaderComponent implements OnInit{
  constructor(public router: Router, private authService: AuthorizeService, private un: UserNotificationsService) { }
    ngOnInit(): void {
      //this.getNotifications();
      //this.authService.user().subscribe(
      //  (result) => console.log(result)
      //)
      
      this.getNotifications();
    }
  showNotificationsPanel = false;
  showNotificationsPanel2 = false;
  showProfileMenu = false;
  profileImageUrl = 'profileIcon.png';

  notifications: Notification[] = [
    {
      id: 1,
      title: 'Nova mensagem no chat',
      message: 'Você recebeu uma nova mensagem de João',
      type: 'message',
      time: new Date(),
      read: false,
      link: '/chat'
    },
    {
      id: 2,
      title: 'Avaria reportada',
      message: 'O painel solar #45 apresentou falha',
      type: 'alert',
      time: new Date(Date.now() - 3600000),
      read: false,
      link: '/avarias/45'
    },
    {
      id: 3,
      title: 'Atualização disponível',
      message: 'Nova versão do sistema (v2.5.0)',
      type: 'system',
      time: new Date(Date.now() - 86400000),
      read: true
    }
  ];

  realNotifications: UserNotification[] = [];

  // Getter para notificações não lidas
  get unreadNotifications(): UserNotification[] {
    return this.realNotifications.filter(n => !n.isRead);
  }

  toggleNotifications(): void {
    this.showNotificationsPanel = !this.showNotificationsPanel;
  }

  toggleRealNotifications(): void {
    this.showNotificationsPanel2 = !this.showNotificationsPanel2;
  }

  toggleProfileMenu(): void {
    this.showProfileMenu = !this.showProfileMenu;
  }

  getNotifications(): void {    
    this.un.getByLoggedInUser().subscribe(
      (result: UserNotification[]) => this.realNotifications = result,
      (error: any) => console.error(error)
    );
  }

  getNotificationIcon(type: string): string {
    const icons: Record<string, string> = {
      'message': 'fas fa-comment-alt',
      'alert': 'fas fa-exclamation-circle',
      'system': 'fas fa-cog',
      'warning': 'fas fa-exclamation-triangle'
    };
    return icons[type] ?? 'fas fa-bell';
  }

  dismissNotification(id: number): void {
    this.un.delete(id).subscribe(
      (result) => {
        alert("Notificação apagada com sucesso!");
        this.getNotifications();
      },
      (error) => {
        alert("Ocorreu um erro. Por favor tente novamente mais tarde.");
        console.error(error);
      }
    );
  }

  markAllAsRead(): void {
    this.notifications = this.notifications.map(n => ({ ...n, read: true }));
  }

  openChat(): void {
    this.router.navigate(['/chat']); // ✅ Navega para o chat
  }

  logout(): void {
    this.router.navigate(['/login']);
  }
}
