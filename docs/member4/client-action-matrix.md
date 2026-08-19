# Client Action Matrix

The `ILibraryApiClient` contract exposes 18 explicit methods. The MVC pages make each method accessible through a form or table action.

| Entity | Client method | HTTP request | UI entry point |
| --- | --- | --- | --- |
| Books | `GetBooksAsync` | `GET /api/books` | Books table |
| Books | `GetBookAsync` | `GET /api/books/{id}` | Retrieve-by-ID form |
| Books | `CreateBookAsync` | `POST /api/books` | Create form |
| Books | `UpdateBookAsync` | `PUT /api/books/{id}` | Replace form |
| Books | `PatchBookAsync` | `PATCH /api/books/{id}` | Change-one-field form |
| Books | `DeleteBookAsync` | `DELETE /api/books/{id}` | Delete form |
| Members | `GetMembersAsync` | `GET /api/members` | Members table |
| Members | `GetMemberAsync` | `GET /api/members/{id}` | Retrieve-by-ID form |
| Members | `CreateMemberAsync` | `POST /api/members` | Create form |
| Members | `UpdateMemberAsync` | `PUT /api/members/{id}` | Replace form |
| Members | `PatchMemberAsync` | `PATCH /api/members/{id}` | Change-one-field form |
| Members | `DeleteMemberAsync` | `DELETE /api/members/{id}` | Delete form |
| Loans | `GetLoansAsync` | `GET /api/loans` | Loans table |
| Loans | `GetLoanAsync` | `GET /api/loans/{id}` | Retrieve-by-ID form |
| Loans | `CreateLoanAsync` | `POST /api/loans` | Create form |
| Loans | `UpdateLoanAsync` | `PUT /api/loans/{id}` | Replace form |
| Loans | `PatchLoanAsync` | `PATCH /api/loans/{id}` | Change-one-field form |
| Loans | `DeleteLoanAsync` | `DELETE /api/loans/{id}` | Delete form |

All outgoing requests pass through one request builder. When a key is configured, that builder sends it using the selected header or query parameter. PATCH requests use `application/json-patch+json` and coerce Boolean, integer, date, and null values before serialization.
