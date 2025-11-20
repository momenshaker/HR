import { HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { LookupApiService } from './lookup.service';
import { DEFAULT_LOOKUP_SEED, LookupDictionary, LookupValue, LookupValuePayload } from './lookup.types';

type LookupCategoryKey = keyof typeof DEFAULT_LOOKUP_SEED;

function cloneSeed(): LookupDictionary {
  const clone: LookupDictionary = {};
  for (const [category, values] of Object.entries(DEFAULT_LOOKUP_SEED)) {
    clone[category] = values.map((value) => ({ ...value }));
  }
  return clone;
}

@Injectable({ providedIn: 'root' })
export class LookupStore {
  private readonly api = inject(LookupApiService);
  private readonly state = signal<LookupDictionary>(cloneSeed());
  private readonly loadingSignal = signal(false);
  private readonly etagSignal = signal<string | null>(null);

  readonly isLoading = computed(() => this.loadingSignal());
  readonly categories = computed(() => Object.keys(this.state()).sort((a, b) => a.localeCompare(b)));
  readonly branches = computed(() => this.toLabels('branch'));
  readonly businessUnits = computed(() => this.toLabels('businessUnit'));
  readonly operatingHours = computed(() => this.toLabels('operatingHours'));
  readonly industries = computed(() => this.toLabels('industry'));
  readonly regions = computed(() => this.toLabels('region'));
  readonly timeZones = computed(() => this.toLabels('timeZone'));
  readonly leaveTypes = computed(() => this.toLabels('leaveType'));
  readonly roles = computed(() => this.toLabels('role'));
  readonly ratingScales = computed(() => this.toLabels('ratingScale'));

  load(force = false): Observable<void> {
    if (this.loadingSignal()) {
      return new Observable<void>((subscriber) => subscriber.complete());
    }

    this.loadingSignal.set(true);
    return this.api
      .list(force ? null : this.etagSignal())
      .pipe(
        tap((response) => {
          this.loadingSignal.set(false);
          if (response.status === 304) {
            return;
          }
          const payload = response.body?.items?.[0];
          if (!payload) {
            return;
          }
          this.applyCollection(payload.categories);
          const etag = response.headers.get('etag');
          this.etagSignal.set(etag ?? null);
        }),
        catchError((error: HttpErrorResponse) => {
          this.loadingSignal.set(false);
          return throwError(() => error);
        }),
        map(() => void 0)
      );
  }

  loadCategory(category: string): Observable<void> {
    if (!category) {
      return new Observable<void>((subscriber) => subscriber.complete());
    }

    return this.api.getByCategory(category).pipe(
      tap((values) => {
        this.replaceCategory(category, values);
        this.etagSignal.set(null);
      }),
      catchError((error: HttpErrorResponse) => throwError(() => error)),
      map(() => void 0)
    );
  }

  fetchValue(id: string): Observable<LookupValue> {
    return this.api.getById(id).pipe(
      tap((value) => {
        this.insertValue(value);
      })
    );
  }

  create(payload: LookupValuePayload): Observable<LookupValue> {
    return this.api.create(payload).pipe(
      tap((value) => {
        this.insertValue(value);
        this.etagSignal.set(null);
      })
    );
  }

  update(id: string, payload: LookupValuePayload): Observable<LookupValue> {
    return this.api.update(id, payload).pipe(
      tap((value) => {
        this.insertValue(value);
        this.etagSignal.set(null);
      })
    );
  }

  delete(id: string): Observable<void> {
    return this.api.delete(id).pipe(
      tap(() => {
        this.removeValue(id);
        this.etagSignal.set(null);
      })
    );
  }

  getValues(category: string): readonly LookupValue[] {
    return this.state()[category] ?? [];
  }

  nextSortOrder(category: string): number {
    const values = this.state()[category] ?? [];
    return values.length + 1;
  }

  reset(): void {
    this.state.set(cloneSeed());
    this.etagSignal.set(null);
  }

  private toLabels(category: LookupCategoryKey): readonly string[] {
    return (this.state()[category] ?? [])
      .filter((value) => value.isActive)
      .sort((a, b) => a.sortOrder - b.sortOrder)
      .map((value) => value.displayName);
  }

  private applyCollection(categories: readonly { category: string; values: readonly LookupValue[] }[]): void {
    const next: LookupDictionary = {};
    for (const category of categories) {
      next[category.category] = [...category.values].sort(this.sortValues);
    }
    this.state.set(next);
  }

  private insertValue(value: LookupValue): void {
    const category = value.category;
    this.state.update((current) => {
      const next: LookupDictionary = {};
      for (const [key, values] of Object.entries(current)) {
        next[key] = values.filter((item) => item.id !== value.id);
      }

      const targetValues = next[category] ?? [];
      next[category] = [...targetValues, value].sort(this.sortValues);
      return next;
    });
  }

  private removeValue(id: string): void {
    let targetCategory: string | undefined;
    for (const [category, values] of Object.entries(this.state())) {
      if (values.some((value) => value.id === id)) {
        targetCategory = category;
        break;
      }
    }

    if (!targetCategory) {
      return;
    }

    this.state.update((current) => {
      const nextValues = (current[targetCategory!] ?? []).filter((value) => value.id !== id);
      return { ...current, [targetCategory!]: nextValues };
    });
  }

  private readonly sortValues = (a: LookupValue, b: LookupValue): number => {
    if (a.sortOrder === b.sortOrder) {
      return a.displayName.localeCompare(b.displayName);
    }
    return a.sortOrder - b.sortOrder;
  };

  private replaceCategory(category: string, values: readonly LookupValue[]): void {
    this.state.update((current) => ({ ...current, [category]: [...values].sort(this.sortValues) }));
  }
}
