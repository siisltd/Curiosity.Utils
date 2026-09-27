# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Curiosity.Utils is a collection of .NET NuGet utility libraries published under MIT license by SIIS Ltd. It provides reusable components for configuration, data access, email/SMS, file processing, messaging, notifications, hosting, and more.

## Build Commands

Solution-wide operations (build, test, pack, publish) should go through Cake for consistency with CI. Cake is installed as a local dotnet tool (`dotnet tool restore`).

```bash
# First time after clone — restore Cake dotnet tool
dotnet tool restore

# Full pipeline (clean, build, unit tests, integration tests)
dotnet cake

# Specific Cake targets
dotnet cake --target=Build --exclusive     # Build only
dotnet cake --target=UnitTests --exclusive # Unit tests only
dotnet cake --target=Pack --exclusive      # Pack NuGet packages
dotnet cake --target=GitHubReleases --exclusive --githubReleaseDryRun  # Preview release notes for packed packages (artifacts/release-notes)

# Single-project commands (for focused development)
dotnet test tests/UnitTests/Misc/Curiosity.Tools.UnitTests/Curiosity.Tools.UnitTests.csproj
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

## Architecture

### Module Pattern

Every feature area follows an **abstraction + implementation** pattern:

- **Base project** (`Curiosity.EMail`, `Curiosity.SMS`, `Curiosity.SFTP`, etc.) — defines interfaces and shared types
- **Implementation projects** (`Curiosity.EMail.Smtp`, `Curiosity.EMail.Mailgun`, `Curiosity.SMS.Smsc`, etc.) — concrete implementations with third-party dependencies

This pattern repeats across: Archiver, DAL, Email, FileData, Hosting, Localization, Notifications, RequestProcessing, SFTP, SMS.

### Dependency Flow

```
Curiosity.Configuration (base)
    └── Curiosity.Tools → used by most higher-level libraries
        ├── Curiosity.DAL.EF, Curiosity.DAL.Dapper
        ├── Curiosity.EMail → Smtp, Mailgun, UnisenderGo
        ├── Curiosity.SMS → Smsc, Iqsms
        ├── Curiosity.Notifications → Notifications.EMail, Notifications.SMS
        └── Curiosity.Hosting → Curiosity.Hosting.Web
```

`Curiosity.Tools` is the foundational utility library — most other projects depend on it.

### Source Layout

- `src/` — 37 library projects grouped by feature area
- `tests/UnitTests/` — xUnit tests with FluentAssertions and Moq
- `tests/IntegrationTests/` — integration tests (e.g., UnisenderGo email)
- `samples/` — sample applications demonstrating usage

## Key Technical Details

- **Multi-targeting:** `net9.0;net10.0` for libraries and tests, defined once as `CuriosityTargetFrameworks` in `Directory.Build.props` (projects use `<TargetFrameworks>$(CuriosityTargetFrameworks)</TargetFrameworks>`); samples target `net10.0` only
- **JSON:** `System.Text.Json` only. `Newtonsoft.Json` remains solely for the obsolete `TrimStringNewtonsoftConverter` in `Curiosity.Tools.Web` (removal tracked in #77) — do not add new usages
- **Nullable reference types:** enabled globally
- **Central package management:** `Directory.Packages.props` manages all NuGet versions — update versions there, not in individual .csproj files
- **Test framework:** xUnit + FluentAssertions + Moq + coverlet
- **CI/CD:** GitHub Actions on `ubuntu-latest`. `.github/workflows/build.yml` builds and tests PRs; `nuget_publish.yml` runs on every push to `master`: publishes packages whose `<PackageVersion>` is not on nuget.org yet, then creates a `<PackageId>.v<Version>` tag and GitHub release (notes = that version section of the package `CHANGELOG.md`). To release a package: bump `<PackageVersion>` and add a `## [x.y.z]` section to its CHANGELOG
- **Documentation:** MkDocs hosted on ReadTheDocs at https://curiosityutils.readthedocs.io/
