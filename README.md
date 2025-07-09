# String Processing App (.NET 9 + Angular 20)

This project is a single-page application that allows users to input text, send it to a backend service for processing, and receive the results in a streamed manner. The output includes unique sorted characters with their counts and the Base64-encoded string.

## Tech Stack

- **Frontend**: Angular 20
- **Backend**: ASP.NET Core (.NET 9)
- **Containerization**: Docker
- **Proxy**: Nginx


## Local Development Setup

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [Docker and Docker Compose](https://docs.docker.com/compose/)


### 1. Backend (API)

```
cd DataProcessingService
dotnet restore
dotnet run
```


### 2. Frontend (Angular SPA)

```
cd data-processing-spa
npm install
ng serve
```

Angular app will run on http://localhost:4200.
Make sure the API is running on port 5075.


## Running with Docker

### 1. Build and start all containers:

```
docker compose up --build
```

This will start:

	- data-processing-service – .NET API

	- data-processing-spa – Angular build container

	- nginx – the entrypoint serving the Angular app and proxying API requests
	
### 2. Open the application

Go to: http://localhost

**Default credentials for authorization:**
dps_user
SomePassword!@#


## Running Tests

Backend (NUnit):

```
cd Tests/DataProcessingService.Tests
dotnet test
```

Frontend (Jasmine/Karma):

```
cd data-processing-spa
ng test
```