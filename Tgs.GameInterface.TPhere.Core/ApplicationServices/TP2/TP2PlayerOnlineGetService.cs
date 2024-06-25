using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2PlayerOnlineGetService(
	IGameApiClient client) : IGetService<TP2PlayerOnlineQuery, bool>
{
	public ValueTask<bool> GetAsync(TP2PlayerOnlineQuery query, CancellationToken cancellationToken = default)
		=> client.PlayerIsGamingAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			account: query.PlayerCode,
			cancellationToken: cancellationToken);
}
