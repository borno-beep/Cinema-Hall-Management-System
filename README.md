# 🎬 Cinema Hall Management System

A full-featured desktop application for cinema hall operations, movie scheduling, dynamic seat reservation, concession food ordering, and revenue reporting.

Built using **C# Windows Forms**, raw **ADO.NET** (`Microsoft.Data.SqlClient`), and **Microsoft SQL Server**, following a strict **3-Layer Architecture** (Presentation / BLL / DAL).

---

## 🏗️ Project Architecture & OOP Design

The solution is organized into clear architectural layers:

```
CinemaHallSystem/
├── Models/        ← POCO domain entity models (AppUser, Movie, Hall, Seat, Show, Booking, Payment, etc.)
│                    Includes Polymorphism: abstract AppUser base with Admin, Staff, Customer subclasses
├── DAL/           ← Data Access Layer using raw ADO.NET (SqlConnection, SqlCommand, SqlDataReader, SqlTransaction)
│                    Implements generic IRepository<T> interface for Abstraction
├── BLL/           ← Business Logic Layer (AuthManager, MovieManager, ShowManager, BookingManager, ReportManager, etc.)
│                    Enforces business rules and manages atomic transactions
├── Utilities/     ← Shared helper utilities (SHA256 PasswordHasher, Validator, PriceCalculator)
└── Forms/         ← Windows Forms Presentation Layer with responsive, modern UI
```

### 🎓 Key OOP Concepts for Viva / Academic Presentation:
- **Polymorphism**: `AppUser` is an abstract base class. Each role (`Admin`, `Staff`, `Customer`) overrides the polymorphic `GetDashboardForm()` method to open the appropriate dashboard automatically upon login.
- **Abstraction**: Data access is decoupled through the generic `IRepository<T>` interface.
- **Encapsulation**: Strict separation of concerns — UI forms never execute SQL queries directly; they call BLL managers, which coordinate with DAL repositories.
- **Transaction Management**: `BookingManager.CompleteBooking()` wraps seat reservation, ticket payment, and food ordering in an atomic `SqlTransaction` with full rollback on failure.
- **Concurrency Protection**: Show seats are pre-generated with statuses (`Available`, `Locked`, `Booked`). Seats are atomically locked during checkout to prevent double-booking.

---

## 📋 Prerequisites

Before running the project on your machine, ensure you have:
1. **.NET 8.0 SDK** (or later, e.g. .NET 10.0 SDK): [Download .NET](https://dotnet.microsoft.com/download)
2. **Microsoft SQL Server** (or SQL Server Express / Developer / LocalDB): [Download SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)
3. **SQL Server Management Studio (SSMS)**: [Download SSMS](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)
4. *(Optional)* **Visual Studio 2022** with *.NET desktop development* workload.

---

## 🚀 Step-by-Step Setup Guide

### Step 1: Clone the Repository
```bash
git clone https://github.com/your-username/cinema-hall-management-system.git
cd cinema-hall-management-system
```

---

### Step 2: Set Up the Database (One-Click Setup)
You can set up the complete database in **one single step**:

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your SQL Server instance (usually `localhost`, `.`, or `.\SQLEXPRESS`).
3. Open the master script located at:
   ```
   Database/SetupDatabase.sql
   ```
4. Click **Execute** (or press **F5**).

> **What this script does:**
> - Creates the `CinemaHallDB` database automatically if it doesn't exist.
> - Creates all 11 tables with primary keys, foreign keys, indexes, and constraints.
> - Seeds 3 default user accounts (Admin, Staff, Customer).
> - Seeds 4 cinema halls (Gold, Silver, Platinum, Royal) with 218 configured seats.
> - Seeds 12 movies across 6 genres (Action, Sci-Fi, Thriller, Comedy, Drama, Horror).
> - Generates 7 upcoming movie shows and ~400+ available seats for booking.
> - Seeds 6 concession food items.

---

### Step 3: Check Database Connection String
Open `CinemaHallSystem/App.config`. By default, it connects to standard local SQL Server:

```xml
<connectionStrings>
    <add name="CinemaDB"
         connectionString="Server=localhost;Database=CinemaHallDB;Integrated Security=True;TrustServerCertificate=True;"
         providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

> **Note:** If your SQL Server instance uses a named instance (such as SQL Server Express), change `Server=localhost;` to `Server=.\SQLEXPRESS;`.

---

### Step 4: Run the Application

#### Option A: Using .NET CLI (Terminal)
```bash
dotnet run --project CinemaHallSystem
```

#### Option B: Using Visual Studio
1. Double-click `CinemaHallSystem.sln` to open the solution in Visual Studio.
2. Set `CinemaHallSystem` as the startup project.
3. Press **F5** (or click the green **Start** button).

---

## 🔑 Default Login Credentials

The database comes pre-loaded with testing accounts for both roles:

| Role | Username | Password | Access / Features |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `admin123` | Full dashboard metrics, movies CRUD, hall management, show scheduling, food catalog, booking management, exportable revenue reports |
| **Customer** | `customer` | `customer123` | Movie browsing by genre, dynamic seat selection grid, optional food ordering, payment gateway, printable ticket invoice |

> You can also register a brand-new Customer account anytime using the **Register** link on the login screen.

---

## 🌟 Key Application Features

1. **All-in-One Booking Wizard**:
   - Movie selection with genre filters.
   - Interactive seat grid (Green = Available, Blue = Selected, Gray = Booked).
   - Snack and beverage selection with live subtotal updates.
   - Payment method choice (Cash, Credit/Debit Card, Mobile Banking).
   - Instant ticket invoice receipt with Booking ID and seat numbers.

2. **Live Revenue & Operational Analytics**:
   - **Admin Dashboard**: Live stat cards showing today's revenue, confirmed bookings, active movies, hall occupancy %, and total customers.
   - **Reports Module**: Generate custom reports for Daily Sales, Monthly Revenue breakdown, Movie Popularity, and Food Sales.
   - **Export to CSV**: Export any generated report to an Excel-compatible spreadsheet.

3. **Cinema Administration & Operations**:
   - Movie catalog management (Title, Genre, Duration, Rating, Release Date, Description).
   - Hall configuration with automatic seat generation (Regular & Premium tiers).
   - Show scheduling with time conflict prevention.
   - Booking cancellation with automatic seat release and refund status tracking.

---

## 🛠️ Built With

- **Language**: C# 12 / .NET 8.0 (Windows Forms)
- **Database**: Microsoft SQL Server
- **Data Access**: ADO.NET (`Microsoft.Data.SqlClient` v5.2.0)
- **Security**: Salted SHA-256 password hashing (`System.Security.Cryptography`)
- **Configuration**: `System.Configuration.ConfigurationManager` v8.0.0

