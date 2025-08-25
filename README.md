# CarAPI - Car Stock Management API

A C# ASP.NET Core Web API for managing car inventories for dealerships with JWT Authentication. This project demonstrates fundamental concepts used in virtually every web API project, including security, database management, and API design.

---

## 🚀 Key Features

### Authentication & Authorization
- JWT token-based authentication (expires after 2 hours)
- Each dealer can only access their own cars
- Password hashing using BCrypt
- Registration and login endpoints

### Car Management Operations
- Add and remove cars
- List cars with stock levels
- Update stock levels and prices
- Search by make and model

### Technical Implementation
- **Database:** SQLite with Dapper for lightweight data access
- **Authentication:** JWT tokens with 2-hour expiration
- **Validation:** Data annotations with comprehensive error handling
- **Security:** Dealers are isolated — can only access their own inventory
- **API Documentation:** Swagger/OpenAPI with JWT authentication support

---

## 📦 NuGet Packages Installed
- [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next/) – Password hashing & verification
- [Dapper](https://www.nuget.org/packages/Dapper/) – SQL requests
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/) – JWT
- [Microsoft.AspNetCore.OpenApi](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/) – OpenAPI support
- [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/) – SQLite provider
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/) – Detailed Swagger UI

---

## Folder Structure
```bash
CarAPI/
├── Controllers/
│   ├── AuthController.cs
│   └── CarController.cs
├── Data/
│   └── DatabaseService.cs
├── DTOs/
│   └── CarDtos.cs
├── Models/
│   └── Model.cs
├── Services/
│   ├── AuthService.cs
│   └── CarService.cs
├── appsettings.json
├── CarAPI.http
├── carstock.db
└── Program.cs
```
---

## 🏗 Architecture Patterns
- Layered architecture
- Dependency injection
- Repository pattern via services
- DTO pattern

### Security Concepts
- JWT authentication
- Password hashing
- Data isolation between dealers
- Input validation

### Database Concepts
- Relational design
- Indexing strategy
- Connection management
- Query optimization

### API Design
- RESTful principles
- Proper HTTP status codes
- Error handling
- Comprehensive documentation

---

## 🌐 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new dealer |
| POST | `/api/auth/login` | Login and get JWT token |

### Car Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cars/{id}` | Get specific car by ID |
| GET | `/api/cars?make={make}&model={model}` | Get cars by make and model (omit query to list all) |
| POST | `/api/cars/add` | Add a new car |
| DELETE | `/api/cars/delete/{id}` | Remove car by ID |
| PUT | `/api/cars/{id}/update` | Update car stock and/or price by ID |

---

## ⚙️ Setup Instructions
1. Create a new ASP.NET Core Web API project.
2. Replace the generated files with your project code.
3. Install required NuGet packages:
```bash
   dotnet add package Dapper
   dotnet add package Microsoft.Data.Sqlite
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   dotnet add package BCrypt.Net-Next
   dotnet add package Swashbuckle.AspNetCore
```
4. Run the application — it will automatically create the SQLite database.
- Run either by either an IDE or by using command 
```
dotnet run
```
---

> [!NOTE]
> - JWT tokens expire after 2 hours.
> - When logging in, grab the JWT token and click the green Authorize button in Swagger to authorize requests.
> - DealerId is recognized from the token.
> - Logout authorization is required when switching between dealers during testing.

## 🔄 Complete Workflow Example
Register a dealer
```json
POST /api/auth/register
{
  "username": "testdealer3",
  "password": "password789",
  "companyName": "123 Cars"
}
```

Login
```json
POST /api/auth/login
{
  "username": "testdealer1",
  "password": "password123",
}

```

Add a car (with JWT token)
```json
POST /api/cars/add
Authorization: Bearer {your-jwt-token}
{
  "make": "Toyota",
  "model": "Camry",
  "year": 2023,
  "stockLevel": 5,
  "price": 25000.00
}
```

## 🧪 Extra Test Cases
Users Already Registered
```json
{
  "username": "testdealer1",
  "password": "password123",
  "companyName": "ABC Motors"
}
{
  "username": "testdealer2",
  "password": "password456",
  "companyName": "XYZ Auto Sales"
}
```
Users to Register
```json
{
  "username": "testdealer3",
  "password": "password789",
  "companyName": "123 Cars"
}
```

Cars to Add
```json
{
  "make": "Toyota",
  "model": "Camry",
  "year": 2023,
  "stockLevel": 10,
  "price": 25000.50
}
{
  "make": "Honda",
  "model": "Accord",
  "year": 2022,
  "stockLevel": 5,
  "price": 23500.00
}

```

## 📊 Architecture Diagram


