# Microservices Application Template

If you have found your way here, you have more than likely been asked by Regtransfers Ltd to complete a Coding Exercise as a part of your interview process. 

It’s great that you are eager to join our team. To make your life a bit easier we have provided you with a basic boilerplate project. 

## Forking
Please fork this project into your own github repository, this repository has been locked and cannot be committed to.

## WebMVC Project

ASP.NET Core 6.0.1 Web App (Model-View-Controller).

For ease of use, Startup.cs has been created and initiated in the Program.cs. RazorRuntimeCompilation has also been enabled, 

## Catalog.API Project

ASP.NET Core 6.0.1 Web API

For ease of use, Startup.cs has been created and initiated in the Program.cs.

- ApplicationDbContext inside the data folder is your Entity Framework Code First DatabaseContext.
- CodeFirst Migrations are enabled, any changes you make to the database can be implemented with 
  `Add-Migration` or `dotnet ef migrations add`
- ApplicationDbContextFactory enables these migrations to work
- ApplicationDbContextSeed seeds the DbContext with sample data, if you wish you can update this with more

## Catalog.Domain
This contains the domain objects for the Catalog.API, the Plate class has been generated for you here.

## BuildingBlocks
### EventBus
If you wish to utilise an event bus, we have preinstalled MassTransit for you and RabbitMQ is running in the background. IntegrationsEvents is a handy place to store your Message definitions, we have provided a base class called IntegrationEvent in which your message templates can inherit from.
The RabbitMQ control panel can be accessed on http://localhost:15672 username:guest & password:guest
More information on MassTransit can be found at: https://masstransit-project.com/

### WebHost
WebHostExtention enables code first migrations to be run when the project starts, this can be reused if you wish.

## Database
The database is a SQLServer 19 instance running in a docker container

The database should automatically create and seed itself thanks to some fancy boilerplate code.
Database: RTCodingExercise.Services.CatalogDb
Connection: Server=sqldata;Database=RTCodingExercise.Services.CatalogDb;User Id=sa;Password=Pass@word

you can connect to it via VisualStudio by adding it under the ServerExplorer DataConnections referencing `localhost,5433` as the server and choosing RTCodingExercise.Services.CatalogDb from the Database dropdown list, connecting with sql authentication with the username:sa & password:Pass@word or the same way through SQLServerManagementStudio

## Unit Tests
A xUnit test project has been added for Catalog.API under the tests folder called Catalog.UnitTests, and the Catalog.API and Catalog.Domain projects has been referenced.

A xUnit test project has been added for WebMVC under the tests folder called WebMVC.UnitTests, and the WebMVC and Catalog.Domain projects has been referenced.

If you add more Microservice projects, please follow the pattern and add a UnitTests project also. You are not restricted to using xUnit if you would prefer to use nUnit feel free to replace the test project with one of your choosing.

## Submitting
Please contact your Recruitment agency or us directly with a link to your forked repository.

Good Luck.





