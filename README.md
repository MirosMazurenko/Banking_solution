# Banking Solution API

## Overview

This is a banking solution API that allows users to create accounts, transfer funds, deposit money, and withdraw funds. It exposes several endpoints for interacting with accounts, ensuring data integrity and validation throughout the application.

## Tech Stack

- .NET 8.0: The application is built using ASP.NET Core, leveraging modern C# features and practices.
- Entity Framework Core: ORM used for database interactions.
- SQL Server: Database used to store account and transaction data.
- AutoMapper: Used for mapping between entities and DTOs (Data Transfer Objects).
- Swagger: API documentation and testing tool.
- Postman: For manual testing of the API.
- XUnit: Unit testing framework.

## Features

- Create Account: Allows users to create a new account with an initial balance.
- Get Account Details: Retrieves details of an account using the account number.
- Deposit Funds: Adds funds to an account.
- Withdraw Funds: Withdraws funds from an account.
- Transfer Funds: Transfers funds from one account to another.
- Account Listing: Lists all accounts in the system.

## Prerequisites

Before you begin, ensure you have the following installed on your local machine:

- .NET SDK 8.0

- SQL Server or access to a SQL Server instance

- Postman (optional, for manual testing)

## Setup Instructions

### 1. Clone the repository to your local machine

```cmd
git clone https://github.com/MirosMazurenko/Banking_solution.git
```

### 2. Set Up the Database

Make sure that SQL Server is running. You can either use an existing instance or set up a local SQL Server.

- Create a database called BankingSolution (or use an existing one).
- Modify the appsettings.json file to include your database connection string:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankingSolution;Trusted_Connection=True;MultipleActiveResultSets=true;"
}
```

### 3. Migrate the Database

After setting up your connection string, run the migrations to create the necessary database tables:

```cmd
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application

To run the API locally, use the following command:

```cmd
dotnet run
```

### 5. Testing the Endpoints

Once the application is running, you can test the endpoints using Postman or Swagger UI.

- Swagger UI: Navigate to http://localhost:5123/swagger for a visual interface to test the API.
- Postman: Import the collection or manually test the endpoints.

### 6. Tests

Unit tests for services and repositories are written using XUnit. These tests can be run with the following command:

```cmd
dotnet test
```
