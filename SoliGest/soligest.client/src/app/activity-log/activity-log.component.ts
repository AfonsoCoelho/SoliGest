import { Component, OnInit } from '@angular/core';
import { ActivityLogService, ActivityLogEntry } from '../services/activity-log.service';

@Component({
  selector: 'app-activity-log',
  templateUrl: './activity-log.component.html',
  styleUrls: ['./activity-log.component.css'],
  standalone: false
})
export class ActivityLogComponent implements OnInit {
  logs: ActivityLogEntry[] = [];

  constructor(private activityService: ActivityLogService) { }

  ngOnInit(): void {
    this.logs = this.activityService.getLogs();
  }

  clearLogs(): void {
    this.activityService.clearLogs();
    this.logs = [];
  }
}
