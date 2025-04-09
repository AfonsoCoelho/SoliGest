import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from '../../api-authorization/authorize.service';

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

  // Current chat
  public currentContact: any = null;
  public currentMessages: any[] = [];

  // Data
  public conversations = [
    {
      contact: { id: 1, name: 'António Mendes', position: 'Técnico' },
      lastMessage: 'BLA BLA BLA',
      lastMessageTime: new Date(Date.now() - 3600000),
      messages: [
        { content: 'BLA BLA BLA', sent: false, time: new Date(Date.now() - 3600000) },
        { content: 'BECA BECA BECA', sent: true, time: new Date(Date.now() - 1800000) },
        { content: 'ZIMBABUE', sent: false, time: new Date() }
      ],
      unreadCount: 0
    },
    {
      contact: { id: 2, name: 'João Silva', position: 'Gerente' },
      lastMessage: 'Precisamos falar sobre o projeto',
      lastMessageTime: new Date(Date.now() - 86400000),
      messages: [
        { content: 'Olá, como vai?', sent: false, time: new Date(Date.now() - 86400000) },
        { content: 'Precisamos falar sobre o projeto', sent: false, time: new Date(Date.now() - 82800000) }
      ],
      unreadCount: 2
    }
  ];

  public availableContacts = [
    { id: 3, name: 'Maria Santos', position: 'Supervisora' },
    { id: 4, name: 'Carlos Oliveira', position: 'Técnico' },
    { id: 5, name: 'Ana Pereira', position: 'Administradora' },
    { id: 6, name: 'Pedro Alves', position: 'Supervisora' },
    { id: 7, name: 'Sofia Martins', position: 'Técnica' }
  ];

  constructor(private auth: AuthorizeService, private router: Router) {
    this.auth.onStateChanged().subscribe((state: boolean) => {
      this.isSignedIn = state;
    });
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

  sortedConversations() {
    return [...this.conversations].sort((a, b) =>
      new Date(b.lastMessageTime).getTime() - new Date(a.lastMessageTime).getTime()
    );
  }

  filteredContacts() {
    if (!this.searchTerm) return this.availableContacts;
    const term = this.searchTerm.toLowerCase();
    return this.availableContacts.filter(contact =>
      contact.name.toLowerCase().includes(term) ||
      (contact.position && contact.position.toLowerCase().includes(term))
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

  startNewConversation(contact: any): void {
    const existingConversation = this.conversations.find(c => c.contact.id === contact.id);

    if (existingConversation) {
      this.selectConversation(existingConversation);
    } else {
      const newConversation = {
        contact: contact,
        lastMessage: '',
        lastMessageTime: new Date(),
        messages: [],
        unreadCount: 0
      };
      this.conversations.unshift(newConversation);
      this.selectConversation(newConversation);
    }

    this.closeModal();
  }

  sendMessage(): void {
    if (this.newMessage.trim() && this.currentContact) {
      const newMsg = {
        content: this.newMessage,
        sent: true,
        time: new Date()
      };

      this.currentMessages.push(newMsg);

      // Update conversation in list
      const conversation = this.conversations.find(c => c.contact.id === this.currentContact.id);
      if (conversation) {
        conversation.lastMessage = this.newMessage;
        conversation.lastMessageTime = new Date();
        conversation.messages = [...this.currentMessages];

        // Move to top
        this.conversations = [
          conversation,
          ...this.conversations.filter(c => c.contact.id !== this.currentContact.id)
        ];
      }

      this.newMessage = '';

      // Simulate reply after 1 second
      setTimeout(() => {
        const reply = {
          content: 'Resposta automática',
          sent: false,
          time: new Date()
        };
        this.currentMessages.push(reply);

        if (conversation) {
          conversation.lastMessage = reply.content;
          conversation.lastMessageTime = reply.time;
          conversation.messages = [...this.currentMessages];
        }
      }, 1000);
    }
  }
}
