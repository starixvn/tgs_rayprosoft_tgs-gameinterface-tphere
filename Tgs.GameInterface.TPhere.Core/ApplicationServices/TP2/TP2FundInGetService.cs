using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2FundInGetService(
	IGameApiClient client) : IGetService<TP2FundInQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(TP2FundInQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.TransactionWithdrawAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			account: query.PlayerCode,
			transactionId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundResult(
			TransactionId: result.TransactionId.Replace(string.Concat(result.DestinationAccount, "_"), string.Empty),
			CasinoTransid: result.CasinoTransid,
			Balance: result.BalanceAfter,
			Res: result.Res);
	}
}
