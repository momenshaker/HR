import { Routes } from '@angular/router';
import { LoginPageComponent } from './login-page.component';
import { LogoutPageComponent } from './logout-page.component';

export const AUTH_ROUTES: Routes = [
  { path: 'login', component: LoginPageComponent },
  { path: 'logout', component: LogoutPageComponent }
];
