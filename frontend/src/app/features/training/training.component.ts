import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

interface TrainingCourseDto {
  id: string;
  title: string;
  category: string;
  description: string;
  instructor: string;
  startDate: string;
  endDate: string;
  capacity: number;
  deliveryMode: string;
  competencyCodes: string[];
  skillLevel: string;
  offersCertification: boolean;
  certificationCriteria: string;
  durationHours: number;
}

interface CourseEnrollmentDto {
  id: string;
  courseId: string;
  employeeId: string;
  enrolledOn: string;
  status: string;
  completionPercentage: number;
  completedOn?: string | null;
  certificationId?: string | null;
}

interface CourseProgressAnalyticsDto {
  courseId: string;
  totalEnrollments: number;
  activeEnrollments: number;
  completedEnrollments: number;
  averageCompletionPercentage: number;
  generatedOnUtc: string;
}

interface LiteCourseDto {
  id: string;
  organizationId: string;
  code: string;
  title: string;
  description?: string | null;
  durationHours: number;
  isMandatory: boolean;
}

interface LiteCourseSessionDto {
  id: string;
  courseId: string;
  startUtc: string;
  endUtc: string;
  location?: string | null;
  meetingUrl?: string | null;
  capacity?: number | null;
}

@Component({
  selector: 'app-training-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatTabsModule,
    MatChipsModule,
    MatSnackBarModule,
    MatIconModule,
    MatProgressBarModule,
    MatDividerModule
  ],
  templateUrl: './training.component.html',
  styleUrls: ['./training.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TrainingPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly snackbar = inject(MatSnackBar);

  private readonly trainingCoursesUrl = `${this.config.apiBaseUrl}/TrainingCourses`;
  private readonly liteTrainingUrl = `${this.config.apiBaseUrl}/Training`;

  readonly courses = signal<readonly TrainingCourseDto[]>([]);
  readonly selectedCourse = signal<TrainingCourseDto | null>(null);
  readonly enrollments = signal<readonly CourseEnrollmentDto[]>([]);
  readonly analytics = signal<CourseProgressAnalyticsDto | null>(null);
  readonly liteCourses = signal<readonly LiteCourseDto[]>([]);
  readonly selectedLiteCourse = signal<LiteCourseDto | null>(null);
  readonly liteSessions = signal<readonly LiteCourseSessionDto[]>([]);

  readonly loadingCatalog = signal(false);
  readonly loadingLite = signal(false);

  readonly courseForm = this.fb.group({
    title: ['', Validators.required],
    category: [''],
    description: [''],
    instructor: [''],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    capacity: [25, [Validators.required, Validators.min(1)]],
    deliveryMode: ['Virtual'],
    skillLevel: ['Intermediate'],
    durationHours: [8, [Validators.required, Validators.min(1)]],
    competencyCodes: [''],
    offersCertification: [false],
    certificationCriteria: ['']
  });

  readonly enrollmentForm = this.fb.group({
    employeeId: ['', Validators.required],
    enrolledOn: ['']
  });

  readonly progressForm = this.fb.group({
    enrollmentId: ['', Validators.required],
    completionPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    status: ['InProgress', Validators.required],
    completedOn: ['']
  });

  readonly withdrawForm = this.fb.group({
    enrollmentId: ['', Validators.required]
  });

  readonly liteOrgControl = new FormControl('', { nonNullable: true });
  readonly liteCourseForm = this.fb.group({
    organizationId: ['', Validators.required],
    code: ['', Validators.required],
    title: ['', Validators.required],
    description: [''],
    durationHours: [2, [Validators.required, Validators.min(0.5)]],
    isMandatory: [false]
  });

  readonly liteSessionForm = this.fb.group({
    courseId: ['', Validators.required],
    startUtc: ['', Validators.required],
    endUtc: ['', Validators.required],
    location: [''],
    meetingUrl: [''],
    capacity: ['']
  });

  readonly liteAttendanceForm = this.fb.group({
    sessionId: ['', Validators.required],
    employeeId: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.loadingCatalog.set(true);
    this.http.get<TrainingCourseDto[]>(this.trainingCoursesUrl).subscribe({
      next: (courses) => {
        this.courses.set(courses);
        const firstCourse = courses[0];
        if (firstCourse && !this.selectedCourse()) {
          this.selectCourse(firstCourse);
        }
        this.loadingCatalog.set(false);
      },
      error: () => this.loadingCatalog.set(false)
    });
  }

  createCourse(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }
    const payload = this.buildCoursePayload();
    this.loadingCatalog.set(true);
    this.http.post<TrainingCourseDto>(this.trainingCoursesUrl, payload).subscribe({
      next: (course) => {
        this.snackbar.open('Course created.', 'Dismiss', { duration: 2500 });
        this.courseForm.reset({
          title: '',
          category: '',
          description: '',
          instructor: '',
          startDate: '',
          endDate: '',
          capacity: 25,
          deliveryMode: 'Virtual',
          skillLevel: 'Intermediate',
          durationHours: 8,
          competencyCodes: '',
          offersCertification: false,
          certificationCriteria: ''
        });
        this.courses.set([course, ...this.courses()]);
        this.selectCourse(course);
        this.loadingCatalog.set(false);
      },
      error: () => this.loadingCatalog.set(false)
    });
  }

  selectCourse(course: TrainingCourseDto): void {
    this.selectedCourse.set(course);
    this.loadEnrollments(course.id);
    this.loadAnalytics(course.id);
    this.progressForm.patchValue({ enrollmentId: '' });
    this.withdrawForm.patchValue({ enrollmentId: '' });
  }

  loadEnrollments(courseId: string): void {
    this.http.get<CourseEnrollmentDto[]>(`${this.trainingCoursesUrl}/${courseId}/enrollments`).subscribe({
      next: (enrollments) => this.enrollments.set(enrollments)
    });
  }

  loadAnalytics(courseId: string): void {
    this.http.get<CourseProgressAnalyticsDto>(`${this.trainingCoursesUrl}/${courseId}/analytics`).subscribe({
      next: (analytics) => this.analytics.set(analytics)
    });
  }

  enrollEmployee(): void {
    const course = this.selectedCourse();
    if (!course || this.enrollmentForm.invalid) {
      this.enrollmentForm.markAllAsTouched();
      return;
    }
    const payload = {
      courseId: course.id,
      employeeId: this.enrollmentForm.controls.employeeId.value,
      enrolledOn: this.enrollmentForm.controls.enrolledOn.value || null
    };
    this.http.post<CourseEnrollmentDto>(`${this.trainingCoursesUrl}/${course.id}/enrollments`, payload).subscribe({
      next: () => {
        this.snackbar.open('Employee enrolled.', 'Dismiss', { duration: 2000 });
        this.enrollmentForm.reset({ employeeId: '', enrolledOn: '' });
        this.loadEnrollments(course.id);
        this.loadAnalytics(course.id);
      }
    });
  }

  updateProgress(): void {
    if (this.progressForm.invalid) {
      this.progressForm.markAllAsTouched();
      return;
    }
    const enrollmentId = this.progressForm.controls.enrollmentId.value!;
    const payload = {
      completionPercentage: Number(this.progressForm.controls.completionPercentage.value ?? 0),
      status: this.progressForm.controls.status.value,
      completedOn: this.progressForm.controls.completedOn.value || null
    };
    this.http
      .patch<CourseEnrollmentDto>(`${this.trainingCoursesUrl}/enrollments/${enrollmentId}/progress`, payload)
      .subscribe({
        next: () => {
          this.snackbar.open('Enrollment updated.', 'Dismiss', { duration: 2000 });
          if (this.selectedCourse()) {
            this.loadEnrollments(this.selectedCourse()!.id);
            this.loadAnalytics(this.selectedCourse()!.id);
          }
        }
      });
  }

  withdrawEnrollment(): void {
    if (this.withdrawForm.invalid) {
      this.withdrawForm.markAllAsTouched();
      return;
    }
    const enrollmentId = this.withdrawForm.controls.enrollmentId.value!;
    this.http.post(`${this.trainingCoursesUrl}/enrollments/${enrollmentId}/withdraw`, {}).subscribe({
      next: () => {
        this.snackbar.open('Enrollment withdrawn.', 'Dismiss', { duration: 2000 });
        if (this.selectedCourse()) {
          this.loadEnrollments(this.selectedCourse()!.id);
          this.loadAnalytics(this.selectedCourse()!.id);
        }
      }
    });
  }

  loadLiteCourses(): void {
    const orgId = this.liteOrgControl.value?.trim();
    if (!orgId) {
      this.snackbar.open('Provide an organization identifier.', 'Dismiss', { duration: 2500 });
      return;
    }
    this.loadingLite.set(true);
    const params = new HttpParams().set('orgId', orgId);
    this.http.get<LiteCourseDto[]>(`${this.liteTrainingUrl}/courses`, { params }).subscribe({
      next: (courses) => {
        this.liteCourses.set(courses);
        this.selectedLiteCourse.set(courses[0] ?? null);
        if (courses[0]) {
          this.loadLiteSessions(courses[0].id);
        } else {
          this.liteSessions.set([]);
        }
        this.loadingLite.set(false);
      },
      error: () => this.loadingLite.set(false)
    });
  }

  createLiteCourse(): void {
    if (this.liteCourseForm.invalid) {
      this.liteCourseForm.markAllAsTouched();
      return;
    }
    this.loadingLite.set(true);
    this.http.post<LiteCourseDto>(`${this.liteTrainingUrl}/courses`, this.liteCourseForm.value).subscribe({
      next: (course) => {
        this.snackbar.open('Lite course created.', 'Dismiss', { duration: 2000 });
        this.liteCourseForm.reset({
          organizationId: '',
          code: '',
          title: '',
          description: '',
          durationHours: 2,
          isMandatory: false
        });
        this.liteCourses.set([course, ...this.liteCourses()]);
        this.selectLiteCourse(course);
        this.loadingLite.set(false);
      },
      error: () => this.loadingLite.set(false)
    });
  }

  selectLiteCourse(course: LiteCourseDto): void {
    this.selectedLiteCourse.set(course);
    this.loadLiteSessions(course.id);
    this.liteSessionForm.patchValue({ courseId: course.id });
  }

  loadLiteSessions(courseId: string): void {
    this.http.get<LiteCourseSessionDto[]>(`${this.liteTrainingUrl}/courses/${courseId}/sessions`).subscribe({
      next: (sessions) => this.liteSessions.set(sessions)
    });
  }

  createLiteSession(): void {
    if (this.liteSessionForm.invalid) {
      this.liteSessionForm.markAllAsTouched();
      return;
    }
    const payload = {
      courseId: this.liteSessionForm.controls.courseId.value,
      startUtc: this.liteSessionForm.controls.startUtc.value,
      endUtc: this.liteSessionForm.controls.endUtc.value,
      location: this.liteSessionForm.controls.location.value || null,
      meetingUrl: this.liteSessionForm.controls.meetingUrl.value || null,
      capacity: this.liteSessionForm.controls.capacity.value ? Number(this.liteSessionForm.controls.capacity.value) : null
    };
    this.http.post<LiteCourseSessionDto>(`${this.liteTrainingUrl}/sessions`, payload).subscribe({
      next: (session) => {
        this.snackbar.open('Session created.', 'Dismiss', { duration: 2000 });
        if (this.selectedLiteCourse()) {
          this.loadLiteSessions(this.selectedLiteCourse()!.id);
        }
      }
    });
  }

  manageLiteAttendance(action: 'enroll' | 'complete' | 'cancel'): void {
    if (this.liteAttendanceForm.invalid) {
      this.liteAttendanceForm.markAllAsTouched();
      return;
    }
    const sessionId = this.liteAttendanceForm.controls.sessionId.value!;
    const employeeId = this.liteAttendanceForm.controls.employeeId.value!;
    let endpoint = 'enroll';
    if (action === 'complete') {
      endpoint = 'complete';
    } else if (action === 'cancel') {
      endpoint = 'cancel';
    }
    const params = new HttpParams().set('employeeId', employeeId);
    this.http.post(`${this.liteTrainingUrl}/sessions/${sessionId}/${endpoint}`, {}, { params }).subscribe({
      next: () => {
        const message =
          action === 'enroll'
            ? 'Learner enrolled in session.'
            : action === 'complete'
            ? 'Enrollment marked complete.'
            : 'Enrollment cancelled.';
        this.snackbar.open(message, 'Dismiss', { duration: 2500 });
      }
    });
  }

  private buildCoursePayload() {
    const raw = this.courseForm.getRawValue();
    return {
      title: raw.title,
      category: raw.category ?? '',
      description: raw.description ?? '',
      instructor: raw.instructor ?? '',
      startDate: raw.startDate!,
      endDate: raw.endDate!,
      capacity: Number(raw.capacity ?? 0),
      deliveryMode: raw.deliveryMode ?? 'Virtual',
      skillLevel: raw.skillLevel ?? 'Intermediate',
      durationHours: Number(raw.durationHours ?? 0),
      competencyCodes: this.parseList(raw.competencyCodes),
      offersCertification: raw.offersCertification ?? false,
      certificationCriteria: raw.certificationCriteria ?? ''
    };
  }

  private parseList(value?: string | null): string[] {
    if (!value) {
      return [];
    }
    return value
      .split(/\r?\n|,/)
      .map((item) => item.trim())
      .filter((item) => item.length > 0);
  }
}
