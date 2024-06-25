using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.TPhere.Protos;

namespace Tgs.GameInterface.TPhere.Client;

internal class GameInterfaceTPhereClient(
	GameInterfaceTPhere.GameInterfaceTPhereClient client) : IGameInterfaceTPhereClient
{
	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
	{
		var request = new CheckingServiceRequest
		{
			MachineName = machineName,
		};

		var reply = await client.CheckingServiceAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask<bool> FundStatusAsync(Uri apiUrl, string apiKey, string apiToken, string orderId, CancellationToken cancellationToken = default)
	{
		var request = new FundStatusRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			OrderId = orderId,
		};

		var reply = await client.FundStatusAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.State;
	}

	public async ValueTask<FundReply> TP2FundInAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode,
			OrderId = orderId,
			Amount = amount,
		};

		var reply = await client.TP2FundInAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			TransactionId = reply.TransactionId,
			CasinoTransId = reply.CasinoTransId,
			Balance = reply.Balance,
			Res = reply.Res,
		};
	}

	public async ValueTask<FundReply> TP2FundOutAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode,
			OrderId = orderId,
			Amount = amount,
		};

		var reply = await client.TP2FundOutAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			TransactionId = reply.TransactionId,
			CasinoTransId = reply.CasinoTransId,
			Balance = reply.Balance,
			Res = reply.Res,
		};
	}

	public async ValueTask<GameUrlReply> TP2GameUrlAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, string gameCode, bool isMobile, string language, CancellationToken cancellationToken = default)
	{
		var request = new GameUrlRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode,
			GameCode = gameCode,
			IsMobile = isMobile,
			Language = language,
		};

		var reply = await client.TP2GameUrlAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameUrlReply
		{
			IsPlayerExist = reply.IsPlayerExist,
			GameUrl = reply.GameUrl
		};
	}

	public async IAsyncEnumerable<GameWagerInfo> TP2GameWagerBetLogAsync(Uri apiUrl, string apiKey, string apiToken, DateTimeOffset startTime, DateTimeOffset endTime, int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameWagerRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp(),
			Page = page,
			PageSize = pageSize
		};

		var reply = await client.TP2GameWagerBetLogAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var items = reply.Items ?? Enumerable.Empty<GameWagerInfo>();

		if (!items.Any())
		{
			yield break;
		}

		foreach (var item in items)
		{
			yield return item;
		}
	}

	public async IAsyncEnumerable<GameWagerInfo> TP2GameWagerBetLogByBettimeAsync(Uri apiUrl, string apiKey, string apiToken, DateTimeOffset startTime, DateTimeOffset endTime, int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameWagerRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp(),
			Page = page,
			PageSize = pageSize
		};

		var reply = await client.TP2GameWagerBetLogByBettimeAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var items = reply.Items ?? Enumerable.Empty<GameWagerInfo>();

		if (!items.Any())
		{
			yield break;
		}

		foreach (var item in items)
		{
			yield return item;
		}
	}

	public async ValueTask<string> TP2GameWagerDetailAsync(Uri apiUrl, string apiKey, string apiToken, string externalWagerId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default)
	{
		var request = new GameWagerDetailRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			ExternalWagerId = externalWagerId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			Language = language
		};

		var reply = await client.TP2GameWagerDetailAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.GameWagerDetailUrl;
	}

	public async ValueTask<PlayerBalanceReply> TP2PlayerBalanceAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode
		};

		var reply = await client.TP2PlayerBalanceAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceReply
		{
			IsPlayerExist = reply.IsPlayerExist,
			Balance = reply.Balance
		};
	}

	public async ValueTask TP2PlayerCreateAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode
		};

		_ = await client.TP2PlayerCreateAsync(
		   request: request,
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask TP2PlayerLogoutAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerLogoutRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode
		};

		_ = await client.TP2PlayerLogoutAsync(
		   request: request,
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask<bool> TP2PlayerOnlineAsync(Uri apiUrl, string apiKey, string apiToken, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerOnlineRequest
		{
			ApiUrl = apiUrl.ToString(),
			ApiKey = apiKey,
			ApiToken = apiToken,
			PlayerCode = playerCode
		};

		var reply = await client.TP2PlayerOnlineAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.Online;
	}
}
