import { DefaultUrlSerializer, UrlSerializer, UrlTree } from '@angular/router';

const CAMEL_TO_KEBAB = /([a-z0-9])([A-Z])/g;
const MULTI_CAP_TO_KEBAB = /([A-Z])([A-Z][a-z])/g;

const toKebabCase = (value: string): string => {
  if (!value) {
    return value;
  }

  const replaced = value
    .replace(CAMEL_TO_KEBAB, '$1-$2')
    .replace(MULTI_CAP_TO_KEBAB, '$1-$2')
    .replace(/[-_]+/g, '-');

  return replaced.replace(/^-+|-+$/g, '').toLowerCase();
};

const normalizeSegment = (segment: string): string => {
  if (segment.startsWith(':')) {
    return segment;
  }
  return toKebabCase(segment);
};

const normalizePath = (path: string): string => {
  return path
    .split('/')
    .map(normalizeSegment)
    .join('/');
};

const splitUrl = (url: string): { path: string; remainder: string } => {
  const queryIndex = url.indexOf('?');
  const hashIndex = url.indexOf('#');
  const indices = [queryIndex, hashIndex].filter((i) => i >= 0);
  const splitAt = indices.length === 0 ? url.length : Math.min(...indices);
  return {
    path: url.slice(0, splitAt),
    remainder: url.slice(splitAt)
  };
};

export class CaseInsensitiveUrlSerializer implements UrlSerializer {
  private readonly delegate = new DefaultUrlSerializer();

  parse(url: string): UrlTree {
    const { path, remainder } = splitUrl(url);
    const normalizedPath = normalizePath(path);
    return this.delegate.parse(`${normalizedPath}${remainder}`);
  }

  serialize(tree: UrlTree): string {
    return this.delegate.serialize(tree);
  }
}
