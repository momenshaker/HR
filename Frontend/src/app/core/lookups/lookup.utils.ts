import { LookupValue } from './lookup.types';

export const LEAVE_TYPE_LOOKUP_CATEGORY = 'leaveType';

export interface LeaveTypeLookupOption {
  readonly id: string;
  readonly code: string;
  readonly name: string;
}

export function normalizeLeaveTypeOptions(values: readonly LookupValue[]): LeaveTypeLookupOption[] {
  return values
    .filter((value) => value.isActive)
    .map((value) => ({
      id: value.id,
      code: value.code,
      name: value.displayName
    }));
}
