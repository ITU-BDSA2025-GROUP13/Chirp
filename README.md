# Chirp!
This is Chirp!, a social media created for the [ITU BSDA course](https://learnit.itu.dk/local/coursebase/view.php?ciid=2037)

# Running
Chirp! uses GitHub for authentication, and therefore requires you to have a [GitHub OAuth app](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/creating-an-oauth-app) if you wish to run Chirp! locally.
After creating an OAuth app, run the following command from the root of the project:
```
  dotnet user-secrets init --project src/Chirp.Web
  dotnet user-secrets set "authentication:github:clientId" "<YOUR_CLIENTID>" --project src/Chirp.Web
  dotnet user-secrets set "authentication:github:clientSecret" "<YOUR_CLIENTSECRET>" --project src/Chirp.Web
  dotnet run --project src/Chirp.Web
```
Alternatively you can find it hosted publically [here](https://bdsagroup13chirprazor.azurewebsites.net).

# License
Chirp! is licensed under [The 3-Clause BSD License](https://github.com/ITU-BDSA2025-GROUP13/Chirp/blob/main/LICENSE)
