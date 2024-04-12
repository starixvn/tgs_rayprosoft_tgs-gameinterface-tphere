using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2GameWagerDetailGetService(
	IGameApiClient client) : IGetService<TP2GameWagerDetailQuery, string>
{
	public ValueTask<string> GetAsync(TP2GameWagerDetailQuery query, CancellationToken cancellationToken = default)
		=> client.BetLogPlayCheckAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			betId: query.ExternalWagerId,
			betTime: query.ExternalCreatedOn,
			lang: query.Language,
			cancellationToken: cancellationToken);
}
