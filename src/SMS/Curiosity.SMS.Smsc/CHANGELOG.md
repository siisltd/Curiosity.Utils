# Changelog

## [2.0.0] - 2026-09-27

### Changed

- Dropped `netstandard2.1` support. Now multi-targeting `net9.0` and `net10.0`.
- Replaced `Newtonsoft.Json` with `System.Text.Json`. `SmsSentResult.ResponseJson` keeps the previous format.
- Upgraded `RestSharp` up to `114.0.0` (fixes known vulnerabilities).

### Fixed

- `SmsSentResult.SmsCount` was always empty: the `cnt` field of the SMSC response was not mapped since `RestSharp` switched to `System.Text.Json`.

## [1.4.0] - 2026-02-13

### Changed

- Upgraded dependencies.

## [1.3.2]

### Changed

- Added delivery error

## [1.3.1]

### Changed

- Fixed smsc delivery error detection

## [1.3.0] - 2023-01-29

### Changed

- Upgraded `RestSharp` up to `108.0.3`.

## [1.2.6] - 2022-07-0

### Added

- Added missed mapping error codes from smsc to sms error code

## [1.2.5] - 2022-03-09

### Added

- Added mapping error codes from smsc to sms error code

## [1.2.4] - 2022-03-09

## Change

- Upgraded `Curiosity.Tools` to `1.4.5`

## [1.2.3] - 2022-01-12

### Added

- Made sending SMS to Megafon and Tele2 more robust: reties sending sms without sender name if we got message denied error.

## [1.2.2] - 2021-12-07

### Added

- Added icon to package.

## [1.2.1] - 2021-12-07

### Changed

- Improved sending resposne from Smsc from `ISmsSender`.

## [1.2.0] - 2021-11-16

### Changed

- Upgraded `Curiosity.Tools` package.
- 
## [1.1.2] - 2021-11-16

### Changed

- Upgraded `Curiosity.Tools` package.

## [1.1.1] - 2021-10-18

### Added

- Added icon to nuget package.

## [1.1.0] - 2021-07-30
       
### Added

- Added `ISmsSender` with extra params.

## [1.0.0] - 2021-07-29

First release.
