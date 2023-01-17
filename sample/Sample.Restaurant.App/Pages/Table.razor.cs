﻿using Microsoft.AspNetCore.Components;
using Sample.Restaurant.Application.Menu;

namespace Sample.Restaurant.App.Pages;
public partial class Table
{
    private MenuItemDto[]? menuItems;

    [Parameter]
    public int TableNumber { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await this.Mediator.Send(new GetMenuItemsQuery(Guid.NewGuid()));

        if (!result.IsSuccess || result.Data == null)
        {
            // TODO
            return;
        }

        this.menuItems = result.Data.ToArray();
    }
}