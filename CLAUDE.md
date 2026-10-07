# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A monorepo of community-maintained provider packages for [Umbraco Automate](https://github.com/umbraco/Umbraco.Automate) (Umbraco CMS's workflow-automation engine). Each package contributes some combination of a connection type, triggers, and actions for a third-party service. Packages version and release **independently** of each other even though they share one git history.

- `GoogleSheets/` — Google Sheets connection + actions (OAuth). The only package currently listed as released in `README.md`.
- `Skoda/` — Škoda vehicle connection + actions (API key), split into `Umbraco.Community.Automate.Skoda` (logic) and `Umbraco.Community.Automate.Skoda.Assets` (backoffice icons served as static web assets). Currently in progress on this branch.
- `Umbraco.Community.Automate.Demo/` — throwaway Umbraco site (SQLite, unattended install) used to manually exercise every package end-to-end, and that hosts the Playwright E2E suite for CI.

Each provider folder groups that provider's package, its `*.Tests` project, and (if it has backoffice UI) a `Client/` frontend together — this is the layout to follow when adding a new provider.

## Commands

```bash
dotnet build
dotnet test
```

Run a single package's tests directly:

```bash
dotnet test GoogleSheets/Umbraco.Community.Automate.GoogleSheets.Tests
```

Run a single test with the standard xUnit filter, e.g.:

```bash
dotnet test GoogleSheets/Umbraco.Community.Automate.GoogleSheets.Tests --filter "FullyQualifiedName~AppendRowActionTests"
```

If a package has a `Client/` folder (currently only GoogleSheets), its frontend unit tests run from that folder:

```bash
npm ci
npm test
```

CI additionally runs a Playwright E2E pass against the Demo site (build the `Client/` bundle first, since it's gitignored generated output, then `npx playwright test` from the `Client/` folder with the Demo site running under `AUTOMATE_E2E_MODE=1`) — see `.github/workflows/ci.yml` for the exact steps to reproduce locally.

Package versions are derived entirely from git tags via MinVer — never hand-edit a `<Version>`. A release only ever starts by pushing a tag matching `<package-slug>-v<semver>` (e.g. `googlesheets-v1.0.0`); merging to `main` never releases anything. See `CONTRIBUTING.md` for the full release/tagging mechanics and how to add a new package to the scheme.

## Secrets

Never put real OAuth/API credentials into a git-tracked file (e.g. `appsettings.Development.json`), even locally. Use `dotnet user-secrets set ... --project Umbraco.Community.Automate.Demo` instead — this transparently overrides the tracked placeholder values at runtime in Development. Run `./SetupRepo.sh` / `.\SetupRepo.ps1` once per clone to install Lefthook's `pre-commit`/`pre-push` gitleaks hooks as a second line of defense.

## Architecture: the Automate SDK conventions

Every package plugs into `Umbraco.Automate.Core` (and, for OAuth providers, `Umbraco.Automate.OpenIddict`) using the same shapes. Understanding these once makes every package's code legible without re-reading each one:

**Actions** — a step an automation can run. Each is a triplet of files under `Actions/`:
- `<Name>Action.cs` — `[Action("alias", "Display Name", Group, Icon, ConnectionTypeAlias)]` on a class extending `ActionBase<TSettings, TOutput>`. `ExecuteAsync(ActionContext, CancellationToken)` pulls settings via `context.GetSettings<TSettings>()`, validates them (returning `ActionResult.Failed(exception, StepRunErrorCategory.X)` on bad input), does the work, and returns `Success(new TOutput { ... })`.
- `<Name>Settings.cs` — the action's configurable inputs.
- `<Name>Output.cs` — the action's result shape, available to later steps in the workflow.

**Connections** — reusable, authenticated links to an external service, referenced by `ConnectionTypeAlias` on actions:
- OAuth-based providers extend `OAuthConnectionTypeBase<TSettings>` (see `GoogleSheetsConnectionType`) and rely on `IOAuthCredentialsService` for token storage/refresh; `ProviderName` must match the name used when registering the OpenIddict client.
- API-key/non-OAuth providers extend `ConnectionTypeBase<TSettings>` directly and implement `ValidateAsync` themselves (see `SkodaConnectionType`), typically by making a lightweight authenticated call against the real API.

**Composers** (`IComposer.Compose(IUmbracoBuilder)`) wire a package into the Umbraco host at startup — binding `IOptions<T>` from `appsettings.json`, registering typed `HttpClient`s, and (for OAuth) registering the OpenIddict client. When a provider's credentials are simply absent from config, the composer should skip that provider's registration rather than let SDK calls throw during service resolution — pair this with `.ValidateOnStart()` so the *real* problem (missing config) surfaces clearly instead of a cryptic exception. See `GoogleSheetsComposer` for the guard pattern and the OpenIddict multi-provider registration (`SetProviderName`/`SetRegistrationId`, distinct from the generic `AddGoogle()` default, so a future Google Drive/Docs package can register its own client without colliding).

**Client frontends** (`Client/` folders) are Lit-based Umbraco backoffice extensions (custom property editors, icons) built with Vite, unit-tested with `web-test-runner`, and E2E-tested with Playwright against the Demo site. `Client/public/umbraco-package.json` declares the manifests Umbraco loads; built output goes to a gitignored `wwwroot`-equivalent that CI must build before E2E tests can find the registered custom elements.

**Central package management**: all NuGet package versions live in the root `Directory.Packages.props` — never add a `Version` attribute in a `.csproj`'s `PackageReference`. Each package's own `Directory.Build.props` carries package-specific metadata (`MinVerTagPrefix`, symbol package settings, authors/URLs) — copy that pattern (per `GoogleSheets/.../Directory.Build.props`) when scaffolding a new package rather than duplicating metadata in the `.csproj`.

## Contributing conventions

Branch names: `<type>/<short-description>` (e.g. `feat/google-sheets-clear-range`). Commit messages: `type(scope): Description` (e.g. `add(action): AppendRowAction — append a row of values to a sheet`); common types are `add`, `fix`, `refactor`, `test`, `docs`, `chore`. Keep PRs scoped to one action/feature where practical — a new action typically lands as its own PR with its own tests.
