---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group `13`
author:
- "Dimitri Alessandro Nielsen <dini@itu.dk>"
- "Karl Karl Theodor Ruby <krub@itu.dk>"
- "Lukas Shaghashvili-Johannessen <lush@itu.dk>"
- "Matthias Nyman Nielsen <mnni@itu.dk>"
- "Matthias Schoenning Nielsen <mscn@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

## Architecture — In the small
Below is a onion architecture diagram to illustrate the overall architecture of the _Chirp_ application. 
The diagram also illustrates dependencies, where the outer circles depend on the inner circles.

_Dependencies are illustrated as red arrows._

### Domain entities
In yellow is the center of the architecture as _Chirp.Core_.
This layer stores the most fundemental parts of the codebase. 
In this project _Chirp.Core_ stores the _Cheep_ and _ChirpUser_ domain model.

### Repository layer
In red is the infrastructure layer of the codebase. 
This layer is responsible for retrieving domain relevant information from the database. 

### Service layer
In orange is the service layer. 
This layer is responsible for translating the domain models into _DTO_'s (Data Transfer Object) and connect requests to the ui. 
This layer therefore acts as a binder between the infrastructure and the ui layer. 
When a user request is received the service layer handles that requests, 
retrieves information from the infrastructure layer, and translates the information received into DTO's.
These DTO's are then used by the UI to display information and data to the user.

### UI layer
In blue is the UI layer.
Here the UI is displayed to the user via `.cshmtl` pages. 
Here _page models_ sent user requests to the service layer and decide the state which to display for the user.
The state can change over the lifetime of the application, for example, when the user is logged in. 
Logging in changes the formatting of the pages, which the _page models_ are responsible for handling. 
## Architecture of deployed application

## User activities
This segment will focus on some of the typical scenarios and user journeys throughout the _Chirp_ application. 
First we will document what features are accessible to the user when unauthorized and authorized, 
and then go into more details about some of the most important features of the application.

### Activity diagram for unauthorized- and authorized users
Below is an activity diagram illustrating what actions the user can take when they are both authorized and unauthorized.
![Activity diagram for unathorized- and authorized users](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/77580ab9423b98d793567f2326f422c84f4c40b3/docs/diagrams/images/UserActivities.png)




### Follow User
Below is an activity diagram illustrating what happens when a user tries to follow another user. 
Following has the effect of adding the followed users cheeps to the users _My Timeline_. 
Following is therefore essential when two users wants to see what new cheeps the other posts 
![Activity diagram of a user following another user](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/384eeea077f20d2dfe4ee2889a95d1d529812cfd/docs/diagrams/images/Follow%20User.png){ width=50% }

### _Forget Me!_ (Deleting user)
The diagram below shows the actions performed when a user tries to delete their data.
This feature is called _Forget Me!_ in the _Chirp_ application, and can be performed under the `/user/<username>/about` endpoint.
It's worth noting that the _About Me_ site exists for every user, but the information
on the site is only loaded for the user who is authenticated on the platform, meaning,
_user1_ cant access the _About Me_ for _user2_. 
![Activity diagram of a user trying to delete their information](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/384eeea077f20d2dfe4ee2889a95d1d529812cfd/docs/diagrams/images/Forget%20Me.png){ width=50% }

When deleting user data, shown in the illustration after "User clicks forget me", an important design decision had to be made.
Normally in a lot of systems when the user deletes their data, they expect it to be deleted.
The effect of this can be optained by either soft deleting or hard deleting user data and information.
Before GDPR a lot of software used to just mark data as "deleted" in databases and never query them again. 
Now, because of GDPR, it is mandatory by law to always delete or anonymize user data when requested to do so, 
or when its no longer neccessary to keep that data stored[^userdata_deletion].

