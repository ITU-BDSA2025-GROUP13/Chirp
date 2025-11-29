# Changelog

## [5.5.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.4.0...v5.5.0) (2025-11-29)


### Features

* add auto focus to edit ([fe91dd1](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/fe91dd1061fa40b43fd124018165f88f3a16a20a))
* add character limit to post and reply ([cbd6fff](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/cbd6fff40c2eee311b09557f865be3dc654cfd5b))
* add charcount to post and reply ([329fce7](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/329fce7706259ea1166ddf0ca2f027aefcb89c41))
* add enter handler to all forms ([b473398](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/b47339861fefc5a0dd268ac54d7a40207769bc52))
* add forget me button for GDPR ([edfc6d9](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/edfc6d91eaadec59ff9829894bcd6c702fd4c557))
* add scroll persistence for redirects on deletion, reply, etc. ([857f498](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/857f498275aa04682b156d70011f3d2297511900))
* add scroll persistence to likes ([789498c](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/789498c3ee8eb719ae0b727b0d74e328cc2e3b36))
* improved ENTER behaviour for post form ([0d1428f](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0d1428f4b1314ece2722e1c7c319d513ee222100))
* toggle dark/light mode ([c9c46c4](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/c9c46c45d32030420399823ef301d6145d7e0391))


### Bug Fixes

* add proper failure handling for forget me ([cb4ca6b](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/cb4ca6ba4fa2470981017cefccc13d3566783b40))
* clean up queries in CheepRepo to fix bugs ([cd11de9](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/cd11de9f8d269e3e96116699e4bf564f70949cf0))
* get rid of reply bugs on user timelines ([0c26034](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0c26034bd0e72aedf92367bca94309c3c6d1c518))
* make GetFollowedUsers not the user's own name ([5eed285](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/5eed285b46d6d4f1bd8c8ea9469a48a72cc30671))
* make ico match the logo ([52bb379](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/52bb3792944d272547f368aa3c2355b6ceb018a3))
* make pagination on user timeline look the same as on public timeline ([f96fb8e](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/f96fb8ee2664b37079ba0a62641bc743be5777f8))
* make pagination start at 1, not 0 ([db139c1](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/db139c1002065d4576194f0133eee300523df1f1))

## [5.4.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.3.1...v5.4.0) (2025-11-25)


### Features

* add about me page for GDPR ([8a22f42](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/8a22f42fe5e7e0eba2e30de9dcb9374620d74553))
* add download user data functionality ([0644cbf](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0644cbf987dba9e725402cd1e60af355c22765ae))
* add liked posts to chirp to db ([5e90419](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/5e90419067e2f4e3fb5163bdd435e4cb22946436))
* add likes to cheep repo and service ([1ecd2be](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1ecd2be76bca2c25f7f3e9b1ff8e67c158f61d03))
* add likes to ui ([c2b7047](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/c2b704751ad07b4ced8f34f575ab6a43827a0eed))
* add visual indicators for unliking ([1bc453d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1bc453d6f284212849cb139fdc7ec831946fe494))
* adds new ChirpUser repo and service ([6bedfb9](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/6bedfb9d1a244488bb70334d3860e3a9228f1191))
* allow editing cheeps on backend ([99e60de](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/99e60de2ae9bec482d8b969ab24dee9292e4a5c6))
* backend now can handle following relations from post requests ([630599d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/630599df069932df42a185adca47d8106aa6f4e1))
* deletion button now turns into follow button on chepes not written by the authenticated users themselves ([1af9608](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1af9608bb9e8f6cb6d013ba854048780dcac8474))
* expose ui to edit cheeps ([afc910f](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/afc910fee77b8123aeaaccaa628eb78652f0bcbf))
* following now has UI elements and backend was updated slightly. ([4163694](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/416369482cdaabcf38c894024bfabe923decff11))
* make about me page display followed users ([e8cc56a](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/e8cc56a813523a540319e884dee73f014d747ee3))
* make user data listings collapsable ([6e1317d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/6e1317da98f0424980677b62bd553ef70e29cf74))
* QOL improvements for reply form ([ab79b12](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/ab79b124f87722a539d81c8e6dedcf6b98013f0c))
* reply to Cheep ([afead26](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/afead2664a90427de3c0945303706b5ead46e3bf))


