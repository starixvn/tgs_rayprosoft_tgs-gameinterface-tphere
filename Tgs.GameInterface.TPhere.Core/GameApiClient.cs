using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TPhere.Core.Models;
using System.Web;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using Tgs.GameInterface.TPhere.Core.Extensions;
using Tgs.GameInterface.TPhere.Core.Exceptions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

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
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", startTime.DatetimeConvertToString()},
			{ "end_time", endTime.DatetimeConvertToString() },
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		apiUrl = new Uri(apiUrl, "/api/betlog");

		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "player_wallet",
			cancellationToken: cancellationToken)
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
				PayoutTime: result["payout_time"]!.ToString().StringConvertToDatetime(),
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
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", startTime.DatetimeConvertToString()},
			{ "end_time", endTime.DatetimeConvertToString() },
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		apiUrl = new Uri(apiUrl, "/api/betlog-by-bettime");

		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "player_wallet",
			cancellationToken: cancellationToken)
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
				PayoutTime: result["payout_time"]!.ToString().StringConvertToDatetime(),
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
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall",gamehall },
			{ "betID",betId },
			{ "bet_time", betTime.DatetimeConvertToString()},
			{ "lang", lang}
		};
		apiUrl = new Uri(apiUrl, "/api/betlog/playcheck");

		var jsonNode = await GetBetLogPlayCheckAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "betlog_playcheck",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
			: jsonNode["data"]?["playCheck"]?.ToString() ?? string.Empty;
	}

	public async ValueTask<GameLinkResult> GameLinkAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string gamecode, string account, string lang = "zh-tw", bool isMobile = false, string sub_gamehall = "", string return_url = "", int isFreeTrial = 0, string registerUrl = "", string roomId = "", string agentId = "", string bounsmode = "", CancellationToken cancellationToken = default)
	{
		var parameters = new SortedDictionary<string, string>
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
		};

		apiUrl = new Uri(apiUrl, "/api/game/game-link");

		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "game_game-link",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return jsonNode["status"]?["code"]?.GetValue<int>() != 0
			? jsonNode["status"]?["code"]?.GetValue<int>() != 102
				? throw new ErrorCodeException(jsonNode["status"]!["code"]!.GetValue<int>())
				: new GameLinkResult(false, string.Empty)
			: new GameLinkResult(true, jsonNode["data"]?["url"]?.GetValue<string>() ?? string.Empty);
	}

	public async ValueTask PlayerCreatedAsync(Uri apiUrl, string apiKey, string apiToken, string gamehall, string account, CancellationToken cancellationToken = default)
	{
		var parameters = new SortedDictionary<string, string>
		{
			{ "account",account },
			{ "password","tgspassw0rd" },
			{ "nickname",account }
		};
		apiUrl = new Uri(apiUrl, "/api/player");

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "player_created",
			cancellationToken: cancellationToken)
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
		var parameters = new SortedDictionary<string, string>
		{
			{ "account",account },
			{ "gamehall", gamehall }
		};
		apiUrl = new Uri(apiUrl, "/api/player/logout");

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "player_logout",
			cancellationToken: cancellationToken)
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
		var parameters = new SortedDictionary<string, string>
		{
			{ "account",account },
			{ "gamehall", gamehall }
		};
		apiUrl = new Uri(apiUrl, "/api/player/wallet");

		var jsonNode = await GetAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "player_wallet",
			cancellationToken: cancellationToken)
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

		apiUrl = new Uri(apiUrl, $"/api/transaction/{transactionId}");

		var jsonNode = await GetTransactionAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			logPrefix: "transaction_status",
			cancellationToken: cancellationToken)
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

		var parameters = new SortedDictionary<string, string>
		{
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};
		apiUrl = new Uri(apiUrl, "/api/transaction/deposit");

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "transaction_deposit",
			cancellationToken: cancellationToken)
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

		var parameters = new SortedDictionary<string, string>
		{
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};
		apiUrl = new Uri(apiUrl, "/api/transaction/withdraw");

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			parameters: parameters,
			logPrefix: "transaction_withdraw",
			cancellationToken: cancellationToken)
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
		logPrefix = $"{nameof(GameApiClient).ToLower()}.{logPrefix}";

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
		logPrefix = $"{nameof(GameApiClient).ToLower()}.{logPrefix}";

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
		logPrefix = $"{nameof(GameApiClient).ToLower()}.{logPrefix}";

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
		logPrefix = $"{nameof(GameApiClient).ToLower()}.{logPrefix}";

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
