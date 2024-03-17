﻿using Fluxor;

namespace Sample.Pizzeria.Client.Orders;

public sealed class GetOrdersStateFeature : Feature<GetOrdersState>
{
    public override string GetName() => nameof(GetOrdersState);

    protected override GetOrdersState GetInitialState() => new();
}
