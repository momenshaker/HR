export interface Subscription {
  readonly id: string;
  readonly planId: string;
  readonly status: string;
  readonly seats: number;
  readonly createdAt: string;
  readonly canceledAt: string | null;
  readonly renewsAt: string | null;
  readonly metadata: Record<string, string>;
  readonly organizationIds: readonly string[];
}

export interface CreateSubscriptionPayload {
  readonly planId: string;
  readonly seats: number;
  readonly trialPeriodDays?: number;
  readonly metadata?: Record<string, string>;
  readonly paymentMethodId?: string;
}

export interface UpdateSubscriptionPayload {
  readonly planId?: string;
  readonly seats?: number;
  readonly status?: string;
  readonly metadata?: Record<string, string>;
}

export interface Invoice {
  readonly id: string;
  readonly subscriptionId: string;
  readonly amountDue: number;
  readonly currency: string;
  readonly dueDate: string;
  readonly status: string;
  readonly hostedInvoiceUrl?: string | null;
  readonly pdfUrl?: string | null;
  readonly createdAt: string;
  readonly paidAt?: string | null;
}

export interface Plan {
  readonly id: string;
  readonly code: string;
  readonly name: string;
  readonly description: string;
  readonly price: number;
  readonly billingInterval: string;
  readonly entitlements: readonly PlanEntitlement[];
}

export interface PlanEntitlement {
  readonly featureKey: string;
  readonly displayName: string;
  readonly description: string;
  readonly measurementUnit: string;
  readonly quantity?: number | null;
}
