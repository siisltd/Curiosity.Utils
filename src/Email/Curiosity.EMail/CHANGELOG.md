# Changelog

## [1.4.0] - 2023-01-29

### Changed

- Upgraded `Microsoft`'s packages up to `6.*` versions.
- Upgraded `MimeKit`' up to `3.4.3`.

## [1.3.7] - 2023-01-12

## Changed

- Added support of cyrillic symbols to email regex pattern.

## [1.3.5] - 2022-05-19

## Added

- Added option for ignoring incorrect extra params type.

## [1.3.4] - 2022-05-19

## Added

- Guard for checking email params.

## [1.3.3] - 2022-03-09

## Change

- Upgraded `Curiosity.Tools` to `1.4.5`

## [1.3.2] - 2022-02-09

### Changed

- Improved email validation (added regex).

## [1.3.1] - 2021-12-15

### Added

- New email result for rate limit errors.

## [1.3.0] - 2021-12-02

### Added 

- Email sending result;

### Changed

- `IEmailLogger` returns `Response` class object.

## [1.2.0] - 2021-11-17

### Added

- Moved `TestLogEmailSender` from another package 

## [1.0.1] - 2021-07-30

### Added

- Added `IEMailExtraParams`.
- Added `CancellationToken` to sender.

## [1.0.1] - 2021-03-11

### Added

- Added changelog file.
- Added EMail validation attribute.
- Added method `ToEmailsList` to convert string EMails to collection.
