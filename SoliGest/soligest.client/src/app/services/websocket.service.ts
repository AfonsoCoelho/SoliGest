import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WebsocketService {
  private socket!: WebSocket;
  public messages$ = new Subject<string>();

  connect(url: string): void {
    console.log('[WS] → connecting to', url);
    this.socket = new WebSocket(url);

    this.socket.onopen = () => console.log('[WS] ✅ open');
    this.socket.onmessage = (event) => {
      console.log('[WS] ← message', event.data);
      this.messages$.next(event.data);
    };
    this.socket.onerror = (err) => console.error('[WS] ❌ error', err);
    this.socket.onclose = (ev) => console.log('[WS] ❎ closed', ev);
  }

  send(message: any): void {
    if (this.socket.readyState === WebSocket.OPEN) {
      this.socket.send(JSON.stringify(message));
    }
  }
}
