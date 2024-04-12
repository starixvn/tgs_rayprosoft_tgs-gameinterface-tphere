using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2FundOutGetService(
	IGameApiClient client) : IGetService<TP2FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(TP2FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var result = await client.TransactionDepositAsync(
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
			TransactionId: result.TransactionId.Replace(string.Concat(result.SourceAccount, "_"), string.Empty),
			CasinoTransid: result.CasinoTransid,
			Balance: result.BalanceAfter,
			Res: result.Res);
	}
}
