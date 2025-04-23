import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { ChatService, Conversation } from '../services/chat.service';
import { UsersService } from '../services/users.service';

interface Contact {
  id: string;
  name: string;
}

interface MessageDto {
  id: number;
  senderId: string;
  receiverId: string;
  content: string;
  timestamp: string;
}

interface ConversationDto {
  id: number;
  contact: Contact;
  messages: MessageDto[];
}

// UI interfaces
export interface UiMessage {
  sent: boolean;
  content: string;
  time: Date;
}
export interface UiConversation {
  id: number;
  contact: Contact;
  messages: UiMessage[];
  lastMessage: UiMessage;
  unreadCount: number;
}

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  standalone: false
})
export class ChatComponent implements OnInit {
  // Menu state
  public isMenuCollapsed = false;
  public isSignedIn = false;

  // Chat state
  public showNewChatModal = false;
  public searchTerm = '';
  public newMessage = '';

  public currentUser = '';
  public currentUserId = '';
  public currentContact: Contact | null = null;
  public currentMessages: UiMessage[] = [];
  public conversations: UiConversation[] = [];
  public contacts: Contact[] = [];

  public hubConnection: signalR.HubConnection | null = null;

  constructor(
    private auth: AuthorizeService,
    private router: Router,
    private chatService: ChatService,
    private us: UsersService
  ) {

    this.auth.onStateChanged().subscribe(state => {
      this.isSignedIn = state;
      if (state) {
        //this.auth.getUserInfo().subscribe(userInfo => {
        //  this.currentUser = userInfo.name;
        //  this.currentUserId = userInfo.id;
        //});
        this.currentUserId = localStorage.getItem('loggedUserId') ?? '';
        this.us.getUser(this.currentUserId).subscribe(
          (result) => this.currentUser = result.name,
          (error) => console.error(error)
          )
      };
    });
  }

  ngOnInit(): void {
    this.auth.onStateChanged()
      .pipe(filter(signedIn => signedIn))
      .subscribe(() => {
        this.auth.getUserInfo().subscribe(userInfo => {
          this.currentUser = userInfo.name;
          this.currentUserId = userInfo.id;

          this.loadConversations();

        });
      });
  }


  private loadConversations(): void {
    this.hubConnection = this.chatService.getHubConnection();

    this.chatService.getConversations()
      .subscribe((dtos) => {
        this.conversations = dtos.map(dto => {
          const msgs = dto.messages.map(m => ({
            content: m.content,
            sent: m.senderId === this.currentUserId,
            time: new Date(m.timestamp)
          }));
          return {
            id: dto.id,
            contact: dto.contact,
            messages: msgs,
            lastMessage: msgs[msgs.length - 1],
            unreadCount: 0
          } as UiConversation;
        });
      });

    this.chatService.getAvailableContacts()
      .subscribe(cts => this.contacts = cts);
  }


  sendMessage(): void {
    if (!this.currentContact || !this.newMessage.trim()) return;

    this.chatService.sendMessage(this.currentContact.id, this.newMessage);


    const uiMsg: UiMessage = {
      content: this.newMessage,
      sent: true,
      time: new Date()
    };
    const conv = this.conversations.find(c => c.contact.id === this.currentContact!.id);
    if (conv) {
      conv.messages.push(uiMsg);
      conv.lastMessage = uiMsg;
    }

    this.newMessage = '';
  }

  onMessageReceived(callback: (sender: string, message: any) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('ReceiveMessage', (sender: string, message: any) => {
        callback(sender, message);
      });
    }
  }

  startNewConversation(contact: Contact): void {
    let conv = this.conversations.find(c => c.contact.id === contact.id);
    if (!conv) {
      conv = { id: 0, contact, messages: [], lastMessage: { content: "", sent: false ,time: new Date()  }, unreadCount: 0 };
      this.conversations.push(conv);
    }

    this.currentContact = contact;
    this.currentMessages = conv.messages;
    this.closeModal();
  }

  // Utility methods
  getInitials(name: string): string {
    return name ? name.split(' ').map(n => n[0]).join('').toUpperCase() : '';
  }

  truncate(text: string, maxLength: number): string {
    if (!text) return '';
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
  }

  formatTime(date: Date): string {
    return date ? date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : '';
  }

  filteredContacts(): Contact[] {
    return this.contacts.filter(c => c.name.toLowerCase().includes(this.searchTerm.toLowerCase()));
  }

  sortedConversations(): UiConversation[] {
    return this.conversations.sort((a, b) => {
      const aTime = a.messages.length ? a.messages[a.messages.length - 1].time.getTime() : 0;
      const bTime = b.messages.length ? b.messages[b.messages.length - 1].time.getTime() : 0;
      return bTime - aTime;
    });
  }

  // Menu methods
  toggleMenu(): void {
    this.isMenuCollapsed = !this.isMenuCollapsed;
  }

  signOut(): void {
    if (this.isSignedIn) {
      this.auth.signOut();
      this.router.navigateByUrl('');
    } else {
      this.router.navigateByUrl('login');
    }
  }

  openNewChatModal(): void {
    this.showNewChatModal = true;
  }

  closeModal(): void {
    this.showNewChatModal = false;
    this.searchTerm = '';
  }

  selectConversation(conv: UiConversation): void {
    this.currentContact = conv.contact;
    this.currentMessages = conv.messages;
    conv.unreadCount = 0;
  }
}
