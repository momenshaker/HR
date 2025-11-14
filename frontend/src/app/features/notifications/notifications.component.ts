import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EntityCrudFactory } from '@core/data-access';
import { AuthStore } from '@core/auth/auth.store';

interface EngagementCampaignDto {
  id: string;
  name: string;
  description: string;
  channels: string;
  targetAudience: string;
  launchDateUtc: string;
  endDateUtc?: string | null;
  isAutomated: boolean;
}

interface CreateEngagementCampaignRequest {
  name: string;
  description: string;
  channels: string;
  targetAudience: string;
  launchDateUtc: string;
  endDateUtc?: string | null;
  isAutomated: boolean;
  ownerId: string;
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
    MatIconModule,
    MatCheckboxModule
  ],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NotificationsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly authStore = inject(AuthStore);
  private readonly service = inject(EntityCrudFactory).create<
    CreateEngagementCampaignRequest,
    CreateEngagementCampaignRequest,
    EngagementCampaignDto
  >('EngagementCampaigns');

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    channels: ['Email,In-app'],
    targetAudience: ['All Employees', Validators.required],
    launchDateUtc: [this.toLocalInputValue(new Date()), Validators.required],
    endDateUtc: [''],
    isAutomated: [false]
  });

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<EngagementCampaignDto>>([]);

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

    const ownerId = this.authStore.userId();
    if (!ownerId) {
      this.snackbar.open('Unable to determine campaign owner. Please re-authenticate.', 'Dismiss', {
        duration: 4000
      });
      return;
    }

    this.loading.set(true);
    const payload: CreateEngagementCampaignRequest = {
      ...this.form.getRawValue(),
      launchDateUtc: this.toUtc(this.form.controls.launchDateUtc.value),
      endDateUtc: this.form.controls.endDateUtc.value
        ? this.toUtc(this.form.controls.endDateUtc.value)
        : null,
      ownerId
    };

    this.service.create(payload).subscribe({
      next: () => {
        this.snackbar.open('Engagement campaign scheduled', 'Dismiss', { duration: 3000 });
        this.form.reset({
          name: '',
          description: '',
          channels: 'Email,In-app',
          targetAudience: 'All Employees',
          launchDateUtc: this.toLocalInputValue(new Date()),
          endDateUtc: '',
          isAutomated: false
        });
        this.load();
      },
      error: () => this.loading.set(false)
    });
  }

  private load(): void {
    this.loading.set(true);
    this.service.list({ page: 1, pageSize: 20 }).subscribe({
      next: (response) => {
        const sorted = [...response.data].sort(
          (a, b) => new Date(b.launchDateUtc).getTime() - new Date(a.launchDateUtc).getTime()
        );
        this.items.set(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private toLocalInputValue(date: Date): string {
    const pad = (value: number) => value.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(
      date.getMinutes()
    )}`;
  }

  private toUtc(value: string): string {
    const parsed = new Date(value);
    return new Date(
      Date.UTC(
        parsed.getFullYear(),
        parsed.getMonth(),
        parsed.getDate(),
        parsed.getHours(),
        parsed.getMinutes(),
        parsed.getSeconds()
      )
    ).toISOString();
  }
}
