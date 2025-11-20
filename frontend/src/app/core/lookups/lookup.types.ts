export interface LookupValue {
  readonly id: string;
  readonly category: string;
  readonly code: string;
  readonly displayName: string;
  readonly description?: string | null;
  readonly sortOrder: number;
  readonly isActive: boolean;
  readonly updatedAtUtc: string;
}

export interface LookupCategory {
  readonly category: string;
  readonly values: readonly LookupValue[];
}

export interface LookupCollection {
  readonly versionToken: string;
  readonly categories: readonly LookupCategory[];
}

export interface LookupValuePayload {
  readonly category: string;
  readonly code: string;
  readonly displayName: string;
  readonly description?: string | null;
  readonly sortOrder?: number | null;
  readonly isActive?: boolean;
}

export type LookupDictionary = Record<string, readonly LookupValue[]>;

const seedTimestamp = '2024-01-01T00:00:00.000Z';

export const DEFAULT_LOOKUP_SEED: LookupDictionary = {
  branch: [
    { id: 'ea6e39b4-7911-43d8-aba5-5e1846f88874', category: 'branch', code: 'HEADQUARTERS', displayName: 'Headquarters', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'f4aa9d9b-5925-48c6-ba70-220bb9260c6a', category: 'branch', code: 'FIELD', displayName: 'Field', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '719b7a47-109f-431d-9418-9d3cdb377c7a', category: 'branch', code: 'REGIONAL_OFFICE', displayName: 'Regional Office', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  businessUnit: [
    { id: '771cb91a-a6ae-453f-bdc9-f4baf22fc436', category: 'businessUnit', code: 'CORPORATE', displayName: 'Corporate', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '8e6db0b5-a580-46c8-9a00-c4a81b448f3a', category: 'businessUnit', code: 'PRODUCT', displayName: 'Product', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '37aa74bb-f001-4ecc-bf7a-b35057c35f0a', category: 'businessUnit', code: 'SERVICES', displayName: 'Services', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '2f48a4d1-3519-4b02-86c3-1e77fec77bf0', category: 'businessUnit', code: 'OPERATIONS', displayName: 'Operations', description: null, sortOrder: 4, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  operatingHours: [
    { id: '86694dbe-a50f-4330-a794-cf9625528ac3', category: 'operatingHours', code: 'DAY', displayName: 'Day', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '36d48abe-4ade-4189-af71-03e38526f06d', category: 'operatingHours', code: 'SWING', displayName: 'Swing', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '972e6450-17e9-48ee-ac9a-9041d6d8fb97', category: 'operatingHours', code: 'NIGHT', displayName: 'Night', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'dfce0848-757f-4ebd-990a-f6861a25b981', category: 'operatingHours', code: '24_7', displayName: '24/7', description: null, sortOrder: 4, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  industry: [
    { id: '02029a6d-a17d-4773-92ba-ac63955e8a17', category: 'industry', code: 'TECHNOLOGY', displayName: 'Technology', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'e03d5548-0998-4b74-836f-ace82fd812f3', category: 'industry', code: 'RETAIL', displayName: 'Retail', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '17f974c2-ed4d-454a-b592-86e48bf74e3f', category: 'industry', code: 'FINANCE', displayName: 'Finance', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '5c1e0e99-540c-4dea-a361-b49d3e1c2ec5', category: 'industry', code: 'HEALTHCARE', displayName: 'Healthcare', description: null, sortOrder: 4, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  region: [
    { id: '44c8cf0e-ea28-4f0f-ae7c-990f361776a5', category: 'region', code: 'NORTH_AMERICA', displayName: 'North America', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'fce559ca-6ff7-4876-a770-c24126aef993', category: 'region', code: 'EMEA', displayName: 'EMEA', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'c785c01d-84a4-4923-a6e7-fa59b15f63b4', category: 'region', code: 'APAC', displayName: 'APAC', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'a88beff5-2332-42f8-9a96-0550c1d2364a', category: 'region', code: 'LATAM', displayName: 'LATAM', description: null, sortOrder: 4, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  timeZone: [
    { id: '3c2aba9e-a562-4f72-a564-a85f25689272', category: 'timeZone', code: 'UTC', displayName: 'UTC', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'c8c741e5-fc1b-4895-bc98-6ae8607afdcf', category: 'timeZone', code: 'AMERICA_NEW_YORK', displayName: 'America/New_York', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'fd228009-14bb-46e0-ade8-aabeb82f8abd', category: 'timeZone', code: 'EUROPE_LONDON', displayName: 'Europe/London', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '383c0c64-3592-475f-8b31-cd5b5cfcc146', category: 'timeZone', code: 'ASIA_SINGAPORE', displayName: 'Asia/Singapore', description: null, sortOrder: 4, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  leaveType: [
    { id: '5897d8d7-8eb8-4612-a123-3f8d7f5d24c8', category: 'leaveType', code: 'VACATION', displayName: 'Vacation', description: null, sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '65a1f6c3-9e6a-4d5f-901f-3b4e7c1f0e3f', category: 'leaveType', code: 'SICK', displayName: 'Sick', description: null, sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'b6b1c9e2-3d44-4c01-9f21-a9c9475c6f4f', category: 'leaveType', code: 'PERSONAL', displayName: 'Personal', description: null, sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  ratingScale: [
    { id: 'd1c7b9f9-4b97-4f11-92b8-2d67320b6a81', category: 'ratingScale', code: 'FIVE_POINT', displayName: 'Five-point scale', description: 'Standard 1-5 rating scale', sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '7f5c4d20-1d66-4db2-959f-8c4b7c08d42a', category: 'ratingScale', code: 'FOUR_POINT', displayName: 'Four-point scale', description: '4-point rating scale with no neutral', sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: '4f8bdb5c-9b6e-4c70-beb9-052ac5c67f22', category: 'ratingScale', code: 'THREE_POINT', displayName: 'Three-point scale', description: 'Simple 3-point rating scale', sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp }
  ],
  role: [
    { id: '6b3d5a72-29a5-4b8c-8380-a6a59f1e3f12', category: 'role', code: 'ADMIN', displayName: 'Administrator', description: 'Full system access', sortOrder: 1, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'bb5fabd6-5b4d-4cb4-b89c-8c5c583a872d', category: 'role', code: 'MANAGER', displayName: 'Manager', description: 'Approval and managerial duties', sortOrder: 2, isActive: true, updatedAtUtc: seedTimestamp },
    { id: 'a1d2c3b4-0f56-4eba-8c4d-7c0d9e3f4a8b', category: 'role', code: 'STAFF', displayName: 'Staff', description: 'Individual contributor', sortOrder: 3, isActive: true, updatedAtUtc: seedTimestamp }
  ]
};
