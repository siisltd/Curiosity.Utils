# Changelog

## [2.0.0] - 2026-09-27

### Changed

- Dropped `netstandard2.1` support. Now multi-targeting `net9.0` and `net10.0`.
- **Breaking:** RPC client uses `System.Text.Json` instead of `Newtonsoft.Json`. Default options (`RabbitMqRpcClient.DefaultJsonSerializerOptions`) mimic Newtonsoft.Json behavior to stay wire-compatible: case-insensitive property names, public fields, numbers from strings, unescaped non-ASCII characters. Enums are still written as numbers; reading enums from strings requires custom options.

### Added

- `jsonSerializerOptions` parameter in `RabbitMqRpcClientFactory.CreateClient` to customize JSON serialization.

## [1.2.0] - 2026-02-13

### Changed

- Upgraded `RabbitMQ.Client` up to `6.8.1`.
- Upgraded `Newtonsoft.Json` up to `13.0.4`.

## [1.1.0] - 2023-01-29

### Changed

- Upgraded `RabbitMQ.Client` up to `6.4.0`.
- Upgraded `Newtonsoft.Json` up to `13.0.2`.

## [1.0.1] - 2022-01-12

### Added

- Added Rabbit's port to options.

## [1.0.0] - 2022-01-12

Package was released.
