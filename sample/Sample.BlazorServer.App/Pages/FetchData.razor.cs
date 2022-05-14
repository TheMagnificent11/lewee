using MediatR;
using Microsoft.AspNetCore.Components;
using Sample.Weather.Application;
using Sample.Weather.Domain;

namespace Sample.BlazorServer.App.Pages;

public partial class FetchData
{
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
    private WeatherForecast[]? forecasts = null;
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

    [Inject]
    private IMediator? Mediator { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (this.Mediator == null)
        {
            this.forecasts = Array.Empty<WeatherForecast>();
            return;
        }

        var result = await this.Mediator.Send(new GetWeatherForecastsQuery(Guid.NewGuid()));

        this.forecasts = result.ToArray();
    }
}
