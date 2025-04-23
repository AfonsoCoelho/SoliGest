import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { User } from './users.service';

export interface Conversation {
  id?: number;
  contact: any;
  messages: any[];
  unreadCount?: number;
}

export interface Contact {
  id: string;
  name: string;
  position?: string;
}

export interface ChatMessageDto {
  receiverId: string;
  content: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private hubConnection: signalR.HubConnection | null = null;
  private baseUrl = '/api/chat';

  constructor(private http: HttpClient, private auth: AuthorizeService) { }

  public startConnection(): Promise<void> {
    if (this.hubConnection) {
      return Promise.resolve();
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chathub', { accessTokenFactory: () => this.auth.getToken() || '' })
      .withAutomaticReconnect()
      .build();

    return this.hubConnection
      .start()
      .then(() => console.log('SignalR conectado'))
      .catch(err => {
        console.error('Erro ao iniciar SignalR:', err);
        throw err;
      });
  }
  getAllUsersAsContacts(): Observable<Contact[]> {
       return this.http.get<User[]>('/api/Users', {
          headers: new HttpHeaders({ 'Authorization': `Bearer ${this.auth.getToken()}` })
       }).pipe(map(users => users.map(u => ({ id: u.id, name: u.name } as Contact))));
 }


  getConversationsFor(userId: string): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(`${this.baseUrl}/conversations:` + userId ,{
      headers: new HttpHeaders({ 'Authorization': `Bearer ${this.auth.getToken()}` })
    });
  }

  public sendMessage(receiverId: string, content: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/message`, { receiverId, content }, {
      headers: new HttpHeaders({ 'Authorization': `Bearer ${this.auth.getToken()}` })
    });
  }

  getHubConnection(): signalR.HubConnection | null {
    if (!this.hubConnection) {
      throw new Error('Hub connection not started.');
    }

    return this.hubConnection;
  }

  onMessageReceived(callback: (sender: string, message: string, timestamp: string) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('ReceiveMessage', (sender, message, timestamp) => {
        callback(sender, message, timestamp);
      });
    }
  }

  getAvailableContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${this.baseUrl}/contacts`, {
      headers: new HttpHeaders({ 'Authorization': `Bearer ${this.auth.getToken()}` })
    });
  }
}
