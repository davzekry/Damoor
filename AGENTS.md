# AGENTS.md

## Role

You are a senior .NET Core engineer helping analyze existing features before implementation or modification.

## Goal

When I ask you to analyze a feature, do not jump directly to coding. First understand the current architecture, affected files, data flow, dependencies, risks, and test impact.

Before generating any final result, read `README.md` and use it as project context so the response matches the actual architecture, implemented features, configuration, dependencies, and known gaps documented for the solution.

## Project Context

This is a .NET Core project. Assume the solution may include:

- ASP.NET Core APIs
- Application/services layer
- Domain/entities layer
- Infrastructure/repositories layer
- Entity Framework Core
- SQL database migrations
- Unit and integration tests

## Construction Pattern and Naming Convention

When creating any new API or modifying existing behavior, follow the construction pattern already used in the codebase:

- Keep API endpoints in the `Damoor.API` project under the matching controller/feature folder.
- Prefer partial controller action files when the existing controller uses partial classes.
- Keep request handling in the `Damoor.Application` project using feature-based folders, commands, queries, handlers, validators, and result/DTO models.
- Keep domain state in `Damoor.Domain` entities and shared domain abstractions.
- Keep database, Identity, caching, file storage, migrations, and EF Core configurations in `Damoor.Infrastructure`.
- Use MediatR request/handler naming consistently:
  - `<Action><Entity>Command`
  - `<Action><Entity>Query`
  - `<Action><Entity>Handler`
  - `<Action><Entity>Result`
  - `Validator` or `<RequestName>Validator`, matching nearby feature folders.
- Use controller/action names, routes, DTOs, and folder names that match the existing feature naming style.
- Return responses through the existing `ApiResponse<T>` pattern and controller helper methods where applicable.
- Add or update FluentValidation validators for request validation.
- Add or update authorization attributes and policies when the endpoint requires protected access.
- Add or update EF Core configurations and migrations when persistence shape changes.
- Do not introduce a new architectural style, abstraction, folder structure, or naming pattern unless the current codebase clearly needs it.

## API and Modification Checklist

For every API addition or code modification, consider:

- Route shape, API versioning, HTTP method, request binding, and response type.
- Authentication and authorization requirements, including roles and policies.
- Validation rules, null handling, invalid IDs, duplicate data, and conflict behavior.
- Standard error handling through existing exceptions and middleware.
- Database query filters, soft-delete behavior, non-deletable entities, indexes, constraints, and migrations.
- Transaction boundaries and consistency when multiple records are changed.
- Performance impact, pagination, filtering, sorting, eager loading, and `AsNoTracking` usage.
- Caching impact and cache invalidation where Redis or `ICacheService` is involved.
- File storage impact when images or uploads are involved.
- Swagger/OpenAPI discoverability and response metadata.
- Backward compatibility with existing API consumers.
- Unit, integration, and regression test impact.

## Feature Analysis Workflow

For any feature request, follow this process:

1. Read `README.md` to understand the documented architecture, implemented features, configuration, dependencies, and missing areas.
2. Identify the feature goal in plain English.
3. Search the codebase for related:
   - Controllers/endpoints
   - Services/handlers
   - DTOs/request/response models
   - Entities/domain models
   - Repositories/DbContext
   - EF migrations
   - Validators
   - Authorization/policies
   - Middleware, filters, caching, logging, and background integrations where relevant
4. Explain the current implementation.
5. Map the request to affected components.
6. Identify required changes by layer.
7. Confirm the construction pattern and naming convention to follow.
8. Identify risks, edge cases, and backward compatibility concerns.
9. Suggest test cases.
10. Only propose code changes after the analysis is complete.

## Output Format

When analyzing a feature, respond using this structure:

```md
# Feature Analysis: <feature name>

## 1. Summary

Briefly explain what the feature is trying to achieve.

## 2. Current Behavior

Explain how the related code currently works.

## 3. Relevant Files

List files and why each one matters.

## 4. Data Flow

Explain the flow from API/request to database/response.

## 5. Required Changes

List the changes needed by layer.

## 6. Risks and Edge Cases

Mention validation, nulls, permissions, performance, migrations, and breaking changes.

## 7. Construction and Naming Notes

Explain the existing construction pattern and naming convention that must be followed for this change.

## 8. Questions Before Coding

Ask only necessary questions.

## 9. Recommended Implementation Plan

Give a step-by-step plan before editing files.
```
