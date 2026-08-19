# Screenshot Checklist

Capture screenshots only after the relevant feature works. Keep account identity or resource context visible when required, but redact passwords, connection strings, consumer secrets, and complete API keys.

## Client

- Books page showing the GET-all table.
- Books POST, PUT, PATCH, and DELETE success messages.
- Members page showing the GET-all and GET-by-ID paths.
- Members POST, PUT, PATCH, and DELETE success messages.
- Loans page showing the GET-all and GET-by-ID paths.
- Loans POST, PUT, PATCH, and DELETE success messages.
- Published-client request showing the `x-apikey` header name; obscure the value.

## AWS

- ECR repository and image digest.
- ECS task definition, service, and running task.
- Task networking and health status.
- Cloud database resource and successful API connectivity.

## Apigee

- Deployed proxy and target endpoint.
- Verify API Key policy attached to the proxy request flow.
- API Product showing the correct environment, proxy, and resource paths.
- Three Developer records.
- Developer App showing its API Product association and approved key status; redact the key and secret.
- Request without a key returning an authorization failure.
- Request with an approved key returning an API response.
- Public portal home page, API catalog entry, SmartDocs reference, and app-registration path.

## Evidence naming

Use a stable sequence such as `01-client-books-list.png`, `02-client-book-create.png`, and `20-apigee-valid-key.png`. Add a one-line caption and the date captured to the final report.
