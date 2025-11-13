export interface AttendancePunch {
  id: string;
  type: string;
  timestampUtc: string;
  notes?: string;
}

export interface AttendanceRecord {
  id: string;
  employeeId: string;
  workDate: string;
  shiftName: string;
  overtimeMinutes: number;
  status: string;
  notes: string;
  punches: AttendancePunch[];
}

export interface LeaveRequest {
  id: string;
  employeeId: string;
  leaveTypeId: string;
  leaveType: string;
  startDate: string;
  endDate: string;
  numberOfDays: number;
  status: string;
  approverId?: string;
  reason: string;
  attachmentPath?: string;
  submittedAtUtc: string;
  approvedAtUtc?: string;
  rejectedAtUtc?: string;
  cancelledAtUtc?: string;
}

export interface SalarySlip {
  payrollRunId: string;
  periodStart: string;
  periodEnd: string;
  processedAtUtc: string;
  status: string;
  grossPay: number;
  netPay: number;
  notes: string;
}

export interface TrainingCourse {
  id: string;
  title: string;
  category: string;
  description: string;
  instructor: string;
  startDate: string;
  endDate: string;
  durationHours: number;
  deliveryMode: string;
  offersCertification: boolean;
  skillLevel: string;
}

export interface DelegatedAuthority {
  id: string;
  grantorEmployeeId?: string;
  delegateEmployeeId?: string;
  grantorPositionId?: string;
  delegatePositionId?: string;
  authorityScope: string;
  approvalLimit?: number;
  grantedOnUtc: string;
  expiresOnUtc?: string;
  revokedOnUtc?: string;
  isRevoked: boolean;
  notes: string;
}

export interface PositionSnapshot {
  id: string;
  title: string;
  grade: string;
  employmentType: string;
  jobCode: string;
}

export interface OrganizationUnitSnapshot {
  id: string;
  name: string;
  code: string;
  type: string;
}

export interface ReportingRelationship {
  id: string;
  relationshipType: string;
  managerPositionId: string;
  reportPositionId: string;
}

export interface SelfServiceAccount {
  id: string;
  email: string;
  oauthProvider: string;
  externalIdentifier: string;
  isMfaEnabled: boolean;
  isLocked: boolean;
  createdOnUtc: string;
  updatedOnUtc?: string;
  lastSignInUtc?: string;
  featureAccess: string[];
}

export interface EmployeeOrganizationSnapshot {
  employeeId: string;
  position?: PositionSnapshot;
  organizationUnit?: OrganizationUnitSnapshot;
  reportingLines: ReportingRelationship[];
  delegatedAuthorities: DelegatedAuthority[];
  selfServiceAccount?: SelfServiceAccount;
}

export interface ClockInPayload {
  shiftName?: string;
  timestampUtc?: string;
  notes?: string;
  punchType?: string;
}

export interface ClockOutPayload {
  timestampUtc?: string;
  notes?: string;
  punchType?: string;
}

export interface LeaveRequestPayload {
  leaveTypeId: string;
  startDate: string;
  endDate: string;
  reason?: string;
  attachmentPath?: string;
  employeeId?: string;
}

export interface LeaveType {
  id: string;
  code: string;
  name: string;
  isPaid: boolean;
  requiresApproval: boolean;
  requiresAttachment: boolean;
  annualAllowanceDays: number;
  carryOverDays: number;
  maxConsecutiveDays?: number;
  color: string;
}

export interface SelfServiceAccountPayload {
  employeeId: string;
  email: string;
  oauthProvider: string;
  externalIdentifier: string;
  isMfaEnabled: boolean;
  isLocked: boolean;
  lastSignInUtc?: string;
  featureAccess?: string[];
}

export interface SelfServiceAccountUpdatePayload {
  email: string;
  oauthProvider: string;
  externalIdentifier: string;
  isMfaEnabled: boolean;
  isLocked: boolean;
  lastSignInUtc?: string;
  featureAccess?: string[];
}
