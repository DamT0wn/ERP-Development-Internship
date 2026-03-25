# ERP-Development-Internship
ItemProcessor is a small ASP.NET Core MVC app that tracks items, lets you create them, and processes them into child output items. The data is stored in SQL Server using Entity Framework Core, and the item structure supports parent and child relationships so you can see hierarchy in both the list and detail screens.

## Steps to Run the Project

1. Open a terminal in the `ItemProcessor` folder.
2. Restore and build:
	```bash
	dotnet restore
	dotnet build
	```
3. Apply the database migration:
	```bash
	dotnet ef database update
	```
4. Run the app:
	```bash
	dotnet run
	```
5. Open the URL shown in the terminal (usually `http://localhost:5074`) and go to `/Item/Index`.

This project is set to .NET 8 and uses SQL Server Express on localhost by default. The connection string is in appsettings.json and currently points to localhost\\SQLEXPRESS with a database name of ItemProcessorDb. If your SQL Server instance name is different, update that connection string before running migrations.

To start from a clean setup, open a terminal in the ItemProcessor folder and run dotnet restore, then dotnet build. After that, run dotnet ef database update to create the database and apply the migration. When that completes, run dotnet run and open the URL shown in the terminal, usually http://localhost:5074.

Once the app is running, go to /Item/Index. You can create a new item, optionally attach it to a parent item, and process an item from the list. Processing marks the item as processed and can generate child items recursively depending on weight, so the hierarchy can grow through multiple levels.

The main pieces are organized in a straightforward way. The Item model and related view models are in the Models folder, database setup is in Data with AppDbContext and migrations, processing logic is in Services/ItemService, request handling is in Controllers/ItemController, and the Razor pages are in Views/Item.

If you need to reset the schema during development, you can drop the ItemProcessorDb database from SQL Server and run dotnet ef database update again. The migration in Data/Migrations will recreate the table structure, including the self-referencing foreign key for ParentItemId.
