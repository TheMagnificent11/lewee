using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Pizzeria.Store.WebClient.Pages;

public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

    public void OnGet()
    {
        this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    }
}
