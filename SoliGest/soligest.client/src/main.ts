import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { provideCharts } from 'ng2-charts';
import { BarController, Legend, Colors } from 'chart.js';

provideCharts({ registerables: [BarController, Legend, Colors] });

platformBrowserDynamic().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));
