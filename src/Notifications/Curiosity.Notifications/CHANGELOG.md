# Changelog

## [1.2.4] - 2022-06-20

### Changed

- Changed log level of processed exception to `warn` instead of `error` at `NotificationChannelBase`. 

## [1.2.3] - 2022-06-17

### Added

- Added new method for registering channel.

## [1.2.2] - 2022-06-17

### Added

- Added new error code for invalid request data.

## [1.2.1] - 2022-03-09

### Added

- Added notification exception with error codes

## [1.2.0] - 2021-08-31

### Changed

- Notification infrastructure uses type of metadata class to match metadata and notification builders.

### Removed

- Removed `Type` property from `INotificationMetadata`.

## [1.1.1] - 2021-08-06

### Added

- Added extension method for registering notification builder in IoC.

## [1.1.0] - 2021-07-30

### Added

- Added `CancellationToken` support for sending notifications.

## [1.0.0] - 2021-03-15

First release.
