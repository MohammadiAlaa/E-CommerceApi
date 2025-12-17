🛒 E-Commerce RESTful API
A production-ready Backend for an E-Commerce platform built with ASP.NET Core 8, featuring a complete business cycle and multi-role authorization.

👥 Multi-Role Authorization
The system manages different business operations through specific roles:

Admin: Full system management, catalog control, and order oversight.

Accountant: Access to the Dashboard for tracking total revenue, sales analytics, and inventory health.

Driver: Dedicated to updating Shipping statuses from pending to delivered.

User: Browsing products, placing orders, and leaving verified reviews.

✨ Core Features
Order Management: End-to-end order lifecycle from creation to completion.

Admin Dashboard: Real-time stats for Total Revenue, Top Sellers, and Low-Stock alerts.

Review System: Integrated product rating and feedback system.

Security: Fully secured using JWT Bearer Authentication and Role-based access control.

🛠️ Technical Stack
Architecture: Repository Pattern & Unit of Work for clean and maintainable code.

Database: SQL Server with Entity Framework Core (Code-First).

Data Handling: Optimized communication using DTOs.

⚙️ Setup & Installation
Clone the repository.

Rename appsettings.Example.json to appsettings.json and configure your Connection String.

Run Update-Database in the Package Manager Console.

Launch the project and explore via Swagger UI.
