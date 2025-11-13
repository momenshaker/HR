import { Routes } from '@angular/router';
import { EmployeesPageComponent } from './employees.component';
import { EmployeeDetailComponent } from './employees.detail.component';

export const EMPLOYEES_ROUTES: Routes = [
  { path: '', component: EmployeesPageComponent },
  { path: ':id', component: EmployeeDetailComponent }
];
