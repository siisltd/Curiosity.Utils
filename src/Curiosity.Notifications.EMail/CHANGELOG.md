# Changelog

## [1.3.2] - 2022-06-17

### Fixed

- Throwing `NotificationException` from channel when sending failed with unexpected exception.

### Changed

- Replaced Task by ValueTask in `IEMailNotificationPostProcessor`.

## [1.3.1] - 2022-03-09

## Change

- Upgraded `Curiosity.Tools` to `1.4.5`

### Changed

## [1.3.0] - 2021-12-02

### Changed

- Supports email result from email sender;

## [1.2.0] - 2021-08-31

### Changed

- Notification infrastructure uses type of metadata class to match metadata and notification builders.

## [1.1.0] - 2021-07-30

### Added

- Added `IEMailExtraParams` implementation.
- Added `CancellationToken` to sender.
- Added `IEMailNotificationPostProcessor`.

## [1.0.0] - 2021-03-15

First release.
