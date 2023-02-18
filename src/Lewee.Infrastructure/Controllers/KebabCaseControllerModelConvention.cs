using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Lewee.Infrastructure.Controllers;

/// <summary>
/// Kabab-Case Controller Model Convention
/// </summary>
public class KebabCaseControllerModelConvention : IControllerModelConvention
{
    /// <inheritdoc />
    public void Apply(ControllerModel controller)
    {
        var camels = controller.ControllerName
            .SplitCamelCase()
            .Select(x => x.ToLower());

        controller.ControllerName = string.Join("-", camels);
    }
}
