import { Routes } from '@angular/router';
import { TimesheetsPageComponent } from './timesheets.component';

export const TIMESHEETS_ROUTES: Routes = [
  {
    path: '',
    component: TimesheetsPageComponent,
    title: 'Timesheets'
  }
];
