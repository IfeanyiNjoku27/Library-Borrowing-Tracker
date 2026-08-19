# Apigee Product, Developer App, and Portal Handoff

This guide separates reproducible repository work from organization-specific cloud work. Do not claim the live setup is complete until the required Apigee resources are visible and the positive/negative key tests pass.

## Inputs required from the group

- Apigee organization and environment.
- Deployed API proxy name, public host, and proxy base path.
- Authorized Google Cloud/Apigee account.
- Real name, lowercase email, and username for each of three developers.
- Portal name, public URL, logo/branding choices, and access decision.
- A reachable API target URL and representative test data.

Never put access tokens, consumer keys, consumer secrets, database credentials, or full private URLs in commits, screenshots, the report, or the slide deck.

## 1. Add API-key verification to the proxy

1. Copy `apigee/policies/Verify-API-Key.xml` into the proxy bundle or create an equivalent **Verify API Key** policy in the Apigee editor.
2. Attach it to the ProxyEndpoint request PreFlow so all product-controlled routes are checked before the request reaches the API.
3. Confirm that the policy reads `request.header.x-apikey`.
4. Deploy the new proxy revision to the intended environment.
5. Test the proxy without a key and retain the authorization-failure evidence.

Apigee's VerifyAPIKey policy validates a consumer key and makes app/developer/product metadata available after verification. The policy reference supports header or query variables; this project standardizes on the `x-apikey` request header. See [Verify API Key policy](https://docs.cloud.google.com/apigee/docs/api-platform/reference/policies/verify-api-key-policy).

## 2. Create the API Product

Create a product named `library-borrowing-tracker`, using `apigee/templates/api-product.json` as a reviewed starting point.

- Replace `YOUR_ENVIRONMENT` and `YOUR_PROXY_NAME`.
- Keep the proxy, environment, and resource-path restrictions. Omitting restrictions can grant broader access than intended.
- Confirm the six Books, six Members, and six Loans operations are reachable through the product.
- Use automatic approval only if it matches the course requirements; otherwise change the approval type before creation.

The Apigee API supports creating products and limiting access by environment, proxy, and resource path. See [create an API product](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.apiproducts/create) and the [API product resource](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.apiproducts).

## 3. Create three Developers

Create three developer records from the group-provided identities. Duplicate `apigee/templates/developer.json` locally for each developer, replace every sample value, and do not commit the filled files.

Required fields include email, first name, last name, and username. Use lowercase email addresses. See [create a developer](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.developers/create) and the [developer resource](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.developers).

## 4. Create the Developer App

1. Choose one of the three real developers as the app owner.
2. Create the app using `apigee/templates/developer-app.json`.
3. Associate it with the `library-borrowing-tracker` API Product.
4. Confirm the credential and product status are approved.
5. Copy the consumer key into a secret manager or .NET User Secrets. Do not save the consumer secret in the client because only the API key is required for this policy.

See [create a developer app](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.developers.apps/create) and the [developer app resource](https://docs.cloud.google.com/apigee/docs/reference/apis/apigee/rest/v1/organizations.developers.apps).

## 5. Configure and publish the portal

1. Create or open an integrated portal in Apigee.
2. Add branding, navigation, contact information, and terms appropriate for a public course-project portal.
3. Add an API entry and import `apigee/openapi.yaml`.
4. Replace the placeholder server URL with the deployed proxy base URL before publication.
5. Render the reference documentation with SmartDocs and verify that Books, Members, and Loans each show six actions.
6. Enable the app-registration path required by the assignment and confirm a registered user can associate an app with the product.
7. Publish the portal, open the public URL in a signed-out browser, and capture the required evidence.

Apigee's portal workflow supports creating pages, managing navigation, publishing API documentation, and surfacing app registration. See [build an integrated portal](https://docs.cloud.google.com/apigee/docs/api-platform/publish/portal/portal-steps), [publish APIs with SmartDocs](https://docs.cloud.google.com/apigee/docs/api-platform/publish/portal/publish-apis), and [surface app registration](https://docs.cloud.google.com/apigee/docs/api-platform/publish/creating-apps-surface-your-api).

## 6. Configure the client for the published API

Store the key outside source control:

```sh
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:BaseUrl" "https://YOUR_APIGEE_HOST/library/"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKey" "YOUR_CONSUMER_KEY"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKeyLocation" "Header"
dotnet user-secrets --project Library-Borrowing-Tracker.Client set "LibraryApi:ApiKeyName" "x-apikey"
```

Restart the client after changing secrets.

## 7. Acceptance tests and evidence

Run and capture all of the following, with sensitive values redacted:

1. No key → authorization failure at Apigee.
2. Invalid key → authorization failure at Apigee.
3. Approved key → successful `GET /api/books` response.
4. Approved key → one representative POST, PUT, PATCH, and DELETE through the proxy.
5. Client configured for Apigee → successful list operation for Books, Members, and Loans.
6. Product screen → correct environment, proxy, and resource paths.
7. Developer list → three real developer records.
8. App screen → product association and approved key status, with secrets obscured.
9. Public portal → home page, API catalog, SmartDocs, and app-registration route.

## Current blocker

The repository artifacts are ready, but live resource creation cannot be completed or evidenced without the organization/environment/proxy values, three real developer identities, and authorized portal access. Send those details through the team's approved private channel; do not post credentials in group chat.

