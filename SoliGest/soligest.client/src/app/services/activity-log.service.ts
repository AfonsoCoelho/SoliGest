import { Injectable } from '@angular/core';

export interface ActivityLogEntry {
  timestamp: string;
  action: string;
}

@Injectable({
  providedIn: 'root',
})
export class ActivityLogService {
  private readonly STORAGE_KEY = 'user_activity_log';

  constructor() { }

  logAction(action: string): void {
    const log: ActivityLogEntry[] = this.getLogs();
    log.push({
      timestamp: new Date().toISOString(),
      action,
    });
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(log));
  }

  getLogs(): ActivityLogEntry[] {
    const data = localStorage.getItem(this.STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  }

  clearLogs(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  }
}
