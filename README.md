# CoreWebApi - project

Boilerplate of Asp.Net Core web-api application.

This is backend for my react-typescript application: https://github.com/Volodymyr759/core-web-client

## Includes:

- Authentication process, based on JWT-tokens (access / refresh);

- CRUD-functionality with filtering, sorting and pagination;

- Swagger documentation;

- Tests of controllers- and services-methods located in separate project CoreWebApi.Tests.

## Useful examples of using views and stored procedures:

- Class OfficeNameId added to context (as not mapped set) and uses view in db - useful for filters on frontend;

- Class StringValue added to context (as not mapped set) and uses stored procedure in db to get list of string data - useful for autocomplete operations on frontend side;

- Good implementation of 'IsExists' functionality - uses small stored procedure instead of usage of EntityFramework for 'heavy' DbSets, which contain lot of linked objects;

- Good implementation of PATCH request is in VacancyController => VacancyService => Repository chain;



