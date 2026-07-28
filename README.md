# Employee Management System API

A controller-based ASP.NET Core Web API for managing employees and departments with ASP.NET Core Identity, JWT authentication, role-based authorization, and XAMPP MySQL/MariaDB.

## Features

- `Admin` and `User` roles
- Identity password hashing and MySQL/MariaDB-backed user/role storage
- JWT bearer authentication
- Admin-only employee and department create/update/delete operations
- Employee search, filters, and pagination
- Built-in ASP.NET Core OpenAPI document with Scalar UI
- Centralized RFC 7807-style error responses
- Automatic database schema creation and demo-data seeding at startup

## Requirements

- .NET 10 SDK
- XAMPP with its MySQL/MariaDB service running
- Optional: Visual Studio 2026 with **ASP.NET and web development**

## Quick start

From the repository root:

```powershell
dotnet restore
dotnet user-secrets init --project EmployeeManagement.Api
dotnet user-secrets set "Jwt:Secret" "replace-with-a-random-secret-at-least-32-bytes" --project EmployeeManagement.Api
dotnet run --project EmployeeManagement.Api
```

Start **MySQL** from the XAMPP Control Panel before running the API. Apache is not required because the API connects directly to the database on port `3306`.

The default connection matches a standard passwordless XAMPP installation:

```text
Server=localhost;Port=3306;Database=EmployeeManagementDb;User=root;Password=;
```

If your XAMPP `root` user has a password, configure it without editing committed settings:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=EmployeeManagementDb;User=root;Password=YOUR_PASSWORD;" --project EmployeeManagement.Api
```

Scalar opens at `https://localhost:7168/scalar/v1` when using the HTTPS launch profile. The OpenAPI document is available at `/openapi/v1.json` in Development.

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@employeeapi.com` | `Admin@12345` |
| User | `user@employeeapi.com` | `User@12345` |

These credentials are for local demonstration only. Override `SeedUsers:AdminPassword` and `SeedUsers:UserPassword` outside Development for any shared environment. The JWT secret is intentionally absent from the repository.

## Authentication

Log in:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@employeeapi.com",
  "password": "Admin@12345"
}
```

Use the returned token on protected requests:

```http
Authorization: Bearer <token>
```

## Endpoints

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/auth/login` | Anonymous |
| GET | `/api/employees` | Admin, User |
| GET | `/api/employees/{id}` | Admin, User |
| POST | `/api/employees` | Admin |
| PUT | `/api/employees/{id}` | Admin |
| DELETE | `/api/employees/{id}` | Admin |
| GET | `/api/departments` | Admin, User |
| GET | `/api/departments/{id}` | Admin, User |
| POST | `/api/departments` | Admin |
| PUT | `/api/departments/{id}` | Admin |
| DELETE | `/api/departments/{id}` | Admin |

Employee query examples:

```http
GET /api/employees?search=developer
GET /api/employees?departmentId=1
GET /api/employees?isActive=true
GET /api/employees?search=developer&departmentId=1&page=1&pageSize=10
```

`page` defaults to `1`, `pageSize` defaults to `10`, and the maximum page size is `100`.

## Database migrations

The initial migration is included. On Oracle MySQL it is applied automatically at startup. On MariaDB, startup uses EF Core schema creation to avoid an incompatibility in Oracle's provider migration-lock implementation. For future model changes:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef migrations add MigrationName --project EmployeeManagement.Api --startup-project EmployeeManagement.Api --output-dir Migrations
dotnet tool run dotnet-ef database update --project EmployeeManagement.Api --startup-project EmployeeManagement.Api
```

## Configuration

Supported configuration keys:

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | MySQL/MariaDB connection |
| `Jwt:Secret` | Signing key; required, minimum 32 bytes |
| `Jwt:Issuer` | Expected token issuer |
| `Jwt:Audience` | Expected token audience |
| `Jwt:ExpiryMinutes` | Token lifetime |
| `SeedUsers:AdminPassword` | Demo admin seed password |
| `SeedUsers:UserPassword` | Demo user seed password |

Use user-secrets during development and environment variables or a secret manager in deployed environments. For example, `Jwt__Secret` maps to `Jwt:Secret`.

## Verify

```powershell
dotnet build EmployeeManagement.slnx
dotnet list EmployeeManagement.Api package --vulnerable --include-transitive
```

## Deploy on Render with Aiven MySQL

The repository includes a multi-stage `Dockerfile` and a Render Blueprint.

1. Create an Aiven Free MySQL service and copy its service URI.
2. In Render, choose **New > Blueprint** and connect this GitHub repository.
3. Configure the prompted environment variables:
   - `ConnectionStrings__DefaultConnection`: convert the Aiven URI to a Connector/NET string, for example `Server=HOST;Port=PORT;Database=defaultdb;User=USER;Password=PASSWORD;SslMode=Required;`
   - `SeedUsers__AdminPassword`: a strong password with uppercase, lowercase, number, and symbol.
   - `SeedUsers__UserPassword`: another strong password.
4. Render generates `Jwt__Secret` automatically and deploys the Docker image.
5. Open `/health` to verify the service and `/scalar/v1` to test the API.

Render supplies the `PORT` environment variable automatically. The API binds to that port in cloud environments while retaining the local launch-profile ports.
