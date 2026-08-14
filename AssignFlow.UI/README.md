# AssignFlow UI

Angular 21 standalone frontend for the AssignFlow assignment and submission platform.

## Stack

- Angular 21 with strict templates and standalone components
- PrimeNG 21 with a customized Aura design preset
- Reactive forms, signals, OnPush change detection, functional guards and interceptor
- PrimeNG Toast and ConfirmDialog for user feedback
- Lazy-loaded role-aware feature pages

## Run locally

```powershell
npm install
ng serve
```

The client runs at `http://localhost:4200` and calls the IIS Express API directly at `https://localhost:44383/api`. No Angular or Nginx API proxy is used.

## Production build

```powershell
npm run build
```

Build output is written to `dist/assignflow-ui`.

## Structure

- `core` contains authentication, guards, the API interceptor/client and toast service.
- `data/types` keeps every API contract in its own focused file.
- `data/services` contains small feature-specific HTTP services.
- `features` contains lazy-loaded standalone pages.
- `layout` contains the responsive role-based application shell.
