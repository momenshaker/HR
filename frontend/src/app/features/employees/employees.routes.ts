import { Routes } from '@angular/router';
import { EmployeesPageComponent } from './employees.component';
import { EmployeeDetailComponent } from './employees.detail.component';
import { EmployeeHierarchyComponent } from './employee-hierarchy.component';

export const EMPLOYEES_ROUTES: Routes = [
  { path: '', component: EmployeesPageComponent },
  { path: 'hierarchy', component: EmployeeHierarchyComponent },
  { path: ':id', component: EmployeeDetailComponent }
];
