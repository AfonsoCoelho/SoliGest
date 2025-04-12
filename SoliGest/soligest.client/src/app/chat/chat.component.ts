import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { ChatService } from '../services/chat.service';

interface Conversation {
  id?: number;
  contact: Contact;
  messages: any[];
  unreadCount?: number;
}

interface Contact {
  id: string;
  name: string;
  position?: string;
}

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  standalone: false
})
export class ChatComponent {
  // Menu state
  public isMenuCollapsed: boolean = false;
  public isSignedIn: boolean = false;

  // Chat state
  public showNewChatModal: boolean = false;
  public searchTerm: string = '';
  public newMessage: string = '';

  
  public currentUser: string = '';
  public currentContact: Contact | null = null;
  public currentMessages: any[] = [];
  public conversations: Conversation[] = [];
  public contacts: Contact[] = [];

  constructor(private auth: AuthorizeService, private router: Router, private chatService: ChatService) {
    this.auth.onStateChanged().subscribe((state: boolean) => {
      this.isSignedIn = state;

      if (this.isSignedIn) {
        this.auth.getUserInfo().subscribe((userInfo) => {
          this.currentUser = userInfo.name;
        });
      }
    });
  }


  ngOnInit(): void {
    this.chatService.startConnection();

    this.chatService.onMessageReceived((user: string, message: any) => {
      // Procura a conversa que possua o contato remetente
      let conversation = this.conversations.find(conv => conv.contact.id === user);
      if (conversation) {
        conversation.messages.push({
          content: message,
          sent: false,
          time: new Date()
        });
        conversation.unreadCount = (conversation.unreadCount || 0) + 1;
        
        if (this.currentContact && this.currentContact.id === conversation.contact.id) {
          this.currentMessages = conversation.messages;
          conversation.unreadCount = 0;
        }
      } else {
        let newConversation: Conversation = {
          contact: { id: user, name: user },
          messages: [{
            content: message,
            sent: false,
            time: new Date()
          }],
          unreadCount: 1
        };
        this.conversations.push(newConversation);
      }
    });

    this.chatService.getConversations().subscribe((data: any[]) => {
      this.conversations = data.map(conv => {
        return {
          contact: conv.contact,
          messages: conv.messages,
          unreadCount: 0
        } as Conversation;
      });
    });

    this.chatService.getAvailableContacts().subscribe((data: any[]) => {
      this.contacts = data;
    });
  }

  

  sendMessage(): void {
    if (this.newMessage.trim() && this.currentContact) {
      const sender = this.currentUser || 'Utilizador';
      // Envia via SignalR
      this.chatService.sendMessage(sender, this.newMessage);

      const messageDto = {
        receiverId: this.currentContact.id,
        content: this.newMessage
      };

      this.chatService.saveMessage(messageDto).subscribe();

      let conversation = this.conversations.find(conv => conv.contact.id === this.currentContact!.id);
      if (!conversation) {
        conversation = {
          contact: this.currentContact,
          messages: [],
          unreadCount: 0
        };
        this.conversations.push(conversation);
      }
      conversation.messages.push({ content: this.newMessage, sent: true, time: new Date() });
      this.currentMessages = conversation.messages;
      this.newMessage = '';
    }
  }

  startNewConversation(contact: Contact): void {
    // Verifica se já existe uma conversa para o contato selecionado
    let conversation = this.conversations.find(conv => conv.contact.id === contact.id);
    if (!conversation) {
      conversation = {
        contact: contact,
        messages: [],
        unreadCount: 0
      };
      this.conversations.push(conversation);
    }
    this.currentContact = contact;
    this.currentMessages = conversation.messages;
    this.closeModal();
  }

  // Utility methods
  getInitials(name: string): string {
    if (!name) return '';
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  }

  truncate(text: string, maxLength: number): string {
    if (!text) return '';
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
  }

  formatTime(date: Date): string {
    if (!date) return '';
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }


  filteredContacts(): Contact[] {
    return this.contacts.filter(contact =>
      contact.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  // Menu methods
  toggleMenu(): void {
    this.isMenuCollapsed = !this.isMenuCollapsed;
  }

  signOut() {
    if (this.isSignedIn) {
      this.auth.signOut();
      this.router.navigateByUrl('');
    } else {
      this.router.navigateByUrl('login');
    }
  }

  // Chat methods
  openNewChatModal(): void {
    this.showNewChatModal = true;
  }

  closeModal(): void {
    this.showNewChatModal = false;
    this.searchTerm = '';
  }

  selectConversation(conversation: any): void {
    this.currentContact = conversation.contact;
    this.currentMessages = conversation.messages;
    conversation.unreadCount = 0;
  }
}
