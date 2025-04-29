import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WebsocketService } from '../services/websocket.service';
import { User, UsersService } from '../services/users.service';

export interface Message {
  from: string;
  text: string;
  sentAt: string;
}
export interface Contact { userId: string; displayName: string; }

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  standalone: false
})
export class ChatComponent implements OnInit {
  myId = localStorage.getItem('userId')!;
  contacts: Contact[] = [];     // preload via API or a static list
  selectedId = '';
  chatLog: Message[] = [];
  text = '';

  constructor(private us: UsersService,
    private ws: WebsocketService,
    private http: HttpClient
  ) { }

  ngOnInit() {
    // 1) connect websocket
    const proto = location.protocol === 'https:' ? 'wss' : 'ws';
    const url = `${proto}://${location.hostname}:${location.port}/ws`;

    this.ws.connect(url);

    this.ws.messages$.subscribe(raw => {
      const m = JSON.parse(raw) as Message;
      if (m.from === this.selectedId || m.from === this.myId) {
        this.chatLog.push(m);
      }
    });

    this.loadContacts();
    
  }

  private loadContacts(): void {
    this.us.getUsers().subscribe({
      next: (users: User[]) => {
        this.contacts = users.map(u => ({
          userId: u.id,
          displayName: u.name
        }));
      },
      error: err => console.error('Failed to load contacts', err)
    });
  }

  select(userId: string) {
    this.selectedId = userId;
    this.chatLog = [];
    // fetch history
    this.http
      .get<Message[]>(`/api/chat/history/${userId}`)
      .subscribe(history => this.chatLog = history);
  }

  send() {
    const t = this.text.trim();
    if (!t || !this.selectedId) return;
    const msg = { to: this.selectedId, text: t };
    this.ws.send(msg);
    this.text = '';
  }
}
