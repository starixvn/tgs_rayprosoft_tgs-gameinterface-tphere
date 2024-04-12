using Tgs.GameInterface.TPhere.Protos;

namespace Tgs.GameInterface.TPhere.Client;

public interface IGameInterfaceTPhereClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);
	ValueTask<bool> FundStatusAsync(Uri apiUrl, string apiKey, string apiToken, string orderId, CancellationToken cancellationToken = default);
	ValueTask<FundReply> TP2FundInAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);
	ValueTask<FundReply> TP2FundOutAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);
	ValueTask<GameUrlReply> TP2GameUrlAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string gameCode, bool isMobile, string language, CancellationToken cancellationToken = default);
	ValueTask<string> TP2GameWagerDetailAsync(Uri apiUrl, string apiKey, string apiToken, string externalWagerId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default);
	IAsyncEnumerable<GameWagerInfo> TP2GameWagerBetLogAsync(Uri apiUrl, string apiKey, string apiToken, DateTimeOffset startTime, DateTimeOffset endTime, int page, int pageSize, CancellationToken cancellationToken = default);
	IAsyncEnumerable<GameWagerInfo> TP2GameWagerBetLogByBettimeAsync(Uri apiUrl, string apiKey, string apiToken, DateTimeOffset startTime, DateTimeOffset endTime, int page, int pageSize, CancellationToken cancellationToken = default);
	ValueTask<PlayerBalanceReply> TP2PlayerBalanceAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default);
	ValueTask TP2PlayerCreateAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default);
	ValueTask TP2PlayerLogoutAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default);
}
