# AllTheBeans

AllTheBeans is a simple RESTful API that manages a collection of coffee beans, including a daily featured "Bean of the Day" and order processing. The solution is divided into layered projects (API, Application, Domain, Infrastructure) following clean architecture principles.

## Features

- **EF Core + SQLite** for persistence (with migrations). The app uses a local SQLite database file `AllTheBeans.db` (no external DB server needed). Initial data (a list of coffee beans) is seeded from a JSON file on first run.
- **Bean of the Day**: Each day, a random coffee bean is selected as the "Bean of the Day", ensured not to repeat the previous day's bean.
- **CRUD APIs for Beans**: Endpoints to create, read, update, and delete coffee beans.
- **Search**: Filter beans by name, country, or roast (`colour`) via query parameters.
- **Orders API**: Create orders with one or multiple bean line items. The total cost is calculated, and order details can be retrieved.
- **Idempotency**: Order creation supports an `Idempotency-Key` header to prevent duplicate order processing on repeated requests.
- **Swagger UI**: Interactive API documentation available (when running in Development) at `/swagger`.
- **Logging & Error Handling**: Middleware logs requests and handles exceptions, returning appropriate HTTP status codes (404 for not found, 400 for validation errors, 409 for duplicate requests, etc.).

## Running the API

1. **Build and Run**: Using the .NET SDK, build the solution and run the API project:
   ```bash
   dotnet build
   dotnet run --project src/AllTheBeans.Api
link: http://localhost:5236/swagger/index.html
