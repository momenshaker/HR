import { computed, inject, Injectable, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthTokens, AuthUser, UserRole } from './auth.models';

interface AuthState {
  user: AuthUser | null;
  tokens: AuthTokens | null;
  loading: boolean;
  lastError?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly snackbar = inject(MatSnackBar);
  private readonly state = signal<AuthState>({ user: null, tokens: null, loading: false, lastError: null });

  readonly user = computed(() => this.state().user);
  readonly userId = computed(() => this.state().user?.id ?? null);
  readonly roles = computed<ReadonlyArray<UserRole>>(() => this.state().user?.roles ?? []);
  readonly tokens = computed(() => this.state().tokens);
  readonly loading = computed(() => this.state().loading);
  readonly isAuthenticated = computed(() => Boolean(this.state().tokens?.accessToken));

  setLoading(isLoading: boolean): void {
    this.patch({ loading: isLoading });
  }

  setTokens(tokens: AuthTokens | null): void {
    this.patch({ tokens });
  }

  setUser(user: AuthUser | null): void {
    this.patch({ user });
  }

  setError(error: string | null, notify = false): void {
    this.patch({ lastError: error });
    if (notify && error) {
      this.snackbar.open(error, 'Dismiss', { duration: 5000 });
    }
  }

  reset(): void {
    this.state.set({ user: null, tokens: null, loading: false, lastError: null });
  }

  private patch(partial: Partial<AuthState>): void {
    this.state.update((current) => ({ ...current, ...partial }));
  }
}
