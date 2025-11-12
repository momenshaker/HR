import { bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors, withRequestsMadeViaParent } from '@angular/common/http';
import { provideRouter, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { AppComponent } from './app/app.component';
import { APP_ROUTES } from './app/app.routes';
import { appHttpInterceptors } from './app/core/interceptors';
import { provideAppConfig } from './app/core/config/app-config.provider';
import { provideErrorHandler } from './app/core/errors/error-handler.provider';

bootstrapApplication(AppComponent, {
  providers: [
    provideAppConfig(),
    provideAnimations(),
    provideAnimationsAsync(),
    provideRouter(APP_ROUTES, withComponentInputBinding(), withInMemoryScrolling({ anchorScrolling: 'enabled' })),
    provideHttpClient(withInterceptors(appHttpInterceptors), withRequestsMadeViaParent()),
    provideErrorHandler(),
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'outline'
      }
    }
  ]
}).catch((err) => console.error(err));
