# Web Application Documentation for Task Management System as aTask.

## Introduction
Welcome to a web application using REST APIs.
This project is an ASP.NET Core API that utilizes static files (HTML, CSS, JS) for the front-end, rather than Swagger. Note that the front-end does not showcase all the project's requirements and features; it's a simple demonstration of data binding and does not fully reflect the backend work. Additional backend requirements can be set up, as the necessary foundation has already been established. To enable Swagger, please check the configurations in the "Program.cs" and "launchSettings.json" files.

### Technologie and Design
C#, OOP, DI, N-tiers, ASP.Net web API, Entity Framework (EF) and LINQ. <br />
HTML, Bootstrap, CSS, JavaScript.

### Main Packages
Microsoft.EntityFrameworkCore <br />
Microsoft.EntityFrameworkCore.SqlServer <br />
Microsoft.EntityFrameworkCore.Tools <br />
Microsoft.AspNetCore.Authentication.JwtBearer <br />

### To Install and Run
- Requires version c#(12) and .Net(8).
- Check packages installing.
- Change ConnectionStrings of your database in appsettings.json.
- In package manager console Run two commands.
    - => add-migration [name of migration].
    - => update-database.
- Make sure to be connected by internet to work CDNs(jQuery, Bootstrap) correctly and appear UI without any problems
   


