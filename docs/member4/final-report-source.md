# Library Borrowing Tracker — Final Report Source

## Executive summary

The Library Borrowing Tracker provides an ASP.NET Core REST API and a separate MVC web client for managing books, members, and loans. The backend uses DTOs, AutoMapper, a generic repository, Entity Framework Core, and SQL Server. The Member 4 client calls the API through `HttpClient` and exposes all six required actions for each entity, for 18 actions total. The published-client path supports a configurable Apigee API key without storing credentials in source control.

## Member 4 implementation

The client is an independent project in the solution. Its Books, Members, and Loans pages each provide list, retrieve-by-ID, create, PUT, PATCH, and delete controls. A typed `LibraryApiClient` centralizes the routes, JSON serialization, JSON Patch media type, error handling, request timeout, and API-key delivery. The client supports either a header or query parameter, with a header named `x-apikey` as the documented default.

Automated request-contract tests exercise all 18 service methods against a recording HTTP handler. The tests confirm HTTP methods, route paths, the PATCH content type, typed Boolean and numeric patch values, header-based keys, query-based keys, and local operation without a key. Four automated tests pass in the local Release verification run.

## Architecture

The intended published request path is browser → MVC client → Apigee proxy → API controllers → DTO/AutoMapper layer → repository → Entity Framework Core → SQL Server. Local development can bypass Apigee by changing only the client's base URL and leaving the key empty.

## Portal and API management

The repository includes an OpenAPI 3.0 document for all 18 operations and a Verify API Key policy that reads `request.header.x-apikey`. The Apigee handoff specifies a product constrained to the intended proxy, environment, and resource paths; three developer records; a developer app associated with the product; and a public integrated portal with SmartDocs generated from the OpenAPI document. These steps follow Google Cloud's Apigee documentation for API products, developers, developer apps, VerifyAPIKey, and integrated portals.

Live Apigee resources cannot be truthfully claimed until an authorized group member supplies the organization, environment, proxy, developer identities, and portal access. The final submission should include redacted screenshots of those live resources and both negative and positive API-key tests.

## Data model

Books contain catalog and availability data. Members contain borrower contact data. Loans reference a Book ID and Member ID and store borrow date, optional return date, and status. The current database migration does not enforce Book or Member foreign keys on loans, so referential integrity and availability synchronization remain backend hardening tasks.

## Security and configuration

API keys must be stored in .NET User Secrets, environment variables, or a managed secret store. They must never be committed to `appsettings.json`, screenshots, portal pages, or the report. Header delivery is preferred over a query parameter because query values may appear in browser history and network logs. The previously identified development database credential must be rotated and removed from tracked history before deployment.

## Verification and remaining evidence

The client and its tests build in Release configuration. The request-contract test suite passes. Completion of the full assignment still depends on a reachable database, representative data, live AWS deployment, live Apigee resources, portal publication, end-to-end browser/API testing, and contribution summaries from the other three group members.

## Conclusion

The Member 4 client closes the largest code-level gap in the original package: a consumer for all 18 required API actions. The remaining Member 4 work is environment-specific evidence—publishing the API through Apigee, registering real developers and an app, publishing the portal, collecting screenshots, and incorporating verified group contributions into the final report and presentation.
