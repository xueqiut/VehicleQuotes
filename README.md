# Web API for a hypothetical car junker business 
This is a sample Web API built with .NET 5, ASP.NET Core, and PostgreSQL Docker Container using VS Code
https://www.endpointdev.com/blog/2021/07/dotnet-5-web-api/

# Setting up the PostgreSQL with Docker
```
# Run Docker Container
docker run -d \
  --name vehicle-quote-postgres \
  -p 5432:5432 \
  -e POSTGRES_DB=vehicle_quote \
  -e POSTGRES_USER=vehicle_quote \
  -e POSTGRES_PASSWORD=password \
  postgres

# Access the Postgresql Container
- docker exec -it vehicle-quote-postgres psql -U vehicle_quote
- psql -h localhost -U vehicle_quote
```

# Install .NET 5
https://dotnet.microsoft.com/en-us/download/dotnet/7.0

# Create ASP.NET Web API with scaffold using dotnot cli
```
dotnet new webapi -o VehicleQuotes
dotnet new gitignore
```

# Initial Folder Structure
```
.
├── .gitignore
├── Controllers
│   └── WeatherForecastController.cs
├── Program.cs
├── Properties
│   └── launchSettings.json
├── README.md
├── VehicleQuotes.csproj
├── WeatherForecast.cs
├── appsettings.Development.json
├── appsettings.json
└── obj
    ├── VehicleQuotes.csproj.nuget.dgspec.json
    ├── VehicleQuotes.csproj.nuget.g.props
    ├── VehicleQuotes.csproj.nuget.g.targets
    ├── project.assets.json
    └── project.nuget.cache

- appsettings.json and appsettings.Development.json files which contain environment specific configuration values
- Program.cs contains our application’s entry point and bootstrapping logic
- VehicleQuotes.csproj contains project-wide configuration that the framework uses, like references, compilation targets, and other options
```

# Run the application
```
dotnet run

# Access the default website by visiting http://localhost:####/swagger
```

# Install the dependencies
```
# Install the ASP.NET Code Generator to auto-generate controller
dotnet tool install --global dotnet-aspnet-codegenerator

# Install the Entity Framework CLI tools to help create and apply database migrations
dotnet tool install --global dotnet-ef

# Install packages related to Entity Framework Core provides scaffolding, and detailed debugging page
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

# Install EF Core Driver for PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# Install package to change capitalized camel case to snake case
dotnet add package EFCore.NamingConventions
```

# Connect to the database and perform initial app configuration
- Create DBContex
- Set up DI in Program.cs
https://www.endpointdev.com/blog/2021/07/dotnet-5-web-api/#connecting-to-the-database-and-performing-initial-app-configuration

# Creating model entities, migrations and updating the database
- Create the POCO entities, which are simple C# classes with some properties. The classes become tables and the properties become the tables’ fields. Instances of these classes represent records in the database.
- Add DbSet to DbContext
- Create Migration script to apply changes to Database
- Use ef database update to create tables in postgresql

# Creating controllers for CRUDing our tables
- Create controller using dotnet-aspnet-codegenerator scaffolding tool