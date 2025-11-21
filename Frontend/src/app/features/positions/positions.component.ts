import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmationDialogComponent } from '@shared/components/confirmation-dialog/confirmation-dialog.component';
import { DataTableComponent, DataTableQuery } from '@shared/components/data-table/data-table.component';
import { EntityCrudFactory } from '@core/data-access';
import { PositionFormComponent, PositionFormDialogData, PositionFormValue } from './positions.form';
import { EmployeeOption, OrganizationUnitSummary, PositionSummary } from './positions.models';

interface PositionTableRow extends PositionSummary {
  readonly organizationUnitName: string;
  readonly reportsToName?: string;
  readonly occupiedByName?: string;
  readonly vacancyStatus: 'Vacant' | 'Filled';
}

@Component({
  selector: 'app-positions-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    DataTableComponent
  ],
  templateUrl: './positions.component.html',
  styleUrls: ['./positions.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PositionsPageComponent implements OnInit {
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(MatSnackBar);
  private readonly entityFactory = inject(EntityCrudFactory);
  private readonly positionsService = this.entityFactory.create<PositionFormValue, PositionFormValue, PositionSummary>(
    'positions'
  );
  private readonly employeeService = this.entityFactory.create<never, never, EmployeeOption>('employees');
  private readonly organizationUnitService = this.entityFactory.create<never, never, OrganizationUnitSummary>(
    'organization-units'
  );

  private organizationUnitsById = new Map<string, OrganizationUnitSummary>();
  private employeesById = new Map<string, EmployeeOption>();
  private positionLookup = new Map<string, PositionSummary>();

  readonly loading = signal(false);
  readonly items = signal<ReadonlyArray<PositionTableRow>>([]);
  readonly total = signal(0);
  readonly columns = {
    title: 'Title',
    jobCode: 'Job Code',
    organizationUnitName: 'Organization Unit',
    reportsToName: 'Reports To',
    occupiedByName: 'Occupant',
    grade: 'Grade',
    employmentType: 'Employment',
    vacancyStatus: 'Status',
    actions: 'Actions'
  } as const;
  readonly displayedColumns = Object.keys(this.columns);
  readonly pageSizeOptions = [10, 25, 50];

  readonly organizationUnits = signal<ReadonlyArray<OrganizationUnitSummary>>([]);
  readonly employees = signal<ReadonlyArray<EmployeeOption>>([]);
  readonly positionOptions = signal<ReadonlyArray<PositionSummary>>([]);

  private readonly querySignal = signal<DataTableQuery>({ pageIndex: 0, pageSize: 10 });
  organizationUnitFilter = '';

  ngOnInit(): void {
    this.loadOrganizationUnits();
    this.loadEmployees();
    this.refreshPositionLookups();
    this.load(this.querySignal());
  }

  onQueryChange(query: DataTableQuery): void {
    this.querySignal.set(query);
    this.load(query);
  }

  applyOrganizationFilter(value: string): void {
    this.organizationUnitFilter = value;
    this.querySignal.set({ ...this.querySignal(), pageIndex: 0 });
    this.load(this.querySignal());
  }

  openCreate(): void {
    this.openDialog();
  }

  openEdit(item: PositionTableRow): void {
    this.openDialog(item.id, this.toFormValue(item));
  }

  remove(item: PositionTableRow): void {
    this.dialog
      .open(ConfirmationDialogComponent, {
        data: {
          title: 'Delete position',
          message: `Are you sure you want to remove the ${item.title} role?`
        }
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (!confirmed) {
          return;
        }
        this.loading.set(true);
        this.positionsService.delete(item.id).subscribe({
          next: () => {
            this.snackbar.open('Position removed', 'Dismiss', { duration: 3000 });
            this.reload();
            this.refreshPositionLookups();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private load(query: DataTableQuery): void {
    this.loading.set(true);
    this.positionsService
      .list({
        page: query.pageIndex + 1,
        pageSize: query.pageSize,
        search: query.search,
        sort: query.sortField,
        direction: query.sortDirection,
        filters: this.buildFilters()
      })
      .subscribe({
        next: (response) => {
          this.items.set(response.items.map((position) => this.toTableRow(position)));
          this.total.set(response.totalCount);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  private openDialog(positionId?: string, payload?: PositionFormValue): void {
    const dialogData: PositionFormDialogData = {
      value: payload,
      organizationUnits: this.organizationUnits(),
      positions: this.positionOptions(),
      employees: this.employees(),
      defaultOrganizationUnitId: this.organizationUnitFilter || undefined
    };

    this.dialog
      .open(PositionFormComponent, {
        width: '720px',
        data: dialogData
      })
      .afterClosed()
      .subscribe((value?: PositionFormValue) => {
        if (!value) {
          return;
        }

        this.loading.set(true);
        const payload = this.normalizePayload(value);
        const action = positionId ? this.positionsService.update(positionId, payload) : this.positionsService.create(payload);
        action.subscribe({
          next: () => {
            this.snackbar.open(positionId ? 'Position updated' : 'Position created', 'Dismiss', { duration: 3000 });
            this.reload();
            this.refreshPositionLookups();
          },
          error: () => this.loading.set(false)
        });
      });
  }

  private toTableRow(position: PositionSummary): PositionTableRow {
    return {
      ...position,
      organizationUnitName: this.organizationUnitsById.get(position.organizationUnitId)?.name ?? 'Unknown unit',
      reportsToName: this.getPositionLabel(position.reportsToPositionId),
      occupiedByName: this.getEmployeeLabel(position.occupiedByEmployeeId),
      vacancyStatus: position.isVacant ? 'Vacant' : 'Filled'
    };
  }

  private toFormValue(position: PositionSummary): PositionFormValue {
    return {
      title: position.title,
      jobCode: position.jobCode,
      organizationUnitId: position.organizationUnitId,
      reportsToPositionId: position.reportsToPositionId ?? undefined,
      occupiedByEmployeeId: position.occupiedByEmployeeId ?? undefined,
      grade: position.grade,
      employmentType: position.employmentType,
      effectiveFrom: position.effectiveFrom ?? undefined,
      effectiveTo: position.effectiveTo ?? undefined,
      isCriticalRole: position.isCriticalRole,
      isVacant: position.isVacant
    };
  }

  private normalizePayload(payload: PositionFormValue): PositionFormValue {
    return {
      ...payload,
      reportsToPositionId: payload.reportsToPositionId || undefined,
      occupiedByEmployeeId: payload.occupiedByEmployeeId || undefined
    };
  }

  private buildFilters(): Record<string, string | undefined> | undefined {
    if (!this.organizationUnitFilter) {
      return undefined;
    }
    return { organizationUnitId: this.organizationUnitFilter };
  }

  private reload(): void {
    this.load(this.querySignal());
  }

  private refreshPositionLookups(): void {
    this.positionsService
      .list({ page: 1, pageSize: 1000 })
      .subscribe({
        next: (response) => {
          this.positionOptions.set(response.items);
          this.positionLookup = new Map(response.items.map((item) => [item.id, item]));
        }
      });
  }

  private loadOrganizationUnits(): void {
    this.organizationUnitService.list().subscribe({
      next: (response) => {
        this.organizationUnits.set(response.items);
        this.organizationUnitsById = new Map(response.items.map((unit) => [unit.id, unit]));
      },
      error: () => this.snackbar.open('Failed to load organization units.', 'Dismiss', { duration: 3000 })
    });
  }

  private loadEmployees(): void {
    this.employeeService
      .list({ page: 1, pageSize: 1000 })
      .subscribe({
        next: (response) => {
          this.employees.set(response.items);
          this.employeesById = new Map(response.items.map((employee) => [employee.id, employee]));
        },
        error: () => this.snackbar.open('Failed to load employees.', 'Dismiss', { duration: 3000 })
      });
  }

  private getPositionLabel(positionId?: string | null): string | undefined {
    if (!positionId) {
      return undefined;
    }
    const position = this.positionLookup.get(positionId);
    return position?.title;
  }

  private getEmployeeLabel(employeeId?: string | null): string | undefined {
    if (!employeeId) {
      return undefined;
    }
    const employee = this.employeesById.get(employeeId);
    if (!employee) {
      return undefined;
    }
    return `${employee.firstName} ${employee.lastName}`;
  }
}
