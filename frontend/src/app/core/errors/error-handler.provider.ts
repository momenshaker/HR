import { ErrorHandler, inject, Injectable, Provider } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
class GlobalErrorHandler implements ErrorHandler {
  private readonly snackbar = inject(MatSnackBar);

  handleError(error: unknown): void {
    console.error(error);
    this.snackbar.open('An unexpected error occurred', 'Dismiss', {
      duration: 5000
    });
  }
}

export function provideErrorHandler(): Provider {
  return {
    provide: ErrorHandler,
    useClass: GlobalErrorHandler
  };
}
