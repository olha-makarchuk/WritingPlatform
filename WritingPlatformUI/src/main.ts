import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module'; // Замість app.config, ми імпортуємо AppModule

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch((err) => console.error(err));
