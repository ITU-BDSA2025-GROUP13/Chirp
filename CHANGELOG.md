# Changelog

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
