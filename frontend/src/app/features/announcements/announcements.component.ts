import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EntityCrudFactory } from '@core/data-access';
import { AuthStore } from '@core/auth/auth.store';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';

interface AnnouncementDto {
  id: string;
  title: string;
  message: string;
  audience: string;
  createdBy: string;
  publishedAtUtc: string;
  requiresAcknowledgement: boolean;
}

interface CreateAnnouncementRequest {
  title: string;
  message: string;
  audience: string;
  createdBy: string;
  requiresAcknowledgement: boolean;
}

@Component({
  selector: 'app-announcements-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    ConfirmationDialogComponent
  ],
  templateUrl: './announcements.component.html',
  styleUrls: ['./announcements.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnnouncementsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackbar = inject(MatSnackBar);
  private readonly authStore = inject(AuthStore);
  private readonly dialog = inject(MatDialog);
  private readonly service = inject(EntityCrudFactory).create<
    CreateAnnouncementRequest,
    CreateAnnouncementRequest,
    AnnouncementDto
  >('announcements');

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    audience: ['All Employees', [Validators.maxLength(100)]],
    message: ['', [Validators.required, Validators.maxLength(5000)]],
    requiresAcknowledgement: [false]
  });

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly deletingId = signal<string | null>(null);
  readonly announcements = signal<ReadonlyArray<AnnouncementDto>>([]);

  readonly publishDisabled = computed(() => this.form.invalid || this.saving());

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  refresh(): void {
    this.loadAnnouncements();
  }

  publish(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const createdBy = this.authStore.userId();
    if (!createdBy) {
      this.snackbar.open('Unable to identify current user. Please re-authenticate.', 'Dismiss', { duration: 4000 });
      return;
    }

    const payload: CreateAnnouncementRequest = {
      ...this.form.getRawValue(),
      createdBy
    };

    this.saving.set(true);
    this.service.create(payload).subscribe({
      next: () => {
        this.snackbar.open('Announcement published', 'Dismiss', { duration: 3000 });
        this.form.reset({
          title: '',
          audience: 'All Employees',
          message: '',
          requiresAcknowledgement: false
        });
        this.loadAnnouncements();
      },
      error: () => this.saving.set(false)
    });
  }

  confirmDelete(announcement: AnnouncementDto): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete announcement',
          message: `Are you sure you want to delete "${announcement.title}"?`,
          confirmLabel: 'Delete'
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }
        this.deleteAnnouncement(announcement.id);
      });
  }

  private deleteAnnouncement(id: string): void {
    this.deletingId.set(id);
    this.service.delete(id).subscribe({
      next: () => {
        this.snackbar.open('Announcement deleted', 'Dismiss', { duration: 3000 });
        this.announcements.set(this.announcements().filter((item) => item.id !== id));
        this.deletingId.set(null);
      },
      error: () => this.deletingId.set(null)
    });
  }

  private loadAnnouncements(): void {
    this.loading.set(true);
    this.service.list({ page: 1, pageSize: 25 }).subscribe({
      next: (response) => {
        const items = response.items ?? [];
        this.announcements.set(
          [...items].sort(
            (a, b) => new Date(b.publishedAtUtc).getTime() - new Date(a.publishedAtUtc).getTime()
          )
        );
        this.loading.set(false);
        this.saving.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.saving.set(false);
      }
    });
  }

}
