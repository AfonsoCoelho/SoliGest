import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { ChatService, Conversation } from '../services/chat.service';

// DTO interfaces (define here since chat.models.ts doesn't exist)
interface Contact {
  id: string;
  name: string;
}

interface MessageDto {
  id: number;
  senderId: string;
  receiverId: string;
  content: string;
  timestamp: string; // from server
}

interface ConversationDto {
  id: number;
  contact: Contact;
  messages: MessageDto[];
}

// UI interfaces
export interface UiMessage {
  content: string;
  sent: boolean;
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

  constructor(
    private auth: AuthorizeService,
    private router: Router,
    private chatService: ChatService
  ) {

    this.auth.onStateChanged().subscribe(state => {
      this.isSignedIn = state;
      if (state) {
        this.auth.getUserInfo().subscribe(userInfo => {
          this.currentUser = userInfo.name;
          this.currentUserId = userInfo.id;
        });
      }
    });
  }

  ngOnInit(): void {

    this.auth.onStateChanged()
      .pipe(filter(signedIn => signedIn))
      .subscribe(() => {

        this.chatService.onMessageReceived((senderId: string, message: string) => {
          const conv = this.conversations.find(c => c.contact.id === senderId);
          const uiMsg: UiMessage = {
            content: message,
            sent: false,
            time: new Date()
          };

          if (conv) {
            conv.messages.push(uiMsg);
            conv.lastMessage = uiMsg;
            conv.unreadCount++;
            if (this.currentContact?.id === senderId) {
              this.currentMessages = conv.messages;
              conv.unreadCount = 0;
            }
          } else {
            this.conversations.push({
              id: 0,
              contact: { id: senderId, name: senderId },
              messages: [uiMsg],
              lastMessage: uiMsg,
              unreadCount: 1
            });
          }

        });

        this.chatService.getConversations()
          .subscribe((dtos: Conversation[]) => {
            this.conversations = dtos.map(dto => {
              const msgs: UiMessage[] = dto.messages.map(m => ({
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

        // Load contacts
        this.chatService.getAvailableContacts()
          .subscribe(data => this.contacts = data);
      });
  }

  sendMessage(): void {
    if (!this.currentContact || !this.newMessage.trim()) return;

    this.chatService.sendMessage(this.currentContact.id, this.newMessage);

    this.chatService.saveMessage({
      receiverId: this.currentContact.id,
      content: this.newMessage
    }).subscribe();

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
