import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { environment } from '@environments/environment';
import { AppConfig } from './app-config.model';
import { APP_CONFIG } from './app-config.token';

const config: AppConfig = {
  apiBaseUrl: environment.apiBaseUrl,
  tokenStorageKey: environment.tokenStorageKey,
  refreshTokenStorageKey: environment.refreshTokenStorageKey,
  themeStorageKey: environment.themeStorageKey
};

export function provideAppConfig(): EnvironmentProviders {
  return makeEnvironmentProviders([
    {
      provide: APP_CONFIG,
      useValue: config
    }
  ]);
}
