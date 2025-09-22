# 🧠 Task Management API

A clean, RESTful API built with ASP.NET Core and SQL Server for managing tasks, users, and authentication. Designed for backend mastery and cloud deployment readiness.



## 🚀 Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT
- **Deployment Planned**: Cloud deployment with Render and Azure SQL is in progress


## 📋 Requirements

- ✅ [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download)
- ✅ [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- ✅ Visual Studio or VS Code

## 📦 Features

- User registration & login with JWT
- Create, update, delete tasks
- Role-based access control
- Environment variable support for secure config
- Swagger UI for interactive API testing

## 🗃️ Data Model Overview

This API manages the following entities:

- **Accounts**: User login and identity
- **Roles**: Role-based access control
- **Employees & Leaders**: Organizational structure
- **Teams**: Grouping of employees
- **Tasks & TaskStates**: Task tracking and status
- **Categories**: Task classification
- **Comments**: User feedback on tasks


## 🔧 Setup Instructions

### 🛠️ Step 1: Create a SQL Server Account in SSMS

To enable SQL authentication, you’ll need to create a SQL Server login using **SQL Server Management Studio (SSMS)**.

📺 **Video Guide**: [How to Create a SQL Server Account](https://www.youtube.com/watch?v=LksXHhS42xs)

> If the video becomes unavailable, simply search YouTube for  
> **"Create a SQL Server account for authentication"** — there are plenty of helpful tutorials.

 
### 2. Clone the repo

```bash
git clone https://github.com/your-username/task-management-api.git
cd task-management-api
