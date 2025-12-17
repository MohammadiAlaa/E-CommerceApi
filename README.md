# E-Commerce Web API 🛒

A complete RESTful API for an E-Commerce system built with **ASP.NET Core 8**.

## 🚀 Key Features
- **Secure Auth:** JWT-based Authentication with Role-based Authorization (Admin/User).
- **Core Logic:** Product Management, Order Processing, and Shopping workflow.
- **Operations:** Payment confirmation and Shipping tracking integration.
- **Feedback:** User Review and Rating system.
- **Analytics:** Admin Dashboard for sales statistics and stock alerts.

## 🛠️ Architecture
- **Design Pattern:** Repository Pattern & Unit of Work for clean code and testability.
- **Database:** Entity Framework Core with SQL Server.
- **Safety:** Sensitive configurations are handled via environment-ready `appsettings.Example.json`.

## ⚙️ Setup
1. Clone the repo.
2. Rename `appsettings.Example.json` to `appsettings.json`.
3. Update your **Connection String**.
4. Run `Update-Database` in Package Manager Console.
