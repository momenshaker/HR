import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-breadcrumbs',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <nav class="breadcrumbs" aria-label="Breadcrumb">
      <ng-container *ngFor="let crumb of items; let last = last">
        <span class="breadcrumbs__item" [class.breadcrumbs__item--active]="last">{{ crumb }}</span>
        <mat-icon *ngIf="!last" class="breadcrumbs__separator">chevron_right</mat-icon>
      </ng-container>
    </nav>
  `,
  styles: [
    `
      .breadcrumbs {
        display: flex;
        align-items: center;
        gap: 0.25rem;
        font-size: 0.875rem;
        color: rgba(0, 0, 0, 0.6);
      }
      .dark-theme .breadcrumbs {
        color: rgba(255, 255, 255, 0.7);
      }
      .breadcrumbs__item--active {
        font-weight: 600;
        color: inherit;
      }
      .breadcrumbs__separator {
        font-size: 1rem;
      }
    `
  ]
})
export class BreadcrumbsComponent {
  @Input() items: ReadonlyArray<string> = [];
}
