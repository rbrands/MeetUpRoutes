# MeetUpRoutes 

Starting from [template](https://github.com/staticwebdev/blazor-starter/generate) for an [Blazor WebAssembly](https://docs.microsoft.com/aspnet/core/blazor/?view=aspnetcore-3.1#blazor-webassembly) client application, a C# [Azure Functions](https://docs.microsoft.com/azure/azure-functions/functions-overview) and a C# class library with shared code.
The application is intended to be used together with MeetUpPlanner for gathering routes for roadbiking and gravel-biking.

## Getting Started

Create a repository from the [GitHub template](https://docs.github.com/en/enterprise/2.22/user/github/creating-cloning-and-archiving-repositories/creating-a-repository-from-a-template) and then clone it locally to your machine.

Once you clone the project, open the solution in [Visual Studio Code](https://code.visualstudio.com/) and follow these steps:

- In the **API** folder, copy `local.settings.example.json` to `local.settings.json`
- Press **F5** to launch both the client application and the Functions API app
- Update client to .NET 5 as described in https://docs.microsoft.com/en-us/aspnet/core/migration/31-to-50

_Note: If you're using the Azure Functions CLI tools, refer to [the documentation](https://docs.microsoft.com/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash) on how to enable CORS._

## Template Structure

- **Client**: The Blazor WebAssembly sample application
- **API**: A C# Azure Functions API, which the Blazor application will call
- **Shared**: A C# class library with a shared data model between the Blazor and Functions application

## Deploy to Azure Static Web Apps

This application can be deployed to [Azure Static Web Apps](https://docs.microsoft.com/azure/static-web-apps), to learn how, check out [our quickstart guide](https://aka.ms/blazor-swa/quickstart).

## Authentication and authorization

Authentication and authorization can easily be implemented as described in https://docs.microsoft.com/en-us/azure/static-web-apps/authentication-authorization. But to get the authentication/authorization tools provided with Blazor working an AuthenticationStateProvider is needed. An example - it's really easy - is provided by Anthony Chu, see https://anthonychu.ca/post/blazor-auth-azure-static-web-apps/ and on GitHub https://github.com/anthonychu/blazor-auth-static-web-apps. The AuthenticationStateProvider is used in this example to get the ClientPrincipal converted to a standard ClaimsPrincipal. AuthorizeView etc are working with this implementation. On server side see function GetUserDetails how the user identity can be leveraged.
