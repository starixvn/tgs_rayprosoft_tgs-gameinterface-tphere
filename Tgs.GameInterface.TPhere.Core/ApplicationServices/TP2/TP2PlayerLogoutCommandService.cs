using Tgs.GameInterface.TPhere.Core;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2PlayerLogoutCommandService(IGameApiClient client) : ICommandService<TP2PlayerLogoutCommand>
{
	public ValueTask ExecuteAsync(TP2PlayerLogoutCommand command, CancellationToken cancellationToken = default)
		=> client.PlayerLogoutAsync(
			apiUrl: command.ApiUrl,
			apiKey: command.ApiKey,
			apiToken: command.ApiToken,
			gamehall: Gamehall.TP2,
			account: command.PlayerCode,
			cancellationToken: cancellationToken);
}
