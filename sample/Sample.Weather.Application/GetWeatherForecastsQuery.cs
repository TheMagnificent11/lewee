using MediatR;
using Saji.Application.Mediation;
using Sample.Weather.Domain;

namespace Sample.Weather.Application;

public class GetWeatherForecastsQuery : IQuery<IEnumerable<WeatherForecast>>
{
    public GetWeatherForecastsQuery(Guid correlationId)
    {
        this.CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
    public Guid? TenantId { get; }

    internal class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>
    {
        public Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastsQuery request, CancellationToken cancellationToken)
        {
            var range = new Random();

            var result = Enumerable
                .Range(1, 5)
                .Select(item => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(item)),
                    range.Next(-20, 55)));

            return Task.FromResult(result);
        }
    }
}
