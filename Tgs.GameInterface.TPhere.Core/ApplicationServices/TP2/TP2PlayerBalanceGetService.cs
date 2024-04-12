using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2PlayerBalanceGetService(
	IGameApiClient client) : IGetService<TP2PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(TP2PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.PlayerWalletAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			account: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceResult(
			IsPlayerExist: result.IsPlayerExist,
			Balance: result.Balance);
	}
}
