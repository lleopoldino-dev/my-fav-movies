# MyFavMovies API

This is MyFavMovies API. Where you can create an account and manage a shared movie list.

## Overview

This service is responsible for handling user creation, retrieving authentication token, and movies management endpoints.
Endpoints that require authentication will be expecting a valid JWT token on the header of the request.

## Technologies

* [.NET 8]
* [PostgreSQL]
* [Docker]

### Characteristics

* Clean Architecture
* Central package management
* Exception Middleware with ProblemDetails
* Swagger documentation
* xUnit
* Doesn't use Entity Framework, Dapper, or Mediator.

## Solution structure

    .
    ├── configurations      # Solution configuration files
        ├── docker-compose  # For creating and executing database and application containers
        ├── dockerfile      # Application docker file
        ├── init.sql        # SQL script for creating database tables
        └── README.md
    ├── src                 # Solution source folder
        ├── WebApi          # Web API application for handling requests, services resolutions and configuration
        ├── Business        # Layer containing business services, models and interfaces to be implemented by other layers
        └── Infrastructure  # Layer containing data access, businesses interface implementations
    ├── test                           # Solution tests folder
        ├── Business.UnitTests         # Unit tests for Business project
        ├── Infrastructure.UnitTests   # Unit tests for Infrastructure project
        └── WebApi.UnitTests           # Unit tests for WebApi project

## Database structure

The database contains two tables, Movies and Users:

```
CREATE TABLE IF NOT EXISTS public.movies (
	title varchar(200) NOT NULL,
	releasedate date NOT NULL,
	category varchar(150) NOT NULL,
	id uuid NOT NULL,
	CONSTRAINT movies_pk PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.users (
	"name" varchar NOT NULL,
	email varchar NOT NULL,
	passwordhash varchar NOT NULL,
	id uuid NOT NULL,
	createddate date NOT NULL,
	CONSTRAINT users_pk PRIMARY KEY (id),
	CONSTRAINT users_unique UNIQUE (email)
);
```

## User Story

As a user
I want to be able to access the system with email and password
So that I can manage shared movies list

### Acceptance Criteria

- Create access with email and password
- Get valid token for accessing authenticated endpoints
- Update users record and retrieve user information
- Users with same email are not allowed
- View public movies list without authentication
- Manage movies list (CRUD operations)
- Movies with same titles are not allowed

### Docker Configuration

On the root path of the application (where docker file is placed), run the command `docker build -t my-fav-movies . --no-cache` for creating a new image for .net application.
If the creation is succeded, run `docker-compose up -d` to run docker-compose file. 
If it's the first time running, the system will download postgres image and load it with preconfigured tables found in init.sql file.

Access `http://localhost:8000/swagger` to view application swagger page and have fun!


### Minikube Configuration

Check out README file located on `./kubernetes` folder for more instructions.