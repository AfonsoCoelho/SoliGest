import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';

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

  constructor(private http: HttpClient) { }

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chatHub') // Atualize a URL se necessário
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Conexão com SignalR iniciada.'))
      .catch(err => console.error('Erro ao iniciar conexão SignalR:', err));
  }
  
  getConversations(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(`${this.baseUrl}/conversations`);
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
    return this.http.get<Contact[]>(`${this.baseUrl}/contacts`);
  }

  saveMessage(messageDto: ChatMessageDto): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/message`, messageDto);
  }
}
