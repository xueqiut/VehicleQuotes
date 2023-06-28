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
*dotnet add package Microsoft.EntityFrameworkCore.Tools* // missing in the tutorial
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

# Install EF Core Driver for PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

# Install package to change capitalized camel case to snake case
dotnet add package EFCore.NamingConventions
```

# Connect to the database and perform initial app configuration
- Create DBContex
- Set up DI in Program.cs
- Create Migration scripts based on existing Models: `dotnet ef migrations add AddLookupTables`
- Create Tables by applying the Mirgration scripts: `dotnet ef database update`
https://www.endpointdev.com/blog/2021/07/dotnet-5-web-api/#connecting-to-the-database-and-performing-initial-app-configuration

# Entity Framework code-first, reverse engineer to create Models
https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/existing-database
`dotnet ef dbcontext Scaffold "Host=localhost;Database=vehicle_quote;Username=vehicle_quote;Password=password;" Npgsql.EntityFrameworkCore.PostgreSQL -o Models-test`

# Creating model entities, migrations and updating the database
- Create the POCO entities, which are simple C# classes with some properties. The classes become tables and the properties become the tables’ fields. Instances of these classes represent records in the database.
- Add DbSet to DbContext
- Create Migration script to apply changes to Database
- Use ef database update to create tables in postgresql

# Creating controllers for CRUDing our tables
- Create controller using dotnet-aspnet-codegenerator scaffolding tool

# Add Models with Navigation Properties
Navigation Properties and are how we tell EF Core that our entities are related to one another. These will result in foreign keys being created in the database to form either one-to-many or many-to-one relationship.
- Model
- ModelStyle
- ModelStyleYear

# Adding composite unique indexes
An index with a combination of more than one columns

# Adding controllers with custom routes
E.g. One Make can have many models, and Model does not make sense without specifying the Make. So, it does not make sense to get a model without specifiying the make.
To reflect this in URL Schemas for models. It should not be `/api/Models/{id}`, instead, it should be `/api/Makes/{makeId}/Models/{modelId}`

One controller works on a single HTTP Resource. Specify the Route at the controller level. Only specify the HTTP method, {id}, and query parameters at action level.

# Using resource models as DTOs for controllers
One side effect to have Controller works on the EF Model directly is that the API will be overly complicated. Because the resouce may reference other models and being referenced.

The POST API will look like below if the controller works with EF Model directly. It contains a lot of duplicated filds for foreigh and primay keys, and unnecessary nested object following the model structure.
```
{
  "id": 0,
  "name": "string",
  "makeID": 0,
  "make": {
    "id": 0,
    "name": "string"
  },
  "modelStyles": [
    {
      "id": 0,
      "modelID": 0,
      "bodyTypeID": 0,
      "sizeID": 0,
      "model": "string",
      "bodyType": {
        "id": 0,
        "name": "string"
      },
      "size": {
        "id": 0,
        "name": "string"
      },
      "modelStyleYears": [
        {
          "id": 0,
          "year": "string",
          "modelStyleID": 0,
          "modelStyle": "string"
        }
      ]
    }
  ]
}
```

The solution is to create a Resource Model (or DTO, Data Transfer Object, or View Model), which contains only the meaningful fields. However, the controller will need to translate the fields into object.
```
# meaningful fields
{
  "name": "string",
  "styles": [
    {
      "bodyType": "string",
      "size": "string",
      "years": [
        "string"
      ]
    }
  ]
}
```

The only purpose of DTO is to streamline the API contract of the endpoint by defining a set of fields that clients will use to make requests and interpret responses. It’s simpler than the actual database structure, but still captures all the information that’s important for the application. 

# Validation using built-in Data Annotations
# Validation using custom attributes

# Adding seed data for lookup tables

# Configuring the app via settings files and environment variables
Two default files
- appsettings.json: applies to all the environments
- appsettings.Development.json: applied only under development environments
Create more environment config if needed

The Environment is given by *ASPNETCORE_ENVIRONMENT* environment variable. It can be set as
- Development
- Staging
- Production: the default value
If ASPNETCORE_ENVIRONMENT=Staging, the appsettings.Staging.json file will be used

The setting can also be overwritten by environment variables, `SampleSetting=123 dotnet run`, which is useful in the DevOps Pipelines.

Read the config in the code, using `Microsoft.Extensions.Configuration`, with DI
```
// ...
+using Microsoft.Extensions.Configuration;

namespace VehicleQuotes.Services
{
    public class QuoteService
    {
        // ...
+       private readonly IConfiguration _configuration;

-       public QuoteService(VehicleQuotesContext context)
+       public QuoteService(VehicleQuotesContext context, IConfiguration configuration)
        {
            _context = context;
+           _configuration = configuration;
        }

        // ...

        public async Task<SubmittedQuoteRequest> CalculateQuote(QuoteRequest request)
        {
            // ...

+           if (response.OfferedQuote <= 0)
+           {
+               response.OfferedQuote = _configuration.GetValue<int>("DefaultOffer", 0);
+           }

            quoteToStore.OfferedQuote = response.OfferedQuote;

            // ...
        }

        // ...
    }
}
```