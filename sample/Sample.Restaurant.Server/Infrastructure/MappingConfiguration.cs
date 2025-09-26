using Mapster;
using MapsterMapper;

namespace Sample.Restaurant.Server.Configuration;

internal static class MappingConfiguration
{
    public static TypeAdapterConfig AddMapper(this IServiceCollection services)
    {
        services.AddTransient<IMapper, ServiceMapper>();

        var config = new TypeAdapterConfig();
        services.AddSingleton(config);

        return config;
    }
}
