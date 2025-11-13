import { UserRole } from './auth.models';

const ID_CLAIMS = [
  'sub',
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
];

const NAME_CLAIMS = [
  'name',
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
];

const EMAIL_CLAIMS = [
  'email',
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
];

const ROLE_CLAIMS = [
  'role',
  'roles',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/roles'
];

const VALID_ROLES: ReadonlyArray<UserRole> = ['Admin', 'HR', 'Manager', 'Employee'];
export const EMPLOYEE_ID_CLAIM = 'employee_id';

export function decodeJwtPayload(token: string | undefined | null): Record<string, unknown> | null {
  if (!token) {
    return null;
  }

  const parts = token.split('.');
  if (parts.length < 2) {
    return null;
  }

  const raw = parts[1];
  if (!raw) {
    return null;
  }

  const normalized = raw.replace(/-/g, '+').replace(/_/g, '/');
  const padding = normalized.length % 4;
  const padded = padding === 0 ? normalized : `${normalized}${'='.repeat(4 - padding)}`;

  try {
    const decoded = atob(padded);
    return JSON.parse(decoded);
  } catch {
    return null;
  }
}

export function extractRoles(payload: Record<string, unknown>): ReadonlyArray<UserRole> {
  const rawRoles = new Set<string>();

  for (const key of ROLE_CLAIMS) {
    const value = payload[key];
    if (typeof value === 'string') {
      rawRoles.add(value);
      continue;
    }

    if (Array.isArray(value)) {
      for (const entry of value) {
        if (typeof entry === 'string') {
          rawRoles.add(entry);
        }
      }
    }
  }

  return [...rawRoles]
    .map((role) => role.trim())
    .filter((role): role is UserRole => VALID_ROLES.includes(role as UserRole));
}

export function getJwtClaim(payload: Record<string, unknown>, keys: ReadonlyArray<string>): string | null {
  for (const key of keys) {
    const value = payload[key];
    if (typeof value === 'string' && value.trim().length > 0) {
      return value;
    }

    if (typeof value === 'number') {
      return value.toString();
    }
  }

  return null;
}

export function getIdClaimKeys(): ReadonlyArray<string> {
  return ID_CLAIMS;
}

export function getNameClaimKeys(): ReadonlyArray<string> {
  return NAME_CLAIMS;
}

export function getEmailClaimKeys(): ReadonlyArray<string> {
  return EMAIL_CLAIMS;
}

export function rolesFromToken(token: string | undefined | null): ReadonlyArray<UserRole> {
  const payload = decodeJwtPayload(token);
  return payload ? extractRoles(payload) : [];
}
