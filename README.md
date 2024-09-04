# Web Application Documentation for Aura Systems as aTask.

## Introduction
Welcome to a web application using REST APIs. The application provide the simple service of operations with database.

### Technologie and Design
C#, OOP, DI, N-tiers, ASP.Net web API, Entity Framework (EF) and LINQ. <br />
HTML, Bootstrap, jQuery and AJAX.

### Packages
Microsoft.EntityFrameworkCore <br />
Microsoft.EntityFrameworkCore.SqlServer <br />
Microsoft.EntityFrameworkCore.Tools <br />

### To Install and Run
- Requires version c#(12) and .Net(8).
- Check packages installing.
- Change ConnectionStrings of your database in appsettings.json.
- In package manager console Run two commands.
    - => add-migration [name of migration].
    - => update-database.
- Make sure to be connected by internet to work CDNs(jQuery, Bootstrap) correctly and appear UI without any problems
   
### Test
After installing and running corectly, you can test by static data :
   - { Id = 1, Name = "John Doe", Activated = true },
   - { Id = 2, Name = "Mona", Activated = false },
   - { Id = 3, Name = "Ali", Activated = false },
   - { Id = 4, Name = "Osama", Activated = true }

