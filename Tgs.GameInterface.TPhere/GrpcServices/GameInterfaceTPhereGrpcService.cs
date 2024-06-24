using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.GrpcServices;

public class GameInterfaceTPhereGrpcService(
	ILogger<GameInterfaceTPhereGrpcService> logger,
	IGetService<TP2FundInQuery, FundResult> tp2FundInService,
	IGetService<TP2FundOutQuery, FundResult> tp2FundOutService,
	IGetService<FundStatusQuery, bool> fundStatusService,
	IGetService<TP2GameUrlQuery, GameUrlResult> tp2GameUrlService,
	IQueryService<TP2GameWagerBetLogByBettimeQuery, GameWagerResult> tp2GameWagerBetLogByBettimeService,
	IQueryService<TP2GameWagerBetLogQuery, GameWagerResult> tp2GameWagerBetLogService,
	IGetService<TP2GameWagerDetailQuery, string> tp2GameWagerDetailService,
	IGetService<TP2PlayerBalanceQuery, PlayerBalanceResult> tp2PlayerBalanceService,
	ICommandService<TP2PlayerCreateCommand> tp2PlayerCreateService,
	ICommandService<TP2PlayerLogoutCommand> tp2PlayerLogoutService) : GameInterfaceTPhere.GameInterfaceTPhereBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

		return Task.FromResult(new CheckingServiceReply
		{
			MachineName = Environment.MachineName
		});
	}

	public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
	{
		var query = new FundStatusQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			OrderId: request.OrderId);

		var result = await fundStatusService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply()
		{
			State = result
		};
	}

	public override async Task<FundReply> TP2FundIn(FundRequest request, ServerCallContext context)
	{
		var query = new TP2FundInQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode,
			OrderId: request.OrderId,
			Amount: request.Amount);

		var result = await tp2FundInService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply()
		{
			TransactionId = result.TransactionId,
			CasinoTransId = result.CasinoTransid,
			Balance = result.Balance,
			Res = result.Res
		};
	}

	public override async Task<FundReply> TP2FundOut(FundRequest request, ServerCallContext context)
	{
		var query = new TP2FundOutQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode,
			OrderId: request.OrderId,
			Amount: request.Amount);

		var result = await tp2FundOutService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply()
		{
			TransactionId = result.TransactionId,
			CasinoTransId = result.CasinoTransid,
			Balance = result.Balance,
			Res = result.Res
		};
	}

	public override async Task<GameUrlReply> TP2GameUrl(GameUrlRequest request, ServerCallContext context)
	{
		var query = new TP2GameUrlQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode,
			GameCode: request.GameCode,
			IsMobile: request.IsMobile,
			Language: request.Language);

		var result = await tp2GameUrlService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new GameUrlReply()
		{
			IsPlayerExist = result.IsPlayerExist,
			GameUrl = result.GameUrl
		};
	}

	public override async Task<GameWagerReply> TP2GameWagerBetLog(GameWagerRequest request, ServerCallContext context)
	{
		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			StartTime: request.StartTime.ToDateTimeOffset(),
			EndTime: request.EndTime.ToDateTimeOffset(),
			Page: request.Page,
			PageSize: request.PageSize);

		var gameWagers = await tp2GameWagerBetLogService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ToArrayAsync(
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerResult>();

		var result = new GameWagerReply();
		if (gameWagers.Any())
		{
			result.Items.AddRange(gameWagers.Select(gw => new GameWagerInfo
			{
				ExternalRoundId = gw.ExternalRoundId,
				ExternalWagerId = gw.ExternalWagerId,
				ExternalGameId = gw.ExternalGameId,
				PlayerCode = gw.PlayerCode,
				IsClosed = gw.IsClosed,
				IsVoid = gw.IsVoid,
				IsFreeGame = gw.IsFreeGame,
				IsResettle = gw.IsResettle,
				ExternalCreatedOn = gw.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = gw.ExternalClosedOn.ToTimestamp(),
				Bet = gw.Bet,
				ValidBet = gw.ValidBet,
				Settle = gw.Settle,
				WinLoss = gw.WinLoss,
				Jackpot = gw.Jackpot,
				AdditionalData = gw.AdditionalData
			}));
		}

		return result;
	}

	public override async Task<GameWagerReply> TP2GameWagerBetLogByBettime(GameWagerRequest request, ServerCallContext context)
	{
		var query = new TP2GameWagerBetLogByBettimeQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			StartTime: request.StartTime.ToDateTimeOffset(),
			EndTime: request.EndTime.ToDateTimeOffset(),
			Page: request.Page,
			PageSize: request.PageSize);

		var gameWagers = await tp2GameWagerBetLogByBettimeService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ToArrayAsync(
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerResult>();

		var result = new GameWagerReply();
		if (gameWagers.Any())
		{
			result.Items.AddRange(gameWagers.Select(gw => new GameWagerInfo
			{
				ExternalRoundId = gw.ExternalRoundId,
				ExternalWagerId = gw.ExternalWagerId,
				ExternalGameId = gw.ExternalGameId,
				PlayerCode = gw.PlayerCode,
				IsClosed = gw.IsClosed,
				IsVoid = gw.IsVoid,
				IsFreeGame = gw.IsFreeGame,
				IsResettle = gw.IsResettle,
				ExternalCreatedOn = gw.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = gw.ExternalClosedOn.ToTimestamp(),
				Bet = gw.Bet,
				ValidBet = gw.ValidBet,
				Settle = gw.Settle,
				WinLoss = gw.WinLoss,
				Jackpot = gw.Jackpot,
				AdditionalData = gw.AdditionalData
			}));
		}

		return result;
	}

	public override async Task<GameWagerDetailReply> TP2GameWagerDetail(GameWagerDetailRequest request, ServerCallContext context)
	{
		var query = new TP2GameWagerDetailQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			ExternalWagerId: request.ExternalWagerId,
			ExternalCreatedOn: request.ExternalCreatedOn.ToDateTimeOffset(),
			Language: request.Language);

		var result = await tp2GameWagerDetailService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new GameWagerDetailReply()
		{
			GameWagerDetailUrl = result
		};
	}

	public override async Task<PlayerBalanceReply> TP2PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
	{
		var query = new TP2PlayerBalanceQuery(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode);

		var result = await tp2PlayerBalanceService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceReply()
		{
			IsPlayerExist = result.IsPlayerExist,
			Balance = result.Balance
		};
	}

	public override async Task<BaseReply> TP2PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
	{
		var command = new TP2PlayerCreateCommand(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode);

		await tp2PlayerCreateService.ExecuteAsync(
			command: command,
		   cancellationToken: context.CancellationToken)
		   .ConfigureAwait(false);

		return new BaseReply()
		{
			State = true
		};
	}

	public override async Task<BaseReply> TP2PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
	{
		var command = new TP2PlayerLogoutCommand(
			ApiUrl: new Uri(request.ApiUrl),
			ApiKey: request.ApiKey,
			ApiToken: request.ApiToken,
			PlayerCode: request.PlayerCode);

		await tp2PlayerLogoutService.ExecuteAsync(
			command: command,
		   cancellationToken: context.CancellationToken)
		   .ConfigureAwait(false);

		return new BaseReply()
		{
			State = true
		};
	}
}