Hard deletes often creates a lot of problems behind the scenes, problems like syncing, irrecoverable data and database schema integrity compromise. 
For the _Chirp_ application there was the issue of what to do with [replies](#Activity_Reply). 
Since replies are linked with a _cheep_ parent-child relation, deleting a parent _cheep_ would result in all subsequent child _cheeps_ being deleted.
This is why we opted in for a deletion style more reminiscent of Reddit. 
In Reddit posts and replies made by the user arent deleted, but simple noted as _Deleted by user_.
With this method users wont loose their replies, simply because the author of the main _cheep_ decided to delete their post. 
An example of the visual effect of anomization of user data can be seen below.
![Here a user who has replied decided to delete their post. With a hard removal of posts, the user _Oliver_ would have lost his reply in the thread.](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/753500d78243fecda5fec20b5b2fe462fa829533/docs/images/DeletingUser.png)

### Login
When a user tries to log-in they have the option of either a application-scoped account or using Github as an external log-in service.
When a user logs in with Github, user data necessary for the application is automatically fetched. 
Information like a users Github username is used as their _Chirp_ username.
The user is therefore auto-redirected to the public timeline, when Github returns a valid authorization.
Below is a diagram of a typical scenario of a user logging into the _Chirp_ application. 
![Activity diagram of a user trying to login to the _Chirp_ application](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/753500d78243fecda5fec20b5b2fe462fa829533/docs/diagrams/images/Login.png)

### Reply {#Activity_Reply}
Below is an illustration of how a user would reply to another users _Cheep_. 
When designing replies it was chosen to use the same _Cheep_ entity as both a "root post" and the following replies to said post.
This method was chosen because we wished to design a _thread_ style of replies, like Reddit. 
Instead of only having one layer of replies, users could now reply to other peoples replies, and continue a _thread_ of replies.
Using the same entity for this, made both the UI and logic simple and DRY, by simply using recursion.
Below is a diagram of a typical scenario of a user replying to another user in the _Chirp_ application. 
![Activity diagram of a user replying to another users _Cheep_](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/753500d78243fecda5fec20b5b2fe462fa829533/docs/diagrams/images/Reply.png)

## Sequence of functionality/calls trough _Chirp!_

# Process

## Build, test, release, and deployment
### Versioning
Before the lecturers introduced us to semantic versioning and told us it was a requirement, we used CalVer[^calver].
CalVer was initially chosen, as it uses the calendar date for versioning, and seemed to be a good way to coordinate our weekly releases. [^release-retag]

Once we switched to semantic versioning we decided that it would make sense to automate this process.
The tool we used to automate this was [Release Please](https://github.com/googleapis/release-please) from Google.
Release Please continuously monitors the git history of a project through a GitHub action.
The action identifies commits which use the [conventional commit](https://www.conventionalcommits.org/en/v1.0.0/) standard and generates a release log based on changes in its own branch.
The action also opens a pull request, which when merged, merges the changelog into main and creates a new release with the changelog added as a description.
This helped give us an, and potential users, an overview over what has changed between releases.

In addition to this, Release Please also automatically computes the next version number based on the ```feat```, ```fix```, and ```feat!``` tags from conventional commit.
This was nice, as we didn't have to consider what our next release number should be.

One issue we faced with this was we ended up with a rather high major version (5.x.y).
The reason for this was our failure to consider what was actually a breaking change.
We followed the convention of tagging any breaking API change as a breaking change, which would make release please update the major version[^semver-lecture-notes].
However, these breaking API changes were often only breaking for internal APIs, for many major releases, no user-facing APIs changed.
We should not have considered these internal API changes as breaking, since, for the end user, these changes were not breaking.
What we should have considered a breaking change should be the switch from a **CLI** to a **web page**, and potentially **the addition of identity**.
This would mean that _Chirp!_ would be on **v3.x.y** or **v2.x.y**, depending on whether the addition of identity was considered breaking, not **v5.x.y**.

### Deployment
Whenever we deploy our code to GitHub, a number of GitHub Actions scripts will be run. These can be found the .github/workflow directory.
- coverage.yml: Runs a code coverage test and fails upon not reaching the set threshold
- format.yml: Runs dotnet format, which maintains a certain code standard in our code. These formatting commits have been attributed to our group member natthias
- main_bdsagroup13chirprazor.yml: Builds and deploys our code to our Azure Webapp instance, using our GitHub Secrets to access login information. This file was auto-generated by Azure and afterwards customized for our needs by a group member.
- release.yml: Builds and publishes our project to our GitHub repository, with builds for different operating systems.

### Linear git history
Initially, we did not enforce a linear git history as a requirement for our project, but it was a soft requirement to attempt to keep the history linear.
However, after [#87381](https://github.com/ITU-BDSA2025-GROUP13/Chirp/commit/873815545bb6ee80dd3c203ff9a895471815e3a6), we decided to enforce a linear history on the project through the git repo settings.

There are many reasons for enforcing a linear history.
For one, having a linear history makes managing and maintaining a project easier.
It is much easier to understand when a certain feature was added, as each commit is a patch, which cleanly applies atop the previous commit.

It also allows for external tools, such as ```git bisect``` to traverse the commit graph easier.
Since a linear history is just a line, it is much easier for ```git bisect``` to perform a binary search and find when a regression was introduced.
On the other hand, when the commit graph is a large interleaving of commits and merge commits, it can be difficult to perform a binary search.
This does not mean it is impossible to perform a binary search on a non-linear history, but the cognitive load on the developer is increased.

It also allows for easier code review.
Since each commit cleanly applies atop the previous, there is no extra noise generated by merge commits.
It is simply a set of patches to apply to the main branch.

Finally, one of the primary reason we used a linear git history was for external tooling.
We used an action called release-please for automated changelog generation.
This tool recommended a linear history to ease parsing of the commit graph[^release-please-linear].

The workflow for a linear git history is rather simple.
When a commit is ready to be merged with the main branch, you ```git rebase origin/main```, changes the base of the branch to be the current *HEAD* of the main branch.
This might introduce merge conflicts, which you resolve as normal.
Once the conflicts have been resolved, and the PR has been approved, it can be merged with ```git merge --ff-only```.

Having a linear git history is one way to manage a project which comes with it's set of benefits.
However, having a non-linear history has its own set of benefits.
Chiefly, some metadata is lost when a branch is rebased, since the commit the branch was initially based on has changed.
In addition to this, a non-linear history can make the history of long-lived feature branches more clear, however, since we used trunk-based development this was not a concern for us.

## Team work
### Trunk-based development

## How to make _Chirp!_ work locally
The get the application running locally either clone this repository or alternatively download the [latest release](https://github.com/ITU-BDSA2025-GROUP13/Chirp/releases/tag/v5.5.0) for your OS.
While _Chirp!_ will run without a GitHub OAuth client, _Chirp!_ will have degraded functionality if you don't have one.
To create a GitHub OAuth client follow [these instructions](https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_08/README_PROJECT.md#1b-oauth-via-github).

### Running from latest release
**On Windows:**
1. Unzip folder
1. Navigate to `chirp-main-<OS>-<architecture>`
1. Run the `Chirp.Web.exe` file. 

**On Linux & macOS**
1. Unzip folder
1. Navigate to `chirp-main-<OS>-<architecture>`
1. Run `./Chirp.Web`

### Running from repository
1. From the root folder of the project:

   `dotnet run --project src/Chirp.Web/`
1. (Optional) Release artifacts do not contain GitHub OAuth ClientID or ClientSecret, however these can be read from the environment variables ```$authentication__github__clientSecret``` and ```$authentication__github__clientId```[^chirp-port-local]

## How to run test suite locally
All tests, including PlayWright, E2E, Integration and Unit tests is stored in the `test` directory. PlayWright needs to get downloaded and installed first. Following is the steps to build and run the test suite (all done from the root folder of the project):
1. Build the project (needed for downloading PlayWright)

   `dotnet build`


1. Install PlayWright for tests

   ```pwsh test/Chirp.E2E.Tests/bin/Debug/net8.0/playwright.ps1 install --with-deps```

1. Run the project tests

   `dotnet test`

### Philosophy behind testing
For the project the group had a strict >=80% test coverage requirement for each feature. 
This requirement was set to avoid rollbacks and hotfixes, and instead focus on implementing safe and complete features. 
The requirement was set at 80, to keep the standard high, but also realisitic. 

To enforce the requirement a GitHub Action script was used, which ran the testsuites on every pull request to main. 
The script used [reportgenerator](https://github.com/danielpalme/ReportGenerator) to also generate a report in which we could better review what parts of the codebase was missing tests.


# Ethics

## License
We chose the [3-Clause BSD License](https://opensource.org/license/bsd-3-clause), which is a permissive, OSI approved license[^osi-approved], open source copyright license.
This license is slightly more restrictive than the MIT License or The 2-clause BSD License.
The license includes a non-endorsement stating that any derivative work may not use the name of the original work, or its authors as an endorsement of the derivative.

We felt that this license was a good choice for an educational project, as it preserves the permissive nature of the MIT License or the 2-Clause BSD License.
This allows for further contributions to the project through a fork, while protecting the original authors and project from both any implications of warranty or liability.

## LLMs, ChatGPT, CoPilot, and others

[^userdata_deletion]: [KILDE](https://ante.dk/blog/hvornaar-skal-persondata-slettes-ifoelge-gdpr/?utm_source=chatgpt.com)
[^calver]: [CalVer](https://calver.org/)
[^release-retag]: The initial release tag was deleted and tagged again using semver
[^semver-lecture-notes]: [Lecture slides on Semantic Versioning](https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_03/Slides.md#semantic-versioning)
[^release-please-linear]: [Release Please documentation about linear history](https://github.com/googleapis/release-please#linear-git-commit-history-use-squash-merge)
[^osi-approved]: [OSI approved licenses](https://opensource.org/licenses)
[^chirp-port-local]: Release artifacts run on port :5000, not :5273
