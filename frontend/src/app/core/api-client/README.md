# API Client Generation

This folder is reserved for the generated OpenAPI clients. Run the following command to regenerate the TypeScript clients from the backend specifications:

```bash
npm run generate:api
npm run generate:api:core
npm run generate:api:recruitment
```

The generator uses [`openapi-typescript-codegen`](https://github.com/ferdikoomen/openapi-typescript-codegen) with the Axios client for first-class TypeScript types and service wrappers. Do not edit generated files manually.
