# Changelog

## [1.4.1] 

### Changed

- Add delivery error

## [1.4.0] - 2023-01-29

### Changed

- Upgraded dependencies.

## [1.3.6] - 2022-06-17

### Added

- Added `ISmsNotificationChannel`.

## [1.3.5] - 2022-06-17

### Fixed

- Throwing `NotificationException` from channel when sending failed with unexpected exception.

### Changed

- Replaced Task by ValueTask in `ISmsNotificationPostProcessor`.
- 
## [1.3.4] - 2022-03-09

### Added

- Throwing a notification exception when something failed in the sending chanel

## [1.3.3] - 2022-03-09

## Change

- Upgraded `Curiosity.Tools` to `1.4.5`

## [1.3.2] - 2021-12-13

### Fixed

- Fixed grammar mistake in exception message.

## [1.3.1] - 2021-12-07

### Changed

- Throwing error info from `ISmsSender` via exception when sending if failed.

## [1.3.0] - 2021-11-16

### Changed

- Upgraded `Curiosity.Tools` package.

## [1.2.1] - 2021-11-16

### Changed

- Upgraded `Curiosity.Tools` package.

## [1.2.0] - 2021-08-31

### Changed

- Notification infrastructure uses type of metadata class to match metadata and notification builders.

## [1.1.1] - 2021-08-06

### Added

- Added extension method for registering post processor to IoC.

## [1.1.0] - 2021-07-30

### Added

- Added `SmsChannel` with post processors.

## [1.0.0] - 2021-03-15

First release.
