# Changelog

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