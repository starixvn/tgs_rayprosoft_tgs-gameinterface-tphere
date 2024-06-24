using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

using Tgs.GameInterface.TPhere.Core.Exceptions;
using Tgs.GameInterface.TPhere.Core.Extensions;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core;

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	IOptions<PolicySettingsOptions> options,
	ICryptographyUtensil cryptographyUtensil,
	HttpClient httpClient) : IGameApiClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async IAsyncEnumerable<BetLogResult> BetLogAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, int pageSize = 5000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(BetLogAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetAsync(
				apiUrl: new Uri(apiUrl, "/api/betlog"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "gamehall", gamehall },
					{ "start_time", startTime.DatetimeConvertToString()},
					{ "end_time", endTime.DatetimeConvertToString() },
					{ "page", page.ToString() },
					{ "page_size", pageSize.ToString() }
				},
				logPrefix: nameof(BetLogAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		if (jsonNode["status"]?["code"]?.GetValue<int>() != 0)
		{
			throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>());
		}

		var pageResult = jsonNode["data"]!["page_result"]!.AsArray();

		if (pageResult is null)
		{
			yield break;
		}

		foreach (var result in pageResult)
		{
			if (result is null)
			{
				continue;
			}

			yield return new BetLogResult(
				Hall: result["hall"]!.GetValue<string>(),
				Category: result["category"]!.GetValue<string>(),
				Rowid: result["rowid"]!.GetValue<string>(),
				Round: result["round"]!.GetValue<string>(),
				CasinoAccount: result["casino_account"]!.GetValue<string>(),
				Betvalid: decimal.Parse(result["betvalid"]!.GetValue<string>()),
				BetAmount: decimal.Parse(result["betamount"]!.GetValue<string>()),
				Betresult: decimal.Parse(result["betresult"]!.GetValue<string>()),
				PcaContribute: decimal.Parse(result["pca_contribute"]!.GetValue<string>()),
				PcaWin: decimal.Parse(result["pca_win"]!.GetValue<string>()),
				Revenue: decimal.Parse(result["revenue"]!.GetValue<string>()),
				Bettime: result["bettime"]!.ToString().StringConvertToDatetime(),
				PayoutTime: result["payout_time"] is not null ? result["payout_time"]!.ToString().StringConvertToDatetime() : DateTimeOffset.UnixEpoch,
				Reporttime: result["reporttime"]!.ToString().StringConvertToDatetime(),
				Status: (BetLogStatus)int.Parse(result["status"]!.GetValue<string>()),
				GameId: result["gameid"]!.GetValue<string>(),
				Trace: result["trace"]!.GetValue<bool>(),
				Freegame: result["freegame"]?.GetValue<int>() ?? 0,
				AdditionalData: result.ToJsonString());
		}
	}

	public async IAsyncEnumerable<BetLogResult> BetLogByBettimeAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, int pageSize = 5000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(BetLogByBettimeAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetAsync(
				apiUrl: new Uri(apiUrl, "/api/betlog-by-bettime"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "gamehall", gamehall },
					{ "start_time", startTime.DatetimeConvertToString()},
					{ "end_time", endTime.DatetimeConvertToString() },
					{ "page", page.ToString() },
					{ "page_size", pageSize.ToString() }
				},
				logPrefix: nameof(BetLogByBettimeAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		if (jsonNode["status"]?["code"]?.GetValue<int>() != 0)
		{
			throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>());
		}

		var pageResult = jsonNode["data"]!["page_result"]!.AsArray();

		if (pageResult is null)
		{
			yield break;
		}

		foreach (var result in pageResult)
		{
			if (result is null)
			{
				continue;
			}

			yield return new BetLogResult(
				Hall: result["hall"]!.GetValue<string>(),
				Category: result["category"]!.GetValue<string>(),
				Rowid: result["rowid"]!.GetValue<string>(),
				Round: result["round"]!.GetValue<string>(),
				CasinoAccount: result["casino_account"]!.GetValue<string>(),
				Betvalid: decimal.Parse(result["betvalid"]!.GetValue<string>()),
				BetAmount: decimal.Parse(result["betamount"]!.GetValue<string>()),
				Betresult: decimal.Parse(result["betresult"]!.GetValue<string>()),
				PcaContribute: decimal.Parse(result["pca_contribute"]!.GetValue<string>()),
				PcaWin: decimal.Parse(result["pca_win"]!.GetValue<string>()),
				Revenue: decimal.Parse(result["revenue"]!.GetValue<string>()),
				Bettime: result["bettime"]!.ToString().StringConvertToDatetime(),
				PayoutTime: result["payout_time"] is not null ? result["payout_time"]!.ToString().StringConvertToDatetime() : DateTimeOffset.UnixEpoch,
				Reporttime: result["reporttime"]!.ToString().StringConvertToDatetime(),
				Status: (BetLogStatus)int.Parse(result["status"]!.GetValue<string>()),
				GameId: result["gameid"]!.GetValue<string>(),
				Trace: result["trace"]!.GetValue<bool>(),
				Freegame: result["freegame"]?.GetValue<int>() ?? 0,
				AdditionalData: result.ToJsonString());
		}
	}

	public async ValueTask<string> BetLogPlayCheckAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string betId, DateTimeOffset betTime, string lang = "zh-tw", CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(BetLogPlayCheckAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetBetLogPlayCheckAsync(
				apiUrl: new Uri(apiUrl, "/api/betlog/playcheck"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "gamehall",gamehall },
					{ "betID",betId },
					{ "bet_time", betTime.DatetimeConvertToString()},
					{ "lang", lang}
				},
				logPrefix: nameof(BetLogPlayCheckAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
			: jsonNode["data"]?["playCheck"]?.ToString() ?? string.Empty;
	}

	public async ValueTask<GameLinkResult> GameLinkAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string gamecode, string account, string lang = "zh-tw", bool isMobile = false, string sub_gamehall = "", string return_url = "", int isFreeTrial = 0, string registerUrl = "", string roomId = "", string agentId = "", string bounsmode = "", CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(GameLinkAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetAsync(
				apiUrl: new Uri(apiUrl, "/api/game/game-link"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "gamehall", gamehall },
					{ "gamecode",gamecode },
					{ "account",account },
					{ "lang",lang },
					{ "platform",isMobile? "mobile":"web"},
					{ "sub_gamehall",sub_gamehall },
					{ "return_url",return_url },
					{ "is_free_trial",isFreeTrial.ToString() },
					{ "register_url",registerUrl },
					{ "room_id",roomId },
					{ "agent_id",agentId },
					{ "bounsmode",bounsmode }
				},
				logPrefix: nameof(GameLinkAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? jsonNode["status"]?["code"]?.GetValue<int>() != 102
				? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
				: new GameLinkResult(false, string.Empty)
			: new GameLinkResult(true, jsonNode["data"]?["url"]?.GetValue<string>() ?? string.Empty);
	}

	public async ValueTask PlayerCreatedAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(PlayerCreatedAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => PostAsync(
				apiUrl: new Uri(apiUrl, "/api/player"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "account",account },
					{ "password","tgspassw0rd" },
					{ "nickname",account }
				},
				logPrefix: nameof(PlayerCreatedAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		if (jsonNode["status"]?["code"]?.GetValue<int>() != 0)
		{
			if (jsonNode["status"]?["code"]?.GetValue<int>() != 101)
			{
				throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>());
			}
		}
	}

	public async ValueTask PlayerLogoutAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(PlayerLogoutAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => PostAsync(
				apiUrl: new Uri(apiUrl, "/api/player/logout"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "account",account },
					{ "gamehall", gamehall }
				},
				logPrefix: nameof(PlayerLogoutAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		if (jsonNode["status"]?["code"]?.GetValue<int>() != 0)
		{
			if (jsonNode["status"]?["code"]?.GetValue<int>() != 102)
			{
				throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>());
			}
		}
	}

	public async ValueTask<PlayerWalletResult> PlayerWalletAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(PlayerWalletAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetAsync(
				apiUrl: new Uri(apiUrl, "/api/player/wallet"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "account",account },
					{ "gamehall", gamehall }
				},
				logPrefix: nameof(PlayerWalletAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? jsonNode["status"]?["code"]?.GetValue<int>() != 102
				? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
				: new PlayerWalletResult(false, 0.0m)
			: new PlayerWalletResult(true, decimal.Parse(jsonNode["data"]?["balance"]?.ToString() ?? "0"));
	}

	public async ValueTask<bool> TransactionAsync(Uri apiUrl, string apiKey, string apiToken, string transactionId, CancellationToken cancellationToken = default)
	{
		transactionId = transactionId.StringLength(4, 30);

		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.GlobalRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(TransactionAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => GetTransactionAsync(
				apiUrl: new Uri(apiUrl, $"/api/transaction/{transactionId}"),
				apiKey: apiKey,
				apiToken: apiToken,
				logPrefix: nameof(TransactionAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? jsonNode["status"]?["code"]?.GetValue<int>() != 303
				? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
				: false
			: jsonNode["data"]!["status"]!.ToString().Contains("success", StringComparison.OrdinalIgnoreCase)
			|| (jsonNode["data"]!["status"]!.ToString().Contains("processing", StringComparison.OrdinalIgnoreCase)
				? throw new Exception($"The status of this transaction number is processing.({transactionId})")
				: false);
	}

	public async ValueTask<TransactionResult> TransactionDepositAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, string transactionId, decimal amount, CancellationToken cancellationToken = default)
	{
		transactionId = transactionId.StringLength(4, 30);

		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.DepositRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.DepositRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(TransactionDepositAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => PostAsync(
				apiUrl: new Uri(apiUrl, "/api/transaction/deposit"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "account",account },
					{ "gamehall", gamehall },
					{ "transaction_id", transactionId },
					{ "amount", amount.ToString("0.00") }
				},
				logPrefix: nameof(TransactionDepositAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
			: new TransactionResult(
				Type: jsonNode["data"]!["type"]!.ToString(),
				TransactionId: jsonNode["data"]!["transaction_id"]!.ToString(),
				SourceAccount: jsonNode["data"]!["source_account"]!.ToString(),
				DestinationAccount: jsonNode["data"]!["destination_account"]!.ToString(),
				Amount: decimal.Parse(jsonNode["data"]!["amount"]!.ToString()),
				Casino: jsonNode["data"]!["casino"]!.ToString(),
				UpdatedAt: jsonNode["data"]!["updated_at"]!.ToString().StringConvertToDatetime(),
				CreatedAt: jsonNode["data"]!["created_at"]!.ToString().StringConvertToDatetime(),
				BalanceBefore: decimal.Parse(jsonNode["data"]!["balance_before"]!.ToString()),
				BalanceAfter: decimal.Parse(jsonNode["data"]!["balance_after"]!.ToString()),
				CasinoTransid: jsonNode["data"]!["casino_transid"]!.ToString(),
				Status: jsonNode["data"]!["status"]!.ToString(),
				Res: jsonNode["data"]!.ToJsonString());
	}

	public async ValueTask<TransactionResult> TransactionWithdrawAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, string transactionId, decimal amount, CancellationToken cancellationToken = default)
	{
		transactionId = transactionId.StringLength(4, 30);

		var retryPolicy = Policy.HandleResult<JsonNode>(jsonNode => jsonNode["status"]?["code"]?.GetValue<int>() == 7)
			.WaitAndRetryAsync(
				retryCount: _policySettingsOptions.WithdrawRetryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.WithdrawRetryWaitSec, retryAttempt)),
				onRetry: (response, retryTime, context) => logger.LogWarning("{logWarningInfo}",
						new
						{
							LogOn = DateTime.Now.ToString("o"),
							LogPrefix = nameof(TransactionWithdrawAsync),
							Message = "Exceed api call limit.",
							Resp = response.ToString()
						}));

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => PostAsync(
				apiUrl: new Uri(apiUrl, "/api/transaction/withdraw"),
				apiKey: apiKey,
				apiToken: apiToken,
				parameters: new SortedDictionary<string, string>
				{
					{ "account",account },
					{ "gamehall", gamehall },
					{ "transaction_id", transactionId },
					{ "amount", amount.ToString("0.00") }
				},
				logPrefix: nameof(TransactionWithdrawAsync),
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
			: new TransactionResult(
				Type: jsonNode["data"]!["type"]!.ToString(),
				TransactionId: jsonNode["data"]!["transaction_id"]!.ToString(),
				SourceAccount: jsonNode["data"]!["source_account"]!.ToString(),
				DestinationAccount: jsonNode["data"]!["destination_account"]!.ToString(),
				Amount: decimal.Parse(jsonNode["data"]!["amount"]!.ToString()),
				Casino: jsonNode["data"]!["casino"]!.ToString(),
				UpdatedAt: jsonNode["data"]!["updated_at"]!.ToString().StringConvertToDatetime(),
				CreatedAt: jsonNode["data"]!["created_at"]!.ToString().StringConvertToDatetime(),
				BalanceBefore: decimal.Parse(jsonNode["data"]!["balance_before"]!.ToString()),
				BalanceAfter: decimal.Parse(jsonNode["data"]!["balance_after"]!.ToString()),
				CasinoTransid: jsonNode["data"]!["casino_transid"]!.ToString(),
				Status: jsonNode["data"]!["status"]!.ToString(),
				Res: jsonNode["data"]!.ToJsonString());
	}

	private async ValueTask<JsonNode> GetTransactionAsync(Uri apiUrl, string apiKey, string apiToken, string logPrefix, CancellationToken cancellationToken = default)
	{
		var sign = cryptographyUtensil.Md5Encrypt(apiKey);
		var builder = new UriBuilder(apiUrl);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["sign"] = sign;
		builder.Query = query.ToString();

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(apiToken);

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await httpClient.GetAsync(
			requestUri: builder.ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = string.Empty,
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}

	private async ValueTask<JsonNode> GetBetLogPlayCheckAsync(Uri apiUrl, string apiKey, string apiToken, IDictionary<string, string> parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var key = string.Concat("betID", "=", parameters["betID"], "&", "bet_time", "=", parameters["bet_time"], "&", "gamehall", "=", parameters["gamehall"], "&", "lang", "=", parameters["lang"]);
		//var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = cryptographyUtensil.Md5Encrypt(key + apiKey);
		var builder = new UriBuilder(apiUrl);
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(apiToken);

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await httpClient.GetAsync(
			requestUri: builder.ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = string.Empty,
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}

	private async ValueTask<JsonNode> GetAsync(Uri apiUrl, string apiKey, string apiToken, IDictionary<string, string> parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = cryptographyUtensil.Md5Encrypt(key + apiKey);
		var builder = new UriBuilder(apiUrl);
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(apiToken);

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await httpClient.GetAsync(
			requestUri: builder.ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = string.Empty,
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}

	private async ValueTask<JsonNode> PostAsync(Uri apiUrl, string apiKey, string apiToken, IDictionary<string, string> parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = cryptographyUtensil.Md5Encrypt(key + apiKey);

		if (!parameters.TryAdd("sign", sign))
		{
			parameters["sign"] = sign;
		}

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(apiToken);

		var requestContent = new FormUrlEncodedContent(parameters);

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await httpClient.PostAsync(
			requestUri: apiUrl,
			content: requestContent,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = (await requestContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}
}
