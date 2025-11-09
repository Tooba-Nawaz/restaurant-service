# Restaurant & Menu Service (.NET 8) - SQL Server (Windows Authentication)

This project is configured to use your local SQL Server with Windows Authentication.

## Connection
Default connection string in appsettings.json:
`Server=localhost;Database=RestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;`

## Setup
1. Ensure SQL Server is running locally (Windows Authentication).
2. Restore packages:
   ```
   dotnet restore
   ```
3. Run the application:
   ```
   dotnet run
   ```
   The app will attempt to apply the included migrations automatically.
4. Open Swagger UI:
   http://localhost:5201/swagger/index.html

## If migrations fail
- Ensure your Windows account has access to SQL Server.
- Run migrations manually:
  ```
  dotnet tool install --global dotnet-ef
  dotnet ef database update
  ```

## Seed data
App seeds sample restaurants and menu items on first run.
