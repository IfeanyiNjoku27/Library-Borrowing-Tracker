# Library Borrowing Tracker

The solution contains the original ASP.NET Core API and a separate MVC web client. The client uses `HttpClient` for all six Books, Members, and Loans operations—18 API actions in total—and can attach an Apigee API key to published requests.

## Projects

- `Library-Borrowing-Tracker/` — existing REST API.
- `Library-Borrowing-Tracker.Client/` — Member 4 web client.
- `Library-Borrowing-Tracker.Client.Tests/` — request-contract tests for the 18 client actions and API-key transport.
- `apigee/` — OpenAPI document, Verify API Key policy, and provisioning templates.
- `docs/member4/` — client, portal, diagrams, evidence, and contribution handoff.

## Run locally

Start the API:

```sh
dotnet run --project Library-Borrowing-Tracker/Library-Borrowing-Tracker.csproj --launch-profile https
```

In a second terminal, start the client:

```sh
dotnet run --project Library-Borrowing-Tracker.Client/Library-Borrowing-Tracker.Client.csproj --launch-profile https
```

The default client configuration targets `https://localhost:7274/`, the API's HTTPS launch URL.

## Test

```sh
dotnet test Library-Borrowing-Tracker.Client.Tests/Library-Borrowing-Tracker.Client.Tests.csproj
```

The tests verify the HTTP method and route for all 18 actions, JSON Patch media types, typed patch values, header-based API keys, query-based API keys, and the no-key local-development path.

## Configure a published Apigee API

Do not commit an API key. Configure the client with environment variables or .NET User Secrets:

```sh
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:BaseUrl" "https://YOUR_APIGEE_HOST/library/"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKey" "YOUR_CONSUMER_KEY"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKeyLocation" "Header"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKeyName" "x-apikey"
```

Header delivery is recommended. Query delivery is also supported by setting `LibraryApi:ApiKeyLocation` to `Query` and choosing the query-parameter name configured in the Apigee policy.

See [`docs/member4/apigee-and-portal.md`](docs/member4/apigee-and-portal.md) for the complete Apigee and portal checklist.
