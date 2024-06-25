using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core;

public interface IGameApiClient
{
	ValueTask PlayerCreatedAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default);

	ValueTask<PlayerWalletResult> PlayerWalletAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerIsGamingAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default);

	ValueTask<TransactionResult> TransactionWithdrawAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, string transactionId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<TransactionResult> TransactionDepositAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, string transactionId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> TransactionAsync(Uri apiUrl, string apiKey, string apiToken, string transactionId, CancellationToken cancellationToken = default);

	ValueTask<GameLinkResult> GameLinkAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string gamecode, string account, string lang = "zh-tw", bool isMobile = false, string sub_gamehall = "", string return_url = "", int isFreeTrial = 0, string registerUrl = "", string roomId = "", string agentId = "", string bounsmode = "", CancellationToken cancellationToken = default);

	IAsyncEnumerable<BetLogResult> BetLogAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, int pageSize = 5000, CancellationToken cancellationToken = default);

	IAsyncEnumerable<BetLogResult> BetLogByBettimeAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, int pageSize = 5000, CancellationToken cancellationToken = default);

	ValueTask<string> BetLogPlayCheckAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string betId, DateTimeOffset betTime, string lang = "zh-tw", CancellationToken cancellationToken = default);
}
