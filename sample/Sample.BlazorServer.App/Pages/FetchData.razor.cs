using Microsoft.AspNetCore.Components;
using Sample.Identity.Infrastructure.Data;

namespace Sample.BlazorServer.App.Pages;

public partial class FetchData
{
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
    private WeatherForecast[]? forecasts = null;
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

    [Inject]
    private WeatherForecastService? ForecastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (this.ForecastService == null)
        {
            this.forecasts = Array.Empty<WeatherForecast>();
            return;
        }

        this.forecasts = await this.ForecastService.GetForecastAsync(DateTime.Now);
    }
}
