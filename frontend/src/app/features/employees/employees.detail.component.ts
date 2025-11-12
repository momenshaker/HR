import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { EntityCrudFactory } from '@core/data-access';

interface EmployeeProfile {
  id: string;
  fullName: string;
  email: string;
  phone?: string;
  departmentName?: string;
  jobTitle?: string;
  documents?: Array<{ id: string; name: string; url: string }>;
  attendance?: Array<{ date: string; status: string }>;
  payslips?: Array<{ id: string; period: string; url: string }>;
}

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, MatTabsModule, MatListModule],
  templateUrl: './employees.detail.component.html',
  styleUrls: ['./employees.detail.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(EntityCrudFactory).create<never, never, EmployeeProfile>('employees');
  readonly loading = signal(true);
  readonly profile = signal<EmployeeProfile | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }
    this.service.getById(id).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
