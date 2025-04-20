import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { HubConnectionState } from '@microsoft/signalr';

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

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: signalR.HubConnection | null = null;
  private baseUrl = '/api/Chat';

  constructor(private http: HttpClient, private auth: AuthorizeService) { }

  public startConnection(): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chathub', {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
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
  
  getConversations(): Observable<Conversation[]> {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<Conversation[]>(`${this.baseUrl}/conversations`);
  }

  public sendMessage(receiverId: string, content: string): void {
    if (this.hubConnection!.state !== HubConnectionState.Connected) {
      console.warn('Tentativa de enviar sem ligação SignalR estabelecida');
      return;
    }
    this.hubConnection!.invoke('SendMessage', receiverId, content)
      .catch(err => console.error('Erro ao enviar via SignalR:', err));
  }

  onMessageReceived(callback: (sender: string, message: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('ReceiveMessage', (sender: string, message: any) => {
        callback(sender, message);
      });
    }
  }

  getAvailableContacts(): Observable<Contact[]> {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<Contact[]>(`${this.baseUrl}/contacts`);
  }

  saveMessage(messageDto: ChatMessageDto): Observable<any> {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post<any>(`${this.baseUrl}/message`, messageDto);
  }


}
