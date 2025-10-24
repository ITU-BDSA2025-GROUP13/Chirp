# Changelog

## [5.1.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.0.0...v5.1.0) (2025-10-24)


### Features

* Added length constraint to EF Core and SQLite Database. Triggers SQLiteException on Cheeps over 160 characters. ([bf92085](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/bf9208504a4b6baf8be568137523636e60500a9b))
* proper handling of DB path ([b3dffc9](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/b3dffc98ee849327d5c11000f77bc074a608bcf0))
* Split repo's into two ([d2f1f0a](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/d2f1f0ae8cf172f200e62fd881f7babc89943895))


### Bug Fixes

* add type to new var ([3baaa24](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/3baaa24712e69678e55049371e0fc1819646f932))
* Fixed wrong config settings for Azure deployment ([8caf525](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/8caf5255f48e82b5c33571b55ac0ebaa39746afe))
* fixes both azure AND local database creation/migrations ([420c213](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/420c213f5333f95f33588ad7297cf8b2a509412c))
* remove migration, DbInitializer and cshtml files from coverage cicd ([e96ff37](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/e96ff37f80a5c3502745ab0ed903fe1638ba969f))
* removed unused column from DB ([40ca1be](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/40ca1be6127c102c46fe519855ca87473d9149ec))
* restore Chirp.Web reading CHIRPDBPATH ([95d0efd](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/95d0efde8e0fed4d5adf82060b3f4f7677070338))
* update service to take a Irepository insted of making one ([c699ba8](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/c699ba86144bc63d7bfafbdd1e7689ad16f0aaec))

## [5.0.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v4.0.1...v5.0.0) (2025-10-20)


### ⚠ BREAKING CHANGES

* changes everything lmao
* CheepService now returns CheepDTOs instead of Cheeps
* add author as identity
* switching to author and message changes the database schema, changing the dbfacade api
* you must now interface with the repository instead of the database

### Features

* add author as identity ([71a239c](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/71a239cfb891b4bead387d8107376d194d649629))
* add dependency injection ([8f08aa7](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/8f08aa7142aa89c9ccbb8d942e9ebc0d99bffa96))
* add DTO to the application layer ([0fa0f80](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0fa0f809c8b4b39fe91234b39599689297bc0645))
* migrate to EF Core ([b72a03e](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/b72a03e336d029ad4f06c060690a7961533c4ff4))


### Bug Fixes

* automatically applies database migrations to Chirp.Razor service, even when using 'dotnet publish' ([466a4dc](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/466a4dc14b6b7748308d60ec323d32baaa80d646))
* explicitly close filestream in dbfacade ([71a239c](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/71a239cfb891b4bead387d8107376d194d649629))
* stops multiple .runtimeconfig.json files from being generated ([b0a6c02](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/b0a6c0223de3bd191c46b17b988c7699a64c91f2))


### Code Refactoring

* add repository pattern ([1dc490f](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1dc490ff6d32843e9ac398467ecd63fafa490d73))

## [4.0.1](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v4.0.0...v4.0.1) (2025-10-03)


### Bug Fixes

* add semi colon to sql ([85e3514](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/85e3514cdee6c18d11da87a09b6726bced7ffaa5))
* remove test build instructions for sqlite test ([0df1155](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0df1155c171e1b35a6460b30b435658b5d28814b))
* restore support for running on windows ([f0fa015](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/f0fa015fe5445403f412038d719abc4942c3268f))

## [4.0.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v3.0.1...v4.0.0) (2025-10-01)


### ⚠ BREAKING CHANGES

* add SQLite database support
* This breaks WebServer and WebServer.Tests

### Features

* add read from user feature to DBFacade ([d6770fd](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/d6770fda3ee952227c65c2e298524ba06d8e8fc5))
* add SQLite database support ([308ec59](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/308ec5934a37ecd47f3d0285ec46b3d54150e7da))
* Added Pagination to Razor pages ([cf8f64f](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/cf8f64f8d4100bbaf931452b9f75f85242909667))
* read db path from env ([bd28502](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/bd28502fa443160494474f9430128671b56b5ff0))


### Bug Fixes

* Add default path to dbfacade to make test use memory db and formatting ([d4bda0a](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/d4bda0a7200cb5d789f3b0167474a12b1d1ece1f))
* Changed pagination to the correct format of "{author}?page={num}" ([f5816a6](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/f5816a61cfafb467b1e84e0fcaec61e85c78e7be))
* create sql schema and file if it doesn't exist ([bce785b](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/bce785b8bbea606e9703e3e986fad896f0a88e3a))
* enforce unique usernames in db schema ([1ec8caa](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1ec8caa54605396c5b7a71c8f2b567f9bef17e45))
* make Chirp.SQLite a library ([948c26d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/948c26d8ec205302b7163d3b0b555f32a919ee85))
* make shell script use absolute paths ([6a24709](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/6a24709111847ca4e4219c50e121e1024a898ab6))
* Remove purely alpha constraints, allowing alphanumeric usernames ([4556376](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/4556376867da8b3faf89d3f96d08b22eebcb6e76))
* remove static from methods ([702db06](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/702db06fe5528048a3ffaa7afcd59a8f99fa9cab))
* show 32 cheeps per page ([fe64de5](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/fe64de564e5daed1da60cc79fa9265eba2192f7b))


### Code Refactoring

* restructured the project for Chirp razor app and SQLite database ([9420dd0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/9420dd0ac02a33cd5440d62ae605b732165a7345))
