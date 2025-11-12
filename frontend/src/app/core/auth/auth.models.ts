export type UserRole = 'Admin' | 'HR' | 'Manager' | 'Employee';

export interface AuthUser {
  id: string;
  fullName: string;
  email: string;
  avatarUrl?: string;
  roles: ReadonlyArray<UserRole>;
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

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
