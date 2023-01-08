using Microsoft.AspNetCore.Components;

namespace Sample.Restaurant.App.Pages;
public partial class Table
{
    [Parameter]
    public int TableNumber { get; set; }
}
