# Negotiation API

A .NET 9 Web API application that implements a price negotiation process for e-commerce products.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Business Logic](#business-logic)
- [Validation](#validation)
- [Testing](#testing)
- [Installation](#installation)
- [Usage](#usage)
- [Example API Requests](#example-api-requests)
- [Future Improvements](#future-improvements)

## Overview

This API facilitates a price negotiation process where:
- Customers can propose prices for products (maximum 3 attempts)
- Store employees can accept or reject offers
- Negotiations expire after 7 days if no new offer is made after rejection

The application follows RESTful principles and includes:
- Product management module
- Negotiation process module
- JWT authentication for employees
- Comprehensive input validation
- Swagger documentation

## Features

### Product Module
- Create new products (employee only)
- Get product by ID
- List all available products

### Negotiation Module
- Start new negotiation (customer)
- Propose new offer (customer)
- Accept negotiation (employee)
- Reject negotiation (employee)
- View negotiation status

### Security
- JWT authentication for employee actions
- Role-based authorization
- Secure password handling

## API Endpoints

### Authentication
- `POST /auth/login` - Employee login (returns JWT token)

### Products
- `GET /products` - List all products
- `GET /products/{id}` - Get product by ID
- `POST /products` - Create new product (requires authentication)

### Negotiations
- `GET /negotiations` - List all negotiations (requires authentication)
- `GET /negotiations/{id}` - Get negotiation by ID
- `POST /negotiations` - Start new negotiation
- `PATCH /negotiations/{id}/accept` - Accept negotiation (requires authentication)
- `PATCH /negotiations/{id}/reject` - Reject negotiation (requires authentication)
- `PATCH /negotiations/{id}/reoffer` - Propose new offer

## Authentication

Employee authentication is required for:
- Creating products
- Viewing all negotiations
- Accepting/rejecting negotiations

**Default Employee Credentials:**
- Username: `admin`
- Password: `admin123`

## Business Logic

### Negotiation Rules
1. Maximum 3 offer attempts per negotiation
2. 7-day deadline to make new offer after rejection
3. New offers must be lower than previous offers
4. Negotiations automatically expire after deadline
5. Only original customer can make new offers

### Status Flow
- `Open` → `Accepted` (by employee)
- `Open` → `Rejected` (by employee)
- `Rejected` → `Open` (by customer with new offer)
- `Rejected` → `Expired` (after 7 days with no action)

## Validation

All endpoints include comprehensive validation:
- Required fields
- Email format validation
- Price must be positive
- Product name length limits
- Business rule enforcement (offer attempts, deadlines, etc.)

## Testing

Unit and integration tests are available in a separate repository:
[NegotiationAPI.Tests](https://github.com/Patryk0329/NegotiationAPI.Tests)

Tests cover:
- Controller actions
- Business logic
- Validation rules

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Patryk0329/NegotiationAPI.git
   ```
2. Navigate to the project directory:
   ```bash
   cd NegotiationAPI
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Configure JWT settings in `appsettings.json`:
   ```json
   "Jwt": {
     "Key": "your-secret-key-here",
     "Issuer": "negotiation-api",
     "Audience": "negotiation-api-users",
     "ExpiryInMinutes": 60
   }
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## Usage

1. Access Swagger UI at:
   ```
   https://localhost:{port}/swagger
   ```

2. For employee actions:
   - Login at `/auth/login`
   - Use the returned token with the `Authorization` header:
     ```http
     Authorization: Bearer {your-token}
     ```

## Example API Requests

### Start New Negotiation
- **Endpoint**: `POST /negotiations`
- **Headers**: 
  - `Content-Type: application/json`
- **Body**:
  ```json
  {
    "productId": 1,
    "offeredPrice": 2500.00,
    "customerEmail": "customer@example.com"
  }
  ```

### Propose New Offer
- **Endpoint**: `PATCH /negotiations/1/reoffer`
- **Headers**: 
  - `Content-Type: application/json`
- **Body**:
  ```json
  {
    "newPrice": 2300.00,
    "customerEmail": "customer@example.com"
  }
  ```

### Accept Negotiation (Employee)
- **Endpoint**: `PATCH /negotiations/1/accept`
- **Headers**: 
  - `Authorization: Bearer {your-token}`

### Reject Negotiation (Employee)
- **Endpoint**: `PATCH /negotiations/1/reject`
- **Headers**: 
  - `Authorization: Bearer {your-token}`

### 📄 Notes
    - This project uses in-memory storage to simplify deployment for recruitment purposes.

    - If you wish to extend the application, you can easily integrate Entity Framework Core and a persistent SQL database.

### 🤝 Author
    Patryk Hołubowicz – Recruitment project for Software Mind
