import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APP_CONFIG } from '@core/config/app-config.token';
import { AppConfig } from '@core/config/app-config.model';

export interface OnboardingAccountInfo {
  readonly email: string;
  readonly password: string;
  readonly fullName: string;
  readonly phoneNumber?: string;
}

export interface OnboardingOrganizationInfo {
  readonly name: string;
  readonly code: string;
  readonly description?: string;
  readonly industry?: string;
  readonly region?: string;
  readonly headquartersAddress: string;
  readonly timeZone?: string;
  readonly primaryContactEmail?: string;
  readonly websiteUrl?: string;
  readonly billingAddressLine1: string;
  readonly billingAddressLine2?: string;
  readonly billingCity: string;
  readonly billingState: string;
  readonly billingPostalCode: string;
  readonly billingCountry: string;
  readonly billingPhone?: string;
}

export interface OnboardingSubscriptionSelection {
  readonly planId: string;
  readonly seats: number;
  readonly trialPeriodDays?: number;
}

export interface OnboardingRequest {
  readonly account: OnboardingAccountInfo;
  readonly organization: OnboardingOrganizationInfo;
  readonly subscription: OnboardingSubscriptionSelection;
}

export interface OnboardingResponse {
  readonly customerId: string;
  readonly organizationId: string;
  readonly subscriptionId: string;
  readonly adminUserId: string;
  readonly adminEmployeeId: string;
}

@Injectable({ providedIn: 'root' })
export class OnboardingApiService {
  private readonly http = inject(HttpClient);
  private readonly config = inject<AppConfig>(APP_CONFIG);
  private readonly baseUrl = `${this.config.apiBaseUrl}/onboarding`;

  start(payload: OnboardingRequest): Observable<OnboardingResponse> {
    return this.http.post<OnboardingResponse>(this.baseUrl, payload);
  }
}
