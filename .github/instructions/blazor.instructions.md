---
applyTo: "**/*.razor,**/*.razor.cs"
---

# Blazor Component Development Guidelines

## Code-Behind Pattern

- Use code-behind pattern with partial classes for Razor components
- Create a separate `.razor.cs` file for component logic
- Do not add `@code` blocks directly in `.razor` files, use code-behind instead (partial classes named `[ComponentName].razor.cs`)
- All other `@` directives (e.g. `@attribute`, `@inherits`, `@inject`, `@using`, etc.) should remain in the `.razor` file

## Example Structure

### Component File (`Home.razor`)
```razor
@page "/"

@using Pizzeria.Store.Web.States.Orders

@attribute [Microsoft.AspNetCore.Authorization.Authorize]
@inherits FluxorComponent

@inject IState<OrdersState> State
@inject IDispatcher Dispatcher
@inject NavigationManager NavigationManager

<PageTitle>Pizzeria Store</PageTitle>

<!-- Component markup here -->
```

### Code-Behind File (`Home.razor.cs`)
```csharp
using Pizzeria.Store.Web.States.Orders.Actions;

namespace Pizzeria.Store.Web.Pages;

public partial class Home
{
    private void StartNewOrder()
    {
        this.Dispatcher.Dispatch(new StartOrderAction());
    }

    private void ClearError()
    {
        this.Dispatcher.Dispatch(new ClearOrderErrorAction());
    }
}
```

## Reference Implementation

See `sample/Pizzeria.Store.Web/Pages/Home.razor` and `Home.razor.cs` for a complete example of the code-behind pattern in action.

## Testing Standards

### bUnit/Playwright Testing

- Do not use magic strings for selectors
- Expose a constant from the component and use that instead

### Example

```csharp
// In the component
public partial class MyComponent
{
    public const string SubmitButtonTestId = "submit-button";
}

// In the test
var button = cut.Find($"[data-testid='{MyComponent.SubmitButtonTestId}']");
```
