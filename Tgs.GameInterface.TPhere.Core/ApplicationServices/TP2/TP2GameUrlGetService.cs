using Tgs.GameInterface.TPhere.Core.Models;
using Tgs.ServiceUtility;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2GameUrlGetService(
	IGameApiClient client) : IGetService<TP2GameUrlQuery, GameUrlResult>
{
	public async ValueTask<GameUrlResult> GetAsync(TP2GameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.GameLinkAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			gamecode: query.GameCode,
			account: query.PlayerCode,
			lang: query.Language,
			isMobile: query.IsMobile,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameUrlResult(
			IsPlayerExist: result.IsPlayerExist,
			GameUrl: result.GameUrl);
	}
}
