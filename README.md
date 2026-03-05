# StageCraft – Theatre Management Platform 🎭

**Live Demo:** [https://stagecraft-8mjt.onrender.com](Demo!)

> Built with C#, ASP.NET Core MVC (.NET 9), Entity Framework Core, MySQL, and X.PagedList

---

## System Architecture

```
                ┌─────────────────────────────────────────────────────────────────────┐
                │                            CLIENT                                   │
                │                   Browser (Bootstrap 5 UI)                          │
                │         ┌──────────────┐  ┌──────────────┐  ┌────────────────┐      │
                │         │  Razor Views │  │  AJAX/Fetch  │  │  Chart.js      │      │
                │         │  (24 views)  │  │  (comments)  │  │  (analytics)   │      │
                │         └──────┬───────┘  └──────┬───────┘  └───────┬────────┘      │
                └────────────────┼─────────────────┼──────────────────┼───────────────┘
                                │                 │                  │
                                ▼                 ▼                  ▼
                ┌─────────────────────────────────────────────────────────────────────┐
                │                     ASP.NET CORE MVC (.NET 9)                       │
                │                                                                     │
                │  ┌──────────────────────────────────────────────────────────────┐   │
                │  │                      MIDDLEWARE PIPELINE                     │   │
                │  │  ForwardedHeaders → HTTPS → Static Files → Auth → Routing    │   │
                │  └──────────────────────────────────────────────────────────────┘   │
                │                                                                     │
                │  ┌────────────┐ ┌──────────────┐ ┌─────────────┐ ┌─────────────┐    │
                │  │  Account   │ │  Productions │ │  Comments   │ │    Admin    │    │
                │  │ Controller │ │  Controller  │ │ Controller  │ │  Controller │    │
                │  │            │ │              │ │             │ │             │    │
                │  │ • Login    │ │ • Index+     │ │ • Create    │ │ • Dashboard │    │
                │  │ • Register │ │   Search+    │ │   (AJAX)    │ │ • Users     │    │
                │  │ • Logout   │ │   Paginate   │ │ • Delete    │ │ • Managers  │    │
                │  │ • Roles    │ │ • CRUD       │ │   (AJAX)    │ │ • Logs      │    │
                │  │            │ │ • Image      │ │ • Partial   │ │ • Stats     │    │
                │  │            │ │   Upload     │ │   Render    │ │             │    │
                │  └─────┬──────┘ └──────┬───────┘ └─────┬───────┘ └────────┬────┘    │ 
                │        │               │               │                  │         │
                │  ┌─────┴───────────────┴───────────────┴──────────────────┴─────┐   │
                │  │                    SECURITY LAYER                            │   │
                │  │  ASP.NET Identity │ 3 Roles │ CSRF Tokens │ DotNetEnv        │   │
                │  └──────────────────────────────┬───────────────────────────────┘   │
                │                                 │                                   │
                │  ┌──────────────────────────────┴───────────────────────────────┐   │
                │  │                 ENTITY FRAMEWORK CORE (ORM)                  │   │
                │  │          LINQ Queries │ AsNoTracking │ Migrations            │   │
                │  └──────────────────────────────┬───────────────────────────────┘   │
                └─────────────────────────────────┼───────────────────────────────────┘
                ┌─────────────────────────────────────────────────────────────────────┐
                │                        MySQL (Aiven Cloud)                          │
                │                                                                     │
                │  ┌──────────┐ ┌──────────┐ ┌──────────────┐ ┌──────────────────┐    │
                │  │Producs.  │ │ Comments │ │ ActivityLogs │ │ LoginStatistics  │    │
                │  └──────────┘ └──────────┘ └──────────────┘ └──────────────────┘    │
                │  ┌──────────┐ ┌──────────┐ ┌──────────────┐ ┌──────────────────┐    │
                │  │AspNetUsers││  Roles.  │ │ UserRoles.   │ │  +4 Identity     │    │
                │  └──────────┘ └──────────┘ └──────────────┘ └──────────────────┘    │ 
                │                       11 tables total                               │
                └─────────────────────────────────────────────────────────────────────┘
```

---

## Features

- **Role-Based Authorization** — 3-tier access control (Admin, ProductionManager, User) with ASP.NET Core Identity
- **Production Management** — Full CRUD with GUID-based image uploads, LINQ-filtered search, and X.PagedList pagination (9/page)
- **AJAX Comment System** — Fetch API with modal-based partial view rendering, anti-forgery token forwarding, and tiered deletion authorization
- **Admin Dashboard** — User/manager management, audit trail (last 50 events), and Chart.js login statistics
- **Security** — CSRF validation on all 12 POST endpoints, DotNetEnv secret management, ForwardedHeaders for reverse proxy
- **Query Optimization** — AsNoTracking for read-only queries, server-side filtering, cascade deletes

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Core MVC (.NET 9) |
| **Language** | C# |
| **ORM** | Entity Framework Core 9 (Pomelo MySQL) |
| **Database** | MySQL (Aiven Cloud) |
| **Auth** | ASP.NET Core Identity |
| **Frontend** | Razor Views, Bootstrap 5, jQuery |
| **Async** | Fetch API, AJAX Partial Views |
| **Pagination** | X.PagedList.Mvc.Core |
| **Analytics** | Chart.js |
| **Secrets** | DotNetEnv |
| **Deployment** | Docker, Render |


---

## Project Structure

```
StageCraft/
├── Controllers/          # 5 controllers, 30 action endpoints
│   ├── AccountController     # Login, Register, Logout, AccessDenied
│   ├── AdminController       # Dashboard, Users, Managers, Logs, Stats
│   ├── CommentsController    # AJAX Create/Delete with partial rendering
│   ├── HomeController        # Spotlight (9 latest productions)
│   └── ProductionsController # CRUD, Search, Pagination, Image Upload
├── Models/               # 6 entity models
├── ViewModels/           # 5 view models
├── Views/                # 24 Razor views + 3 partials
├── Data/AppDbContext.cs  # EF Core context (4 DbSets + Identity)
├── Migrations/           # 13 schema migrations
├── wwwroot/              # Static assets (CSS, JS, images)
├── Dockerfile            # Multi-stage Docker build
├── Program.cs            # Entry point, middleware, seeding
└── appsettings.json      # Configuration
```

---

## Local Setup

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)

### Run Locally
```bash
git clone https://github.com/isakshay007/StageCraft.git
cd StageCraft
```

Create a `.env` file:
```
ADMIN_EMAIL=123@stagecraft.com
ADMIN_PASSWORD=Ad@2025!
```

Update `appsettings.json` with your MySQL connection, then:
```bash
dotnet restore
dotnet run
```

App runs at `https://localhost:5001`

---

## Deployment

Deployed on **Render** (Docker) with **Aiven** (MySQL).

```bash
# Environment variables set in Render dashboard:
CONNECTION_STRING=server=...;port=...;database=...;user=...;password=...;SslMode=Required;
ADMIN_EMAIL=admin@stagecraft.com
ADMIN_PASSWORD=Admin@2025!
ASPNETCORE_ENVIRONMENT=Production
```

---

## Author
**Akshay Keerthi Adhikasavan Suresh**

Built with C#, ASP.NET Core MVC (.NET 9), Entity Framework Core, MySQL, Bootstrap 5.
