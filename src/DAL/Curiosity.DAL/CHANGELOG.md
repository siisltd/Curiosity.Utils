# Changelog

## [1.4.0] - 2023-01-29

### Changed

- Upgraded `Microsoft`'s packages up to `6.*` versions.

## [1.3.0] - 2021-11-02

### Added

- Added async version of `OnTransactionCompleted` event on transaction completed.
- 
## [1.2.0] - 2021-09-08

### Added

- Added `OnTransactionCompleted` to transaction and context.

## [1.1.1] - 2021-03-29

### Added

- Added optional `IsSensitiveDataLoggingEnabled` to `DbOptions`.

## [1.1.0] - 2021-03-29

### Added

- Added flag to enabling logging for a specific context.
- Added async methods for transactions.
- Added timeout to all methods of `ISqlExecutor`.
- Added cancellation token to all methods of `ISqlExecutor`.
- Added cancellation token to `BeginTransactionAsync` at `DataContext`.

### Changed

- Renamed `ReplicaConnectionString` to `ReadOnlyConnectionString`.
- Renamed `CreateReplicaContext` to `CreateReadOnlyContext`.
- Split `ICuriosityDataContextFactory` into `ICuriosityDataContextFactory` and `ICuriosityReadOnlyDataContextFactory`.
- Replaced foreach on for loop at `SafeRollbackTransactions`

## [1.0.2] - 2021-03-27

### Fixed

- Added Changelog and package icon.
