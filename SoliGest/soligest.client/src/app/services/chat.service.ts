import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { AuthorizeService } from '../../api-authorization/authorize.service';

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

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://127.0.0.1:49893/chatHub', {
        accessTokenFactory: () => this.auth.getToken()
      } as signalR.IHttpConnectionOptions) 
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Conexão com SignalR iniciada.'))
      .catch(err => console.error('Erro ao iniciar conexão SignalR:', err));
  }
  
  getConversations(): Observable<Conversation[]> {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<Conversation[]>(`${this.baseUrl}/conversations`, { headers });
  }

  sendMessage(sender: string, content: string): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('SendMessage', sender, content)
        .catch(err => console.error('Erro ao enviar a mensagem via SignalR:', err));
    }
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

    return this.http.get<Contact[]>(`${this.baseUrl}/contacts`, { headers });
  }

  saveMessage(messageDto: ChatMessageDto): Observable<any> {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.post<any>(`${this.baseUrl}/message`, messageDto, { headers });
  }


}
