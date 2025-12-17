# 🛒 E-Commerce RESTful API

A production-ready Backend for an E-Commerce platform built with **ASP.NET Core 8**, featuring a complete business cycle and multi-role authorization.

---

## 👥 Multi-Role Authorization
The system manages different business operations through specific roles:

| Role | Responsibilities |
| :--- | :--- |
| **Admin** | Full system management, catalog control, and order oversight. |
| **Accountant** | Access to Dashboard for tracking revenue, sales analytics, and inventory health. |
| **Driver** | Dedicated to updating shipping statuses from *Pending* to *Delivered*. |
| **User** | Browsing products, placing orders, and leaving verified reviews. |

---

## ✨ Core Features
* **📦 Order Management:** End-to-end order lifecycle from creation to completion.
* **📊 Admin Dashboard:** Real-time stats for Total Revenue, Top Sellers, and Low-Stock alerts.
* **🌟 Review System:** Integrated product rating and feedback system.
* **🔒 Security:** Fully secured using **JWT Bearer Authentication** and Role-based access control.

---

## 🛠️ Technical Stack
* **Architecture:** Repository Pattern & Unit of Work for clean and maintainable code.
* **Database:** SQL Server with Entity Framework Core (Code-First).
* **Data Handling:** Optimized communication using **DTOs**.

---

## ⚙️ Setup & Installation

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/MohammadiAlaa/E-CommerceApi.git](https://github.com/MohammadiAlaa/E-CommerceApi.git)
   Configuration:

Rename appsettings.Example.json to appsettings.json.

Configure your Connection String inside appsettings.json.

Database Migration:

PowerShell

Update-Database
Launch:

Run the project and explore via Swagger UI && Postman
