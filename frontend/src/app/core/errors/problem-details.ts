export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  message?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

type ProblemPayload = Record<string, unknown>;

function getStringValue(payload: ProblemPayload, ...keys: string[]): string | undefined {
  for (const key of keys) {
    const value = payload[key];
    if (typeof value === 'string') {
      return value;
    }
  }
  return undefined;
}

function getNumberValue(payload: ProblemPayload, ...keys: string[]): number | undefined {
  for (const key of keys) {
    const value = payload[key];
    if (typeof value === 'number') {
      return value;
    }
  }
  return undefined;
}

function getErrors(payload: ProblemPayload): Record<string, string[]> | undefined {
  const candidate = payload['errors'] ?? payload['Errors'];
  if (!candidate || typeof candidate !== 'object') {
    return undefined;
  }

  const entries = Object.entries(candidate as Record<string, unknown>).reduce<Record<string, string[]>>(
    (acc, [key, value]) => {
      if (Array.isArray(value) && value.every((item) => typeof item === 'string')) {
        acc[key] = value;
      }
      return acc;
    },
    {}
  );

  return Object.keys(entries).length ? entries : undefined;
}

export function normalizeProblemDetails(value?: ProblemDetails | Record<string, unknown> | null): ProblemDetails | null {
  if (!value || typeof value !== 'object') {
    return null;
  }

  const payload = value as ProblemPayload;

  return {
    type: getStringValue(payload, 'type', 'Type'),
    title: getStringValue(payload, 'title', 'Title'),
    detail: getStringValue(payload, 'detail', 'Detail'),
    message: getStringValue(payload, 'message', 'Message'),
    status: getNumberValue(payload, 'status', 'Status'),
    instance: getStringValue(payload, 'instance', 'Instance'),
    errors: getErrors(payload)
  };
}

export function extractProblemMessage(problem?: ProblemDetails | null): string {
  if (!problem) {
    return 'An unexpected error occurred';
  }

  if (problem.errors) {
    const [, messages] = Object.entries(problem.errors)[0] ?? [];
    if (messages?.length) {
      return messages.join('\n');
    }
  }

  return problem.message ?? problem.detail ?? problem.title ?? 'An unexpected error occurred';
}
