import { UserRole } from '../auth/auth.models';

export interface NavigationItem {
  label: string;
  icon?: string;
  route?: string;
  children?: NavigationItem[];
  roles?: ReadonlyArray<UserRole>;
}
