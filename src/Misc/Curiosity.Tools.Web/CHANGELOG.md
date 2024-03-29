# Changelog

## [1.3.1] 

### Added

- Cancellation exception handling, write a message to WARN log

## [1.3.0] - 2023-11-28

### Changed

- Upgraded `HtmlSanitizer` up to `8.0.723`.

## [1.2.0] - 2023-01-29

### Changed

- Upgraded `Microsoft`'s packages up to `6.*` versions.
- Upgraded `HtmlSanitizer` up to `8.0.645`.
- Upgraded `Flurl.Http` up to `3.2.4`.

## [1.1.9] - 2022-07-15

### Changed

- Modified `ModelBinderProvider` classes to use `ILoggerFactory` from context services.

## [1.1.8] - 2022-03-12

### Changed

- Made `PathBase` in `ReverseProxyOptions` optional.

## [1.1.7] - 2021-11-29

### Changed

- Removed extra slash in request logger

## [1.1.6] - 2021-11-18

### Changed

- Improved enum writer to swagger descriptor

### Added

- Added query string to request logger

## [1.1.5] - 2021-11-16

### Added

- Added null reference annotations.

## [1.1.4] - 2021-11-10

### Added

- Added `SwaggerDescriptor`.

## [1.1.3] - 2021-07-29

### Added

- Added `DelimitedArrayModelBinder`.

## [1.1.2] - 2021-07-29

### Added

- Added `TrimStringNewtonsoftConverter`.

### Changed

- Renamed `TrimStringConverter` to `TrimStringSystemJsonConverter`.

## [1.1.1] - 2021-04-15

### Changed

- Printing `ViewBag.ErrorText` as raw html at `AjaxError` view.

## [1.1.0] - 2021-03-27

### Changed

- Made `MVCBaseController` abstract.

### Removed

- Replaced `SiteMapController` by `SiteMapBuilder`.

## [1.0.11] - 2021-03-26

### Added

- Added key ring dir existence check.

## [1.0.10] - 2021-03-26

### Added

- Added ASP.NET Core data protection.

## [1.0.9] - 2021-03-20

### Added

- Added HTML Sanitizer

## [1.0.8] - 2021-03-13

### Added

- Added Sensitive data protector to request logging middleware.
- Added extension methods for middleware.

## [1.0.7] - 2021-03-10

### Added

- Added json converter for trimming strings.

## [1.0.6] - 2021-03-08

### Fixed

- Fixed localization.

## [1.0.5] - 2021-03-08

### Changed

- Upgraded `System.ComponentModel.Annotations` to `v5.0.0`.

## [1.0.4] - 2021-03-08

### Added

- Added MVC base controller.

### Changed

- Changed behavior of exception handle middleware from `.NET Core 2.1` to `.NET Core 3.1`.
