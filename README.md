# Application Architecture

"The architecture for developers who value simplicity over ceremony."   

The distinction is that SIMP Architecture isn't anti-abstraction or anti-architecture. It's anti-complexity. You can embrace abstractions, dependency injection, design patterns, and reusable components without adopting rigid architectural rules. This is great for microservice architecture and not large monoliths.   

## Philosophy
- Abstractions are good. Use them whenever they make code easier to understand, maintain, test, or extend.
- Patterns should be repeatable. Prefer straightforward implementations that developers can copy, paste, and customize.
- Architecture should be flexible. Organize projects and dependencies according to actual application requirements.
- Simplicity is intentional. Don't mistake complexity for sophistication.
- Consistency beats cleverness. Predictable code is easier to maintain than unnecessarily elaborate unique designs.
- Git Shit Done!

## Project Setup
This explains how we set up project.   

### Project Structure
We set up a `libs` folder in Visual Studio so we can build the dependent libraries easier.

#### Data
In my experience, some of the most significant and challenging changes an application undergoes over its lifetime occur in the data access layer.

Consider migrating from legacy Entity Framework to modern Entity Framework Core. Perhaps we want greater control over database access and decide to adopt Dapper with stored procedures. Or maybe reducing licensing costs becomes a business priority, prompting a migration from SQL Server to PostgreSQL.

These are substantial architectural changes, and they're realistic scenarios that applications may encounter over time.

**This is why SIMP Architecture places particular emphasis on decoupling the data access layer and persistence entities from the application layer.** By establishing clear boundaries between application logic, data access implementations, and database-specific entities, we make it easier to replace or modernize persistence technologies without introducing widespread changes throughout the application.

The goal isn't to abstract everything or introduce unnecessary complexity. It's to apply thoughtful abstractions where they provide the greatest long-term value.

**We want the data access layer to be interchangeable without requiring the application to be rewritten.**

### Project File Updates (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>api-sample-d78ed487-720a-454d-82a6-3d6832f95d3b</UserSecretsId>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
    <DockerfileContext>..\..\..</DockerfileContext>
  </PropertyGroup>
```

### User Secrets

```json

```

### Azure App Configuration + Key Vault

### Containerization

### Scripts: Two is One, One is None
There's a principle that applies here: **"Two is one, and one is none."** If you have two ways to accomplish something and one fails, you still have a fallback. If you only have one and it fails, you're left with nothing.

The same principle applies to our container build process. We can build containers directly in Visual Studio or through the command line. Having both options provides redundancy, reduces our dependency on a single tool, and ensures we can continue working if one approach fails.

`build.ps1` / `build.sh` - Builds the container image for the application.

### Helm Charts
