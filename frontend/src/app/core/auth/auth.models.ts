import { ProblemDetails as CoreProblemDetails } from '../errors/problem-details';

export type UserRole = 'Admin' | 'HR' | 'Manager' | 'Employee';

export interface AuthUser {
  id: string;
  fullName: string;
  email: string;
  avatarUrl?: string;
  roles: ReadonlyArray<UserRole>;
  employeeId?: string;
  organizationIds?: ReadonlyArray<string>;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export type ProblemDetails = CoreProblemDetails;
