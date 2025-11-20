import { Routes } from '@angular/router';
import { LoginPageComponent } from './login-page.component';
import { LogoutPageComponent } from './logout-page.component';
import { OnboardingPageComponent } from '../onboarding/onboarding-page.component';

export const AUTH_ROUTES: Routes = [
  { path: 'login', component: LoginPageComponent },
  { path: 'onboarding', component: OnboardingPageComponent },
  { path: 'logout', component: LogoutPageComponent }
];
