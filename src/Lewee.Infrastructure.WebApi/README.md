# Lewee.Infrastructure.WebApi

Infrastructure utilities for ASP.NET Core Web API applications.

## Features

- **SSE (Server-Sent Events)** - Endpoint configuration for real-time event streaming with user-based filtering
- **Health Checks** - Database health check configuration for ASP.NET Core applications

## Usage

### SSE Endpoint

```csharp
// In Program.cs
builder.Services.AddClientEventChannel();

var app = builder.Build();
app.MapSseEndpoint();
```

### Health Checks

```csharp
// In Program.cs
builder.Services.AddDatabaseHealthCheck<MyDbContext>();

var app = builder.Build();
app.UseHealthEndpoints();
```
