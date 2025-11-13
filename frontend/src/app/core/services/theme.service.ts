import { inject, Injectable, signal } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AppConfig } from '../config/app-config.model';
import { APP_CONFIG } from '../config/app-config.token';

type ThemeMode = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly themeSignal = signal<ThemeMode>(this.loadTheme());

  readonly theme = this.themeSignal.asReadonly();

  toggle(): void {
    const next = this.themeSignal() === 'light' ? 'dark' : 'light';
    this.setTheme(next);
  }

  private setTheme(theme: ThemeMode): void {
    const body = this.document.body;
    body.classList.toggle('dark-theme', theme === 'dark');
    this.themeSignal.set(theme);
    localStorage.setItem(this.config.themeStorageKey, theme);
  }

  private loadTheme(): ThemeMode {
    const stored = localStorage.getItem(this.config.themeStorageKey) as ThemeMode | null;
    if (stored) {
      requestAnimationFrame(() => this.setTheme(stored));
      return stored;
    }
    return 'light';
  }
}
