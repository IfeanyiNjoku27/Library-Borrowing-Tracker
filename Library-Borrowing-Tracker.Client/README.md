# Member 4 Web Client

This ASP.NET Core MVC project is intentionally separate from the API. It does not modify the existing API controllers, models, DTOs, AutoMapper profiles, or configuration.

## Coverage

Each navigation area provides:

1. `GET /api/{entity}` — list all records.
2. `GET /api/{entity}/{id}` — retrieve one record.
3. `POST /api/{entity}` — create a record.
4. `PUT /api/{entity}/{id}` — replace/update a record.
5. `PATCH /api/{entity}/{id}` — replace one selected field using JSON Patch.
6. `DELETE /api/{entity}/{id}` — delete a record.

The pattern is implemented for Books, Members, and Loans.

## Configuration

| Setting | Purpose | Default |
| --- | --- | --- |
| `LibraryApi:BaseUrl` | API or Apigee proxy base URL | `https://localhost:7274/` |
| `LibraryApi:ApiKey` | Apigee consumer key | Empty |
| `LibraryApi:ApiKeyLocation` | `Header` or `Query` | `Header` |
| `LibraryApi:ApiKeyName` | Header or query name expected by the policy | `x-apikey` |
| `LibraryApi:TimeoutSeconds` | Request timeout | `30` |

Keep the key in User Secrets, environment variables, or a managed secret store. Never add it to `appsettings.json`.

## Local limitations

The current API uses SQL Server. A successful client startup does not guarantee that API requests will succeed unless the API has a reachable database and its existing connection configuration is valid. The web client surfaces API and connection errors without exposing an API key.
