# Changelog

## [1.0.14] - 2021-07-29

### Changed

- Added cancellation token to `ProcessAsync` of `CuriosityWatchdog`.

## [1.0.13] - 2021-07-09

### Changed

- Using `Curiosity.Configuration.YAML` v1.0.7.

## [1.1.12] - 2021-06-04

### Added

- Added `Watchdog` for periodic tasks.

## [1.1.11] - 2021-04-08

### Added

- Added `ConfigureServices` overload with arguments passing to `CuriosityToolAppBootstrapper`.

## [1.1.10] - 2021-04-08

### Added

- Added separate method for `AppName` in logging configuration.

## [1.1.9] - 2021-04-07

### Changed

- Use `Curiosity.Configuration.YAML.1.0.6`.

## [1.1.8] - 2021-03-26

### Changed 

- Changed log level at `ThreadPoolMonitoringService`.

## [1.1.7] - 2021-03-26

### Changed 

- Changed name of `ThreadPoolMonitoringService` logger.

## [1.1.6] - 2021-03-25

### Added 

- Adding CLI args to app configuration.

## [1.1.5] - 2021-03-15

### Added 

- Explicitly installed `System.Data.Annotaitons` package.

## [1.1.4] - 2021-03-15

### Added 

- Added IoC extensions and initializer of measurer.
- Added performance measurer and stuck code manager to bootstrappers.

## [1.1.3] - 2021-03-05

### Added 

- Added configuration validation on application start.
