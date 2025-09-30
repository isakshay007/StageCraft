# StageCraft – Theatre Content Management System 🎭  

## Overview  
StageCraft is a **theatre content management system** built using **ASP.NET Core MVC** and **Entity Framework Core (MySQL)**. The platform allows theatre admins to manage productions, scheduling, roles, and comments efficiently. With role-based authentication and a modern UI powered by Razor Pages, the system reduces manual scheduling and enhances collaboration across theatre stakeholders.  

---

## Features  
- **Role-Based Authentication** using Microsoft.AspNetCore.Identity.  
- **Production Management** with CRUD operations for shows, playwrights, runtimes, and schedules.  
- **Comment System** with AJAX-based add/delete, threaded discussions, and modal rendering.  
- **Activity Logs & Analytics** tracking logins, actions, and admin statistics.  
- **Dynamic Spotlight Section** showcasing current productions with posters.  
- **Responsive UI** built with Razor Pages and Bootstrap.  
- **AppDbContext** managing 4+ entities and migrations with EF Core.  

---

## Tech Stack  
- **Frontend:** Razor Pages, Bootstrap, HTML5, CSS3  
- **Backend:** C#, ASP.NET Core MVC  
- **Database:** MySQL with Entity Framework Core  
- **Authentication:** Microsoft.AspNetCore.Identity  
- **Other:** X.PagedList for pagination, AJAX for comment operations  

---

## Project Structure  
```
StageCraft/
│
├── Controllers/            # Account, Admin, Comments, Productions, Home
├── Models/                 # Production, Comment, ActivityLog, ApplicationUser, etc.
├── ViewModels/             # Strongly typed models for Login, Register, Stats, etc.
├── Views/                  # Razor Pages for UI
├── Data/AppDbContext.cs    # EF Core context with migrations
├── Migrations/             # Database migrations history
├── Program.cs              # ASP.NET Core entry point
├── StageCraft.csproj       # Project file
└── appsettings.json        # Configuration (connection strings, logging)
```

---

## Installation & Setup  

###  Prerequisites  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or .NET 6+ SDK  
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)  

###  Clone the Repository  
```bash
git clone https://github.com/your-username/stagecraft.git
cd stagecraft
```

### Configure Database  
Update your **appsettings.json** connection string:  
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=stagecraftdb;user=root;password=yourpassword"
}
```

### Run Migrations  
```bash
dotnet ef database update
```

### Launch Application  
```bash
dotnet run
```
App will be available at  `https://localhost:5001`  

---

## Future Enhancements  
- Role-specific dashboards (Admin, Production Manager, Viewer).  
- Ticket booking & online payment integration.  
- Export of schedules and activity logs (CSV/Excel).  
- CI/CD pipelines with Docker and GitHub Actions.  
- Advanced analytics dashboard with Power BI integration.  

---

## Author 
**Akshay Keerthi Adhikasavan Suresh** – Full-stack developer of StageCraft.  
Built with **C#, ASP.NET Core MVC, Entity Framework Core (MySQL), Razor Pages, Bootstrap**.  

---
