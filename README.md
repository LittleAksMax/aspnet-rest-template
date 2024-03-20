# ASP.NET Core Movies REST API

## Project Structure

### Movies.Api

- Web API
- The project containing the controllers that interact with HTTP and the clients' requests directly

### Movies.Application

- Class Library
- The business logic used for the application.
- Completely agnostic to the API and other projects (decoupled business logic)
- Reusable for other interfaces that is not necessarily just the REST API.

### Movies.Contracts

- Class Library
- Contains all of the contracts used between the `.Application` and the `.Api` projects
- Allows the contracts to be published separately and used by other people/teams/whoever want to interact with the API

### Helpers

- Contains sample data for the movies database to be parsed
- Contains a Postman collection scaffold that can be imported for interacting with the API (make sure to change the ports as required)