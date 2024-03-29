# Changelog

## [1.4.1] - 2023-01-30

### Changed

- Set event only after executing request completion actions.

## [1.4.0] - 2023-01-29

### Changed

- Upgraded `Microsoft`'s packages up to `6.*` versions.

## [1.3.1] - 2022-06-10

### Added

- Added virtual method for fetching free workers to dispatcher.

## [1.3.0] - 2022-06-09

### Changed

- Made options parameters generic for dispatcher, bootstrapper and worker. 
- Made `IsBusy` virtual in `WorkerBase`. 

## [1.2.3] - 2022-06-07

### Added

- Added virtual method to process request after worker completion.

## [1.2.2] - 2022-06-07

### Removed

- Removed properties from `IRequestProcessingEvent`.

## [1.2.1] - 2022-06-07

### Changed

- Renamed `HandleDbEventReceived` -> `HandleEventReceived`.

## [1.2.0] - 2022-06-02

### Added

- Added passing worker name to `CreateWorkerParams` method.

## [1.1.0] - 2022-04-22

### Removed

- Removed filtration from package.

## [1.0.0] - 2021-03-15

First release.
