export interface PositionSummary {
  readonly id: string;
  readonly title: string;
  readonly jobCode: string;
  readonly organizationUnitId: string;
  readonly reportsToPositionId?: string | null;
  readonly occupiedByEmployeeId?: string | null;
  readonly grade: string;
  readonly employmentType: string;
  readonly effectiveFrom?: string | null;
  readonly effectiveTo?: string | null;
  readonly isCriticalRole: boolean;
  readonly isVacant: boolean;
}

export interface OrganizationUnitSummary {
  readonly id: string;
  readonly name: string;
  readonly code: string;
}

export interface EmployeeOption {
  readonly id: string;
  readonly firstName: string;
  readonly lastName: string;
  readonly jobTitle: string;
}
