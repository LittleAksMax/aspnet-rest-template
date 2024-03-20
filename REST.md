# REST API Template

## Intro to REST

- **RE**presentational **S**tate **T**ransfer
- Architectural Style

### What makes a system RESTful?

#### Uniform Interface

- Clean defined interface between client and server (decoupled frontend/backend)
- **Identification of resources** -- clear way to identify a resource uniquely between clients and servers
- **Manipulation of resources through representations** -- Resources should have uniform/consistent representations, each resource representation should
- **Self-descriptive messages**
- **Hypermedia as the engine of application state** -- Required endpoints are returned from the initial URI of a resource

#### Stateless

- Each request by the client to the server contains all the data the server needs to process the request

#### Cacheable

- Server should implicitly or explicitly let the client know whether it can cache a response and for how long it can be cached
- It is still up to the client to bypass the cache

#### Client-server

- Client and server can grow and evolve independently, adhering to pre-defined contracts

#### Layered System

- The client cannot know if it is directly connected to the server or a load balancer

#### Code on demand (optional)

- The server can send scripts to the client to run
- This constraint is (really) never implemented

### Resource naming and routing

- The HTTP verbs describe the operation
- The URL identifies the resource

You can access resources via `GET` requests.
- `/movies` -- get all movies
- `/movies/{id}` -- get movie by ID
- /movies/{id}/ratings` -- get all ratings of a movie

**NOTE:** The pluralisation of the resource root is important.

- `POST` -- Create resource
- `GET` -- Retrieve
- `PUT` -- Complete update (full resource representation replacement)
- `PATCH` -- Partial update (not in common use)
- `DELETE` -- Removal of resource

Verbs are considered **safe** if they don't mutate resources and **unsafe** if they do mutate resources.

**Idempotency** refers to how some requests don't necessarily have the same response for repeated operations.
- POST is not idempotent, because after the first request, future POST requests to create a resource will get a conflict response.

### Response Status Codes.

#### Success

- In range 200-299

Common ones are:
- 200 - OK
- 201 - Created (used for creation of resources, also requires setting of *location* header to allow client to retrieve newly created resource)
- 202 - Accepted (used for signalling to client that a request, although not yet completed, is starting to be processed -- e.g., asynchronous job)
    - Usually, there is a way to check on the status of the request, provided by the response.
- 204 - No Content


#### Redirects

- In range 300-399

#### Client Error

- In range 400-499

Common ones are:
- 400 - Bad request
- 401 - Unauthorised
- 403 - Forbidden
- 404 - Not found
- 405 - Method not allowed
- 409 - Conflict

#### Server Error

- In range: 500+

Common ones are:
- 500 - Internal Server Error

### Hypermedia as the Engine of Application State (HATEOAS)

- Responses return links to other resources that are relevant.
- These responses traditionally contain the URL, the method, and the relationship to the request
- Nowadays, they often just use the links, and then developers rely on the documentation to figure out what to do with the links in the response.

### Types of Errors

#### Errors

- Client sends invalid data that can't be processed by server
- Response statuses are 400-499

#### Faults

- Valid request, but the server is not in a healthy state
- Response statuses are 500+