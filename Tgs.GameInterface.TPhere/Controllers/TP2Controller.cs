using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Controllers;

[Route("[controller]")]
[ApiController]
public class TP2Controller : ControllerBase
{
	/// <summary>
	/// Funds the in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029",
	///				"orderId": "W1777815764054978560",
	///				"amount": 1
	///			}
	/// </remarks>
	[HttpPost("fund/in", Name = "TP2_FundIn")]
	public async ValueTask<FundResultViewModel> FundInAsync(
		[FromServices] IGetService<TP2FundInQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new TP2FundInQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			PlayerCode: source.PlayerCode,
			OrderId: source.OrderId,
			Amount: source.Amount);

		var result = await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new FundResultViewModel(
			TransactionId: result.TransactionId,
			CasinoTransId: result.CasinoTransid,
			Balance: result.Balance,
			Res: result.Res);
	}

	/// <summary>
	/// Funds the out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029",
	///				"orderId": "D1777814779117309952",
	///				"amount": 100
	///			}
	/// </remarks>
	[HttpPost("fund/out", Name = "TP2_FundOut")]
	public async ValueTask<FundResultViewModel> FundOutAsync(
		[FromServices] IGetService<TP2FundOutQuery, FundResult> service,
		FundViewModel source)
	{
		var query = new TP2FundOutQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			PlayerCode: source.PlayerCode,
			OrderId: source.OrderId,
			Amount: source.Amount);

		var result = await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new FundResultViewModel(
			TransactionId: result.TransactionId,
			CasinoTransId: result.CasinoTransid,
			Balance: result.Balance,
			Res: result.Res);
	}

	/// <summary>
	/// Games the wager bet log by bettime asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"StartTime": "2024-04-10T15:40:00+08:00",
	///				"EndTime": "2024-04-10T15:45:00+08:00",
	///				"Page": 1,
	///				"PageSize": 5000
	///			}
	/// </remarks>
	[HttpPost("game/wager/bettime", Name = "TP2_GameWagerBetLogByBettime")]
	public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerBetLogByBettimeAsync(
		[FromServices] IQueryService<TP2GameWagerBetLogByBettimeQuery, GameWagerResult> service,
		GameWagerViewModel source)
	{
		var query = new TP2GameWagerBetLogByBettimeQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			StartTime: source.StartTime,
			EndTime: source.EndTime,
			Page: source.Page,
			PageSize: source.PageSize);

		var gameWagers = await service.QueryAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync(
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerResult>();

		if (!gameWagers.Any())
		{
			yield break;
		}

		foreach (var gameWager in gameWagers)
		{
			yield return new GameWagerResultViewModel(
				ExternalRoundId: gameWager.ExternalRoundId,
				ExternalWagerId: gameWager.ExternalWagerId,
				ExternalGameId: gameWager.ExternalGameId,
				PlayerCode: gameWager.PlayerCode,
				IsClosed: gameWager.IsClosed,
				IsVoid: gameWager.IsVoid,
				IsFreeGame: gameWager.IsFreeGame,
				IsResettle: gameWager.IsResettle,
				ExternalCreatedOn: gameWager.ExternalCreatedOn,
				ExternalClosedOn: gameWager.ExternalClosedOn,
				Bet: gameWager.Bet,
				ValidBet: gameWager.ValidBet,
				Settle: gameWager.Settle,
				WinLoss: gameWager.WinLoss,
				Jackpot: gameWager.Jackpot,
				AdditionalData: gameWager.AdditionalData);
		}
	}

	/// <summary>
	/// Games the wager bet log asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"StartTime": "2024-04-10T15:30:00+08:00",
	///				"EndTime": "2024-04-10T16:00:00+08:00",
	///				"Page": 1,
	///				"PageSize": 5000
	///			}
	/// </remarks>
	[HttpPost("game/wager", Name = "TP2_GameWagerBetLog")]
	public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerBetLogAsync(
		[FromServices] IQueryService<TP2GameWagerBetLogQuery, GameWagerResult> service,
		GameWagerViewModel source)
	{
		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			StartTime: source.StartTime,
			EndTime: source.EndTime,
			Page: source.Page,
			PageSize: source.PageSize);

		var gameWagers = await service.QueryAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync(
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerResult>();

		if (!gameWagers.Any())
		{
			yield break;
		}

		foreach (var gameWager in gameWagers)
		{
			yield return new GameWagerResultViewModel(
				ExternalRoundId: gameWager.ExternalRoundId,
				ExternalWagerId: gameWager.ExternalWagerId,
				ExternalGameId: gameWager.ExternalGameId,
				PlayerCode: gameWager.PlayerCode,
				IsClosed: gameWager.IsClosed,
				IsVoid: gameWager.IsVoid,
				IsFreeGame: gameWager.IsFreeGame,
				IsResettle: gameWager.IsResettle,
				ExternalCreatedOn: gameWager.ExternalCreatedOn,
				ExternalClosedOn: gameWager.ExternalClosedOn,
				Bet: gameWager.Bet,
				ValidBet: gameWager.ValidBet,
				Settle: gameWager.Settle,
				WinLoss: gameWager.WinLoss,
				Jackpot: gameWager.Jackpot,
				AdditionalData: gameWager.AdditionalData);
		}
	}

	/// <summary>
	/// Games the wager detail asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"externalWagerId": "88062550",
	///				"externalCreatedOn": "2024-04-10T15:43:05+08:00",
	///				"language": "zh-cn"
	///			}
	/// </remarks>
	[HttpPost("game/wager/detail", Name = "TP2_GameWagerDetail")]
	public async ValueTask<string> GameWagerDetailAsync(
		[FromServices] IGetService<TP2GameWagerDetailQuery, string> service,
		GameWagerDetailViewModel source)
	{
		var query = new TP2GameWagerDetailQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			ExternalWagerId: source.ExternalWagerId,
			ExternalCreatedOn: source.ExternalCreatedOn,
			Language: source.Language);

		return await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			天降财神 at2_035
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029",
	///				"gameCode": "at2_035",
	///				"isMobile": false,
	///				"language": "zh-tw"
	///			}
	/// </remarks>
	[HttpPost("game/url", Name = "TP2_GameUrl")]
	public async ValueTask<GameUrlResultViewModel> GameUrlAsync(
		[FromServices] IGetService<TP2GameUrlQuery, GameUrlResult> service,
		GameUrlViewModel source)
	{
		var query = new TP2GameUrlQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			PlayerCode: source.PlayerCode,
			GameCode: source.GameCode,
			IsMobile: source.IsMobile,
			Language: source.Language);

		var result = await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new GameUrlResultViewModel(
			result.IsPlayerExist,
			result.GameUrl);
	}

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029"
	///			}
	/// </remarks>
	[HttpPost("player/balance", Name = "TP2_PlayerBalance")]
	public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
		[FromServices] IGetService<TP2PlayerBalanceQuery, PlayerBalanceResult> service,
		PlayerBalanceViewModel source)
	{
		var query = new TP2PlayerBalanceQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			PlayerCode: source.PlayerCode);

		var result = await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PlayerBalanceResultViewModel(
			result.IsPlayerExist,
			result.Balance);
	}

	/// <summary>
	/// Players the create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029"
	///			}
	/// </remarks>
	[HttpPost("player/create", Name = "TP2PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<TP2PlayerCreateCommand> service,
		PlayerCreateViewModel source)
		=> service
		.ExecuteAsync(
			command: new TP2PlayerCreateCommand(
				ApiUrl: source.ApiUrl,
				ApiKey: source.ApiKey,
				ApiToken: source.ApiToken,
				PlayerCode: source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the kick asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"playerCode": "4ucmqplxb029"
	///			}
	/// </remarks>
	[HttpPost("player/logout", Name = "TP2PlayerLogout")]
	public ValueTask PlayerKickAsync(
		[FromServices] ICommandService<TP2PlayerLogoutCommand> service,
		PlayerLogoutViewModel source)
		=> service
		.ExecuteAsync(
			command: new TP2PlayerLogoutCommand(
				ApiUrl: source.ApiUrl,
				ApiKey: source.ApiKey,
				ApiToken: source.ApiToken,
				PlayerCode: source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted);
}
