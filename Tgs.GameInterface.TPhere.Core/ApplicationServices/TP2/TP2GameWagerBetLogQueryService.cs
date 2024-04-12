using System.Runtime.CompilerServices;
using Tgs.GameInterface.TPhere.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

internal class TP2GameWagerBetLogQueryService(
	IGameApiClient client) : IQueryService<TP2GameWagerBetLogQuery, GameWagerResult>
{
	public async IAsyncEnumerable<GameWagerResult> QueryAsync(TP2GameWagerBetLogQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var betLogs = await client.BetLogAsync(
			apiUrl: query.ApiUrl,
			apiKey: query.ApiKey,
			apiToken: query.ApiToken,
			gamehall: Gamehall.TP2,
			startTime: query.StartTime,
			endTime: query.EndTime,
			page: query.Page,
			pageSize: query.PageSize,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<BetLogResult>();

		if (!betLogs.Any())
		{
			yield break;
		}

		foreach (var betLog in betLogs.Distinct())
		{
			var externalCreatedOn = betLog.Bettime;
			var externalClosedOn = DateTimeOffset.UnixEpoch;
			var bet = betLog.BetAmount;
			var validBet = betLog.Betvalid;
			var settle = 0.0m;
			var winLoss = 0.0m;
			var jackpot = 0.0m;
			var isVoid = false;
			var isClosed = false;
			var isFreeGame = false;
			var isResettle = false;
			if (betLog.Status != BetLogStatus.NoPayout)
			{
				isClosed = true;
				isVoid = betLog.Status == BetLogStatus.Void;
				isFreeGame = betLog.Freegame != 0;
				isResettle = betLog.Status == BetLogStatus.PayoutReset;
				externalClosedOn = betLog.PayoutTime;
				jackpot = betLog.PcaWin;
				winLoss = betLog.Betresult;
				settle = winLoss + validBet;
			}

			yield return new GameWagerResult(
				betLog.Round,
				betLog.Rowid,
				betLog.GameId,
				betLog.CasinoAccount,
				isClosed,
				isVoid,
				isFreeGame,
				isResettle,
				externalCreatedOn,
				externalClosedOn,
				bet,
				validBet,
				settle,
				winLoss,
				jackpot,
				betLog.AdditionalData);
		}
	}
}
