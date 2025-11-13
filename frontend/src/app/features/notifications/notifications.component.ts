import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EntityCrudFactory } from '@core/data-access';

interface NotificationItem {
  id: string;
  title: string;
  body: string;
  createdAt: string;
  audience: string;
}

@Component({
  selector: 'app-notifications-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatListModule,
    MatIconModule
  ],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotificationsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly service = inject(EntityCrudFactory).create<any, any, NotificationItem>('notifications');

  readonly form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    body: ['', Validators.required],
    audience: ['All', Validators.required]
  });

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<NotificationItem>>([]);

  constructor() {
    this.load();
  }

  refresh(): void {
    this.load();
  }

  send(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.service.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.snackbar.open('Notification sent', 'Dismiss', { duration: 3000 });
        this.form.reset({ title: '', body: '', audience: 'All' });
        this.load();
      },
      error: () => this.loading.set(false)
    });
  }

  private load(): void {
    this.loading.set(true);
    this.service.list({ page: 1, pageSize: 20 }).subscribe({
      next: (response) => {
        this.items.set(response.data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
