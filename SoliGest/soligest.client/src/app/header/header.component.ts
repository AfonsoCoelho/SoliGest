import { Component } from '@angular/core';
import { Router } from '@angular/router';

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
export class HeaderComponent {
  constructor(public router: Router) { }
  showNotificationsPanel = false;
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

  // Getter para notificações não lidas
  get unreadNotifications(): Notification[] {
    return this.notifications.filter(n => !n.read);
  }

  toggleNotifications(): void {
    this.showNotificationsPanel = !this.showNotificationsPanel;
  }

  toggleProfileMenu(): void {
    this.showProfileMenu = !this.showProfileMenu;
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

  dismissNotification(id: number, event: Event): void {
    event.stopPropagation();
    this.notifications = this.notifications.filter(n => n.id !== id);
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
