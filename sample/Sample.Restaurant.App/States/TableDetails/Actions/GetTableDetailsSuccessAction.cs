﻿using Lewee.Fluxor.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetailsSuccessAction : IQuerySuccessAction<TableDetailsDto>
{
    public GetTableDetailsSuccessAction(TableDetailsDto data)
    {
        this.Data = data;
    }

    public TableDetailsDto Data { get; }
}