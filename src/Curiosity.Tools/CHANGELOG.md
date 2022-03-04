# Changelog

## [1.4.3] - 2022-03-04

### Changed

- Made constructor for `Response` classed opened.

## [1.4.2] - 2022-01-22

### Changed

- Added checking for log level to `PerformanceManager`. 

## [1.4.1] - 2022-01-22

### Changed

- Improved disposing in `StuckCodeManager`. 

## [1.4.0] - 2022-01-22

### Changed

- Added object polling to the `PerformanceManager` and `StuckCodeManager`.
- Made logger required for the `PerformanceManager` and `StuckCodeManager`. 

## [1.3.5] - 2022-01-21

### Added

- Added `DynamicSemaphoreSlim` - an improvement over System.Threading.SemaphoreSlim that allows you to dynamically increase and decrease the number of threads that can access a resource or pool of resources concurrently. 

## [1.3.4] - 2021-12-16

### Added

- Added new method to `TimeZoneHelper` to converting from user local time to UTC.

## [1.3.3] - 2021-12-07

### Changed

- Fixed namespace for `ExpressionExtensions`.

## [1.3.2] - 2021-11-29

### Added

- Added isStrict flag to overlapping method of DateTimePeriod

## [1.3.1] - 2021-11-18

### Added

- Added milliseconds trim to date time helpers

## [1.3.0] - 2021-11-16

### Added

- Moved `IAppInitializer` from `Curiosity.AppInitializer`.
- Moved `TimeZoneHelper` from `Curiosity.TimeZone`.

### Changed

- Changed namespace for response, errors models
- Made `SequenceCounter` thread safe.

## [1.2.0] - 2021-10-18
    
### Added

- Added `GenerateRandomKey` to `UniqueKeyGenerator`.  

### Changed

- Renamed `GenerateUniqueUrlPath` to `GenerateUniqueSequentialKey` in `UniqueKeyGenerator`.

## [1.1.0] - 2021-10-18
    
### Added

- Added `ExpressionExtensions` that allows to combine Expressions with boolean operators. 

### Changed

- Moved classes from `Curiosity.Tools.Models` namespace to `Curiosity.Tools`.

## [1.0.13] - 2021-09-20

### Added

- Added `UniqueUrlGenerator`.

## [1.0.12] - 2021-09-10

### Changed

- Replaced `Duration` field of `Period` be method `GetDuration`.

## [1.0.12] - 2021-09-03

### Added

- Added new methods to `Period` to check overlaps with separate `DateTime` or another `Period`.

## [1.0.11] - 2021-08-23

### Added

- Added new extensions to `Response` and `Response<T>`.

## [1.0.10] - 2021-08-14

### Added

- Added `CircularBuffer`.

## [1.0.9] - 2021-08-13

### Changed

- Convert `Permute` to extensions method.

## [1.0.8] - 2021-08-13

### Added

- Added `Permute` method for `IReadOnlyList<T>` and `IList<T>`.

## [1.0.7] - 2021-07-29

### Added

- Added page, error and response models.

## [1.0.6] - 2021-07-28

### Added

- Added `IsScrambledEquals` for `IReadOnlyList<T>`.

## [1.0.5] - 2021-04-15

### Fixed

- Fixed regex pattern which searches sensitive data in JSON.

## [1.0.4] - 2021-03-15

### Added

- Added performance measurer and stuck code manager.

## [1.0.3] - 2021-03-13

### Added

- Added Sensitive data protector

### Fixed

- Fixed transliteration for ned core 3

## [1.0.2] - 2021-03-11

### Added

- Added changelog file.
