using System.Diagnostics.CodeAnalysis;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;

namespace Pizzeria.Store.Web.Infrastructure.ServerSentEvents;

public interface IMessageToActionMapper
{
    IMessageReceivedAction? Map([NotNull] object message, Guid correlationId);
}
