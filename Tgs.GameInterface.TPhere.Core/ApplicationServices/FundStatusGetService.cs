using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices;

internal class FundStatusGetService(IGameApiClient client) : IGetService<FundStatusQuery, bool>
{
	public ValueTask<bool> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
		=> client.TransactionAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			transactionId: query.OrderId,
			cancellationToken: cancellationToken);
}