### Bug Fixes

* adapt follow to new cheep component ([2accb27](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/2accb27bf9af0272431f9a3c5c68538aab03d074))
* database creation on windows ([c816871](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/c8168718595902b372026889e452fd9bd5c6a612))
* fixed pagniation error on own private timeline ([bd26606](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/bd26606511a43d002a978997cf44c1fb7decdc32))
* fixed redirect bug including handler in url, causing exceptions ([976a055](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/976a0558d574cc6247d7d398fca1a32b3a5ca59c))
* make replies also work in user timelines ([4c5e41c](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/4c5e41c6887d3f318ab4c13bb4aeb4b6e6534410))
* now displays own timeline correct after merge ([15871f3](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/15871f34e5be5ff46ed01ddfd319f23a5cdfd276))

## [5.3.1](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.3.0...v5.3.1) (2025-11-17)


### Bug Fixes

* make external login auto register ([52a2af2](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/52a2af2a6bfa810486d924aecc8698ae3e3bf9f7))
* next page in user timeline redirects to valid page ([6671e57](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/6671e57a241157fdb0f92d13ed9a4ca805f0706e))

## [5.3.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.2.0...v5.3.0) (2025-11-14)


### Features

* add backend support for deleting cheeps ([5686040](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/568604025b9a37d5dfe72195ca3316a150298951))
* add github oauth ([4a56124](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/4a5612492c9f08ca9dbd1be6ef5b0e070a0d8ccd))
* add login to Chirp.Web ([3b10c76](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/3b10c768896058762d74e87f1849bed866dfb928))
* Add ui for cheep post ([9aa85e8](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/9aa85e84e9452d0193e4d4e013cbc8fecd2297ec))
* extend ChirpDbContext to include identities ([0291a29](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0291a29b02a05e498b2247cf077046358bf7f56f))
* implement delete button in the ui ([7963081](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/7963081ff96457ea4613d0d1df566965357f79c0))
* Login as Helge and Adrian ([1557e82](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/1557e823f3588c26458f6f6cf00e75aa8b1f5e19))
* Post cheep ([a2d965d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/a2d965d515eff390ee2fffca55ed704b7bd924b4))
* support for danish usernames ([2924837](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/2924837225a2b05e5eefa4f594151ea4d6d936c1))


### Bug Fixes

* add padding and clean up logic ([801565d](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/801565d99ba2f3b3c35e8fc4d112c8805b74296b))
* errorhandling for cheeps over 160 chars ([2924837](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/2924837225a2b05e5eefa4f594151ea4d6d936c1))
* String primary key instead of int ([755f0e3](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/755f0e3adcc2ba6d91e2d2dcf917965503ce6081))
* Username unique ([755f0e3](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/755f0e3adcc2ba6d91e2d2dcf917965503ce6081))

## [5.2.0](https://github.com/ITU-BDSA2025-GROUP13/Chirp/compare/v5.1.0...v5.2.0) (2025-11-01)


### Features

* add taskbar to website layout ([0e12ecc](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0e12ecc41cab0d8195d7f2afeeef1e71f2fe2346))
* added pagination buttons ([23f55b9](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/23f55b935aada11348dbb49d4af8f9dc8cf47181))
* make logo link to homepage ([32b2bff](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/32b2bff05f60a88488e764cb6a887857a08c4046))
* migrate on startup ([0dd3ecc](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/0dd3ecc4fc400d4278a561cc8a1fb457aae648ec))
* move user timelines to /user route ([03504cf](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/03504cfde3e4d31df5b9d7f0a0f4ba5ca04671f7))
* restore migrations ([8134580](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/813458032aed50fc32d7c08de379bbb8b8f15b96))


### Bug Fixes

* fix pagination not working when specifing an author ([e973272](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/e9732724816b4211fa408d49966bec37012413e7))

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
