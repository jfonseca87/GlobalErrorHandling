# GlobalErrorHandling

A .NET 9 Minimal API that demonstrates a **global exception handling middleware** using the `IProblemDetailsService` to return structured error responses conforming to **RFC 7807** (Problem Details). The `GlobalErrorHandler` middleware catches all unhandled exceptions, logs them, maps them to appropriate HTTP status codes, and returns a JSON Problem Details response.

The API exposes several endpoints to test different error scenarios: `/weatherforecast` (normal operation), `/throwexception` (triggers a generic 500 error), `/unauthorized` (returns a manual 401 Problem Details response), and `/badrequest` (validates a POST body and returns 400 with field details). The middleware can be extended by adding more exception type mappings in the `MapException` method.

## Technologies

- .NET 9
- C#
- Minimal API
- Swagger / OpenAPI
- `IProblemDetailsService` (RFC 7807)

## Features

- Global exception handling middleware
- Automatic `ProblemDetails` JSON responses
- Exception type mapping (UnauthorizedAccessException -> 401, default -> 500)
- Structured logging with `ILogger`
- Test endpoints for 500, 401, and 400 scenarios

## How to Run

```bash
dotnet run --project GlobalErrorHandling.API
```

Test the endpoints:
- `GET /weatherforecast`
- `GET /throwexception`
- `GET /unauthorized`
- `POST /badrequest` with a JSON body `{ "name": "", "age": 0, "email": "" }`
