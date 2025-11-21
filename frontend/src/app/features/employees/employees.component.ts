import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { EntityCrudFactory } from '@core/data-access';
import { EmployeeFormComponent, EmployeeFormValue } from './employees.form';

interface EmployeeSummary {
  id: string;
  fullName: string;
  email: string;
  departmentName?: string;
  jobTitle?: string;
  status?: string;
}

interface EmployeeDocument {
  id: string;
  fileName: string;
  storagePath: string;
  description?: string;
  contentType?: string;
  uploadedAtUtc: string;
}

interface EmployeeDetail {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  jobTitle?: string;
  phoneNumber?: string;
  employmentType?: 'FullTime' | 'PartTime' | 'Contractor';
  employmentStartDate?: string;
  employmentEndDate?: string;
  dateOfBirth?: string;
  primaryDepartmentId: string;
  departmentIds: string[];
  profileDocuments?: EmployeeDocument[];
}

@Component({
  selector: 'app-employees-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeesPageComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(EntityCrudFactory).create<EmployeeFormValue, EmployeeFormValue, EmployeeSummary>('employees');
  private readonly detailService = inject(EntityCrudFactory).create<never, never, EmployeeDetail>('employees');

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<EmployeeSummary>>([]);
  readonly total = signal(0);
  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });
  private departmentFilter?: string;

  readonly columns = {
    fullName: 'Name',
    email: 'Email',
    departmentName: 'Department',
    jobTitle: 'Job Title',
    status: 'Status',
    actions: 'Actions'
  } as const;

  readonly displayedColumns = Object.keys(this.columns);

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      const departmentId = params.get('departmentId');
      this.departmentFilter = departmentId ?? undefined;
      this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
      this.load(this.querySignal());
    });
  }

  viewHierarchy(): void {
    this.router.navigate(['/employees/hierarchy']);
  }

  view(item: EmployeeSummary): void {
    this.router.navigate(['/employees', item.id]);
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  create(): void {
    this.openForm();
  }

  edit(item: EmployeeSummary): void {
    this.loading.set(true);
    this.detailService.getById(item.id).subscribe({
      next: (employee) => {
        this.loading.set(false);
        this.openForm(item.id, this.toFormValue(employee));
      },
      error: () => this.loading.set(false)
    });
  }

  remove(item: EmployeeSummary): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete employee',
          message: `Deactivate ${item.fullName}?`
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }
        this.loading.set(true);
        this.service.delete(item.id).subscribe({
          next: () => {
            this.snackbar.open('Employee removed', 'Dismiss', { duration: 3000 });
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private openForm(employeeId?: string, payload?: EmployeeFormValue): void {
    const dialogData = this.buildDialogData(payload);
    if (!dialogData) {
      return;
    }

    this.dialog
      .open(EmployeeFormComponent, {
        width: '520px',
        data: dialogData
      })
      .afterClosed()
      .subscribe((value?: EmployeeFormValue) => {
        if (!value) {
          return;
        }

        this.loading.set(true);
        const action = employeeId ? this.service.update(employeeId, value) : this.service.create(value);
        action.subscribe({
          next: () => {
            this.snackbar.open(employeeId ? 'Employee updated' : 'Employee created', 'Dismiss', { duration: 3000 });
            this.load(this.querySignal());
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private toFormValue(employee: EmployeeDetail): EmployeeFormValue {
    return {
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      jobTitle: employee.jobTitle,
      phoneNumber: employee.phoneNumber,
      employmentType: employee.employmentType,
      employmentStartDate: employee.employmentStartDate,
      employmentEndDate: employee.employmentEndDate,
      dateOfBirth: employee.dateOfBirth,
      departmentAssignment: {
        primaryDepartmentId: employee.primaryDepartmentId,
        secondaryDepartmentIds: employee.departmentIds.filter((id) => id !== employee.primaryDepartmentId)
      },
      profileDocuments: (employee.profileDocuments ?? []).map((document) => ({
        fileName: document.fileName,
        storagePath: document.storagePath,
        description: document.description,
        contentType: document.contentType,
        uploadedAtUtc: document.uploadedAtUtc
      }))
    };
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    const filters = this.departmentFilter ? { departmentId: this.departmentFilter } : undefined;
    this.service
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection,
        filters
      })
      .subscribe({
        next: (response) => {
          this.items.set(response.items);
          this.total.set(response.totalCount);
          this.loading.set(false);
        },
      error: () => this.loading.set(false)
    });
  }

  private buildDialogData(payload?: EmployeeFormValue): Partial<EmployeeFormValue> | null {
    if (payload) {
      return payload;
    }
    if (!this.departmentFilter) {
      this.snackbar.open('Select a department before adding employees.', 'Dismiss', { duration: 3000 });
      return null;
    }
    return {
      departmentAssignment: {
        primaryDepartmentId: this.departmentFilter,
        secondaryDepartmentIds: []
      }
    };
  }
}
