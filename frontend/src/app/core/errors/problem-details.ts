export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
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

  return problem.detail ?? problem.title ?? 'An unexpected error occurred';
}
