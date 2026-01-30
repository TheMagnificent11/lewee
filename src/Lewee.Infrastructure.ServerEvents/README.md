# Lewee.Infrastructure.ServerEvents

Infrastructure utilities for [ASP.NET Core Web API Server-Sent Events](https://www.milanjovanovic.tech/blog/server-sent-events-in-aspnetcore-and-dotnet-10).

## Usage

### SSE Endpoint

```csharp
// In Program.cs
builder.Services.AddClientEventChannel();

var app = builder.Build();
app.MapSseEndpoint();
```
