import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AttendancePunchConfigurationRequest, AttendancePunchConfigurationService, PunchTypeOption } from './attendance-punch-configuration.service';

type PunchConfigGroup = FormGroup<{
  id: FormControl<string>;
  punchType: FormControl<string>;
  displayName: FormControl<string>;
  description: FormControl<string>;
  sortOrder: FormControl<number>;
  isActive: FormControl<boolean>;
}>;

@Component({
  selector: 'app-attendance-punch-configuration-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './attendance-punch-configuration-page.component.html',
  styleUrls: ['./attendance-punch-configuration-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AttendancePunchConfigurationPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);
  private readonly service = inject(AttendancePunchConfigurationService);

  readonly loading = signal(false);
  readonly savingIndex = signal<number | null>(null);
  readonly configForm = this.fb.group({
    configs: this.fb.array<PunchConfigGroup>([])
  });

  readonly configControls = computed(() => (this.configForm.controls.configs as FormArray));

  ngOnInit(): void {
    this.loadConfiguration();
  }

  get configs(): FormArray<PunchConfigGroup> {
    return this.configForm.controls.configs as FormArray<PunchConfigGroup>;
  }

  addConfiguration(): void {
    this.configs.push(this.createGroup());
  }

  saveConfiguration(index: number): void {
    const group = this.configs.at(index);
    if (!group.valid) {
      group.markAllAsTouched();
      return;
    }

    const request: AttendancePunchConfigurationRequest = {
      id: group.controls.id.value || undefined,
      punchType: group.controls.punchType.value?.trim() ?? '',
      displayName: group.controls.displayName.value?.trim() ?? '',
      description: group.controls.description.value?.trim() ?? '',
      sortOrder: group.controls.sortOrder.value ?? 0,
      isActive: group.controls.isActive.value
    };

    if (!request.punchType || !request.displayName) {
      this.snackBar.open('Punch type and display name are required.', 'Dismiss', { duration: 3000 });
      return;
    }

    this.savingIndex.set(index);
    this.service.savePunchType(request).subscribe({
      next: (result) => {
        group.patchValue({
          id: result.id,
          punchType: result.punchType,
          displayName: result.displayName,
          description: result.description,
          sortOrder: result.sortOrder,
          isActive: result.isActive
        });
        this.savingIndex.set(null);
        this.snackBar.open('Punch configuration saved.', 'Dismiss', { duration: 2500 });
      },
      error: () => {
        this.savingIndex.set(null);
        this.snackBar.open('Failed to save punch configuration.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  deleteConfiguration(index: number): void {
    this.configs.removeAt(index);
  }

  private loadConfiguration(): void {
    this.loading.set(true);
    this.service.getPunchTypes().subscribe({
      next: (options) => {
        this.loading.set(false);
        this.configs.clear();
        options.forEach((option) => this.configs.push(this.createGroup(option)));
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Unable to load punch configuration.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  private createGroup(option?: PunchTypeOption): PunchConfigGroup {
    return this.fb.group({
      id: [option?.id ?? ''],
      punchType: [option?.punchType ?? '', Validators.required],
      displayName: [option?.displayName ?? '', Validators.required],
      description: [option?.description ?? ''],
      sortOrder: [option?.sortOrder ?? 0, Validators.required],
      isActive: [option?.isActive ?? true]
    }) as PunchConfigGroup;
  }
}
