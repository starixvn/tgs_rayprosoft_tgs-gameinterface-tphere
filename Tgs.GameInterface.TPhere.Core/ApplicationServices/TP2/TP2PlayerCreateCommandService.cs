using Tgs.GameInterface.TPhere.Core;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2PlayerCreateCommandService(IGameApiClient client) : ICommandService<TP2PlayerCreateCommand>
{
	public ValueTask ExecuteAsync(TP2PlayerCreateCommand command, CancellationToken cancellationToken = default)
		=> client.PlayerCreatedAsync(
			apiUrl: command.ApiUrl,
			apiKey: command.ApiKey,
			apiToken: command.ApiToken,
			gamehall: Gamehall.TP2,
			account: command.PlayerCode,
			cancellationToken: cancellationToken);
}
