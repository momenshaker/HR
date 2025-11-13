import { computed, Injectable, signal } from '@angular/core';

type LookupCategory = 'branch' | 'businessUnit' | 'operatingHours' | 'industry' | 'region' | 'timeZone';

interface LookupState {
  branch: string[];
  businessUnit: string[];
  operatingHours: string[];
  industry: string[];
  region: string[];
  timeZone: string[];
}

const DEFAULT_LOOKUPS: LookupState = {
  branch: ['Headquarters', 'Field', 'Regional Office'],
  businessUnit: ['Corporate', 'Product', 'Services', 'Operations'],
  operatingHours: ['Day', 'Swing', 'Night', '24/7'],
  industry: ['Technology', 'Retail', 'Finance', 'Healthcare'],
  region: ['North America', 'EMEA', 'APAC', 'LATAM'],
  timeZone: ['UTC', 'America/New_York', 'Europe/London', 'Asia/Singapore']
};

@Injectable({ providedIn: 'root' })
export class LookupStore {
  private readonly state = signal<LookupState>({ ...DEFAULT_LOOKUPS });

  readonly branches = computed(() => this.state().branch);
  readonly businessUnits = computed(() => this.state().businessUnit);
  readonly operatingHours = computed(() => this.state().operatingHours);
  readonly industries = computed(() => this.state().industry);
  readonly regions = computed(() => this.state().region);
  readonly timeZones = computed(() => this.state().timeZone);

  setLookup(category: LookupCategory, values: readonly string[]): void {
    this.state.update((current) => ({ ...current, [category]: [...values] }));
  }

  setMany(values: Partial<Record<LookupCategory, readonly string[]>>): void {
    this.state.update((current) => {
      const next: LookupState = { ...current };
      for (const [category, list] of Object.entries(values)) {
        if (list && list.length) {
          next[category as LookupCategory] = [...list];
        }
      }
      return next;
    });
  }

  reset(): void {
    this.state.set({ ...DEFAULT_LOOKUPS });
  }
}
