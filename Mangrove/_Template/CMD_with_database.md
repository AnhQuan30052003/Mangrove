# Command generate data from database
## For: Visual Studio Code:
dotnet ef dbcontext scaffold "Data Source=ADMIN\SQLEXPRESS02;Initial Catalog=mangrove;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Data --force

## For: Visual Studio:
Scaffold-DbContext "Data Source=ADMIN\SQLEXPRESS02;Initial Catalog=mangrove;Integrated Security=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data -Force
