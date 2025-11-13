import { inject, Injectable } from '@angular/core';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';
import { AuthTokens } from '../auth/auth.models';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly storage = window.localStorage;

  get accessToken(): string | null {
    return this.storage.getItem(this.config.tokenStorageKey);
  }

  get refreshToken(): string | null {
    return this.storage.getItem(this.config.refreshTokenStorageKey);
  }

  get tokens(): AuthTokens | null {
    const accessToken = this.accessToken;
    const refreshToken = this.refreshToken;
    if (!accessToken || !refreshToken) {
      return null;
    }

    return {
      accessToken,
      refreshToken,
      expiresIn: Number(this.storage.getItem(`${this.config.tokenStorageKey}:expiresIn`) ?? 0)
    };
  }

  save(tokens: AuthTokens): void {
    this.storage.setItem(this.config.tokenStorageKey, tokens.accessToken);
    this.storage.setItem(this.config.refreshTokenStorageKey, tokens.refreshToken);
    this.storage.setItem(`${this.config.tokenStorageKey}:expiresIn`, tokens.expiresIn.toString());
  }

  clear(): void {
    this.storage.removeItem(this.config.tokenStorageKey);
    this.storage.removeItem(this.config.refreshTokenStorageKey);
    this.storage.removeItem(`${this.config.tokenStorageKey}:expiresIn`);
  }
}
