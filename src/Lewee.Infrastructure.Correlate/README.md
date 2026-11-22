# Lewee.Infrastructure.Correlate

This package assits in configuring [Correlate](https://github.com/skwasjer/Correlate) using the correlation ID header constant in the `RequestHeaders` static class [Lewee.Common](../Lewee.Common/Lewee.Common.csproj).

## Depedencies

- [Correlate.AspNetCore](https://github.com/skwasjer/Correlate)
- [Lewee.Common](../Lewee.Common/Lewee.Common.csproj)

## Configuration

### Correlation ID Configuration

Configure correlation ID logging to track requests across your application:

```cs
services.AddCorrelationIdServices();
```

```cs
app.UseCorrelationIdMiddleware();
```
