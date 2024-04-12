using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;

public class TP2GameWagerBetLogQueryServiceUnitTest
{
	[Fact]
	public async Task Win()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-5),
			EndTime: DateTimeOffset.UtcNow,
			Page: 1,
			PageSize: 5000);

		var betLog = new BetLogResult(
			Hall: "gpk2",
			Category: "electronic",
			Rowid: "9110842",
			Round: "9110842",
			CasinoAccount: "4ucmqplxb029",
			Betvalid: 1.0000m,
			BetAmount: 1.0000m,
			Betresult: 4.6000m,
			PcaContribute: 0.0m,
			PcaWin: 0.0m,
			Revenue: 0.0m,
			Bettime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)),
			PayoutTime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)),
			Reporttime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)),
			Status: BetLogStatus.Payout,
			GameId: "am_5",
			Trace: false,
			Freegame: 0,
			AdditionalData: new JsonObject
			{
				["reporttime"] = "2024-04-09 22:08:22",
				["hall"] = "gpk2",
				["category"] = "electronic",
				["rowid"] = "9110842",
				["game_name"] = "FaFaFa",
				["casino_account"] = "4ucmqplxb029",
				["betvalid"] = "1.0000",
				["betamount"] = "1.0000",
				["betresult"] = "4.6000",
				["bettime"] = "2024-04-09 22:04:21",
				["status"] = "1",
				["gameid"] = "am_5",
				["payout_time"] = "2024-04-09 22:04:21",
				["round"] = "9110842",
				["pca_contribute"] = "0",
				["pca_win"] = "0",
				["revenue"] = "0",
				["trace"] = false,
				["freegame"] = null,
				["bettype"] = null,
				["gameresult"] = null,
				["betinfo"] = null
			}.ToJsonString());

		_ = fakeGameApiClient.BetLogAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			startTime: Arg.Is(query.StartTime),
			endTime: Arg.Is(query.EndTime),
			page: Arg.Is(query.Page),
			pageSize: Arg.Is(query.PageSize),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new BetLogResult[] { betLog }.ToAsyncEnumerable());

		var sut = new TP2GameWagerBetLogQueryService(
			fakeGameApiClient);

		var actual = await sut.QueryAsync(query)
			.ToArrayAsync()
			?? Enumerable.Empty<GameWagerResult>();

		Assert.NotNull(actual);
		Assert.Equal("9110842", actual.FirstOrDefault()!.ExternalRoundId);
		Assert.Equal("9110842", actual.FirstOrDefault()!.ExternalWagerId);
		Assert.Equal("am_5", actual.FirstOrDefault()!.ExternalGameId);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.PlayerCode);
		Assert.True(actual.FirstOrDefault()!.IsClosed);
		Assert.False(actual.FirstOrDefault()!.IsVoid);
		Assert.False(actual.FirstOrDefault()!.IsFreeGame);
		Assert.False(actual.FirstOrDefault()!.IsResettle);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalClosedOn);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Bet);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.ValidBet);
		Assert.Equal(5.6000m, actual.FirstOrDefault()!.Settle);
		Assert.Equal(4.6m, actual.FirstOrDefault()!.WinLoss);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Jackpot);
		Assert.Equal(betLog.AdditionalData, actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Loss()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-5),
			EndTime: DateTimeOffset.UtcNow,
			Page: 1,
			PageSize: 5000);

		var betLog = new BetLogResult(
			Hall: "gpk2",
			Category: "electronic",
			Rowid: "9110848",
			Round: "9110848",
			CasinoAccount: "4ucmqplxb029",
			Betvalid: 1.0000m,
			BetAmount: 1.0000m,
			Betresult: -1.0000m,
			PcaContribute: 0.0m,
			PcaWin: 0.0m,
			Revenue: 0.0m,
			Bettime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)),
			PayoutTime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)),
			Reporttime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)),
			Status: BetLogStatus.Payout,
			GameId: "am_5",
			Trace: false,
			Freegame: 0,
			AdditionalData: new JsonObject
			{
				["reporttime"] = "2024-04-09 22:08:22",
				["hall"] = "gpk2",
				["category"] = "electronic",
				["rowid"] = "9110848",
				["game_name"] = "FaFaFa",
				["casino_account"] = "4ucmqplxb029",
				["betvalid"] = "1.0000",
				["betamount"] = "1.0000",
				["betresult"] = "-1.0000",
				["bettime"] = "2024-04-09 22:04:39",
				["status"] = "1",
				["gameid"] = "am_5",
				["payout_time"] = "2024-04-09 22:04:39",
				["round"] = "9110848",
				["pca_contribute"] = "0",
				["pca_win"] = "0",
				["revenue"] = "0",
				["trace"] = false,
				["freegame"] = null,
				["bettype"] = null,
				["gameresult"] = null,
				["betinfo"] = null
			}.ToJsonString());

		_ = fakeGameApiClient.BetLogAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			startTime: Arg.Is(query.StartTime),
			endTime: Arg.Is(query.EndTime),
			page: Arg.Is(query.Page),
			pageSize: Arg.Is(query.PageSize),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new BetLogResult[] { betLog }.ToAsyncEnumerable());

		var sut = new TP2GameWagerBetLogQueryService(
			fakeGameApiClient);

		var actual = await sut.QueryAsync(query)
			.ToArrayAsync()
			?? Enumerable.Empty<GameWagerResult>();

		Assert.NotNull(actual);
		Assert.Equal("9110848", actual.FirstOrDefault()!.ExternalRoundId);
		Assert.Equal("9110848", actual.FirstOrDefault()!.ExternalWagerId);
		Assert.Equal("am_5", actual.FirstOrDefault()!.ExternalGameId);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.PlayerCode);
		Assert.True(actual.FirstOrDefault()!.IsClosed);
		Assert.False(actual.FirstOrDefault()!.IsVoid);
		Assert.False(actual.FirstOrDefault()!.IsFreeGame);
		Assert.False(actual.FirstOrDefault()!.IsResettle);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalClosedOn);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Bet);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.ValidBet);
		Assert.Equal(0, actual.FirstOrDefault()!.Settle);
		Assert.Equal(-1.0000m, actual.FirstOrDefault()!.WinLoss);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Jackpot);
		Assert.Equal(betLog.AdditionalData, actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Win_Some_Betamount_1_Betresult_06()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-5),
			EndTime: DateTimeOffset.UtcNow,
			Page: 1,
			PageSize: 5000);

		var betLog = new BetLogResult(
			Hall: "gpk2",
			Category: "electronic",
			Rowid: "9110847",
			Round: "9110847",
			CasinoAccount: "4ucmqplxb029",
			Betvalid: 1.0000m,
			BetAmount: 1.0000m,
			Betresult: 0.6000m,
			PcaContribute: 0.0m,
			PcaWin: 0.0m,
			Revenue: 0.0m,
			Bettime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)),
			PayoutTime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)),
			Reporttime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)),
			Status: BetLogStatus.Payout,
			GameId: "am_5",
			Trace: false,
			Freegame: 0,
			AdditionalData: new JsonObject
			{
				["reporttime"] = "2024-04-09 22:08:22",
				["hall"] = "gpk2",
				["category"] = "electronic",
				["rowid"] = "9110847",
				["game_name"] = "FaFaFa",
				["casino_account"] = "4ucmqplxb029",
				["betvalid"] = "1.0000",
				["betamount"] = "1.0000",
				["betresult"] = "0.6000",
				["bettime"] = "2024-04-09 22:04:35",
				["status"] = "1",
				["gameid"] = "am_5",
				["payout_time"] = "2024-04-09 22:04:35",
				["round"] = "9110847",
				["pca_contribute"] = "0",
				["pca_win"] = "0",
				["revenue"] = "0",
				["trace"] = false,
				["freegame"] = null,
				["bettype"] = null,
				["gameresult"] = null,
				["betinfo"] = null
			}.ToJsonString());

		_ = fakeGameApiClient.BetLogAsync(
			apiUrl: Arg.Is(query.ApiUrl),
				apiKey: Arg.Is(query.ApiKey),
				apiToken: Arg.Is(query.ApiToken),
				gamehall: Arg.Is(Gamehall.TP2),
				startTime: Arg.Is(query.StartTime),
				endTime: Arg.Is(query.EndTime),
				page: Arg.Is(query.Page),
				pageSize: Arg.Is(query.PageSize),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BetLogResult[] { betLog
	}.ToAsyncEnumerable());

		var sut = new TP2GameWagerBetLogQueryService(
			fakeGameApiClient);

		var actual = await sut.QueryAsync(query)
			.ToArrayAsync()
			?? Enumerable.Empty<GameWagerResult>();

		Assert.NotNull(actual);
		Assert.Equal("9110847", actual.FirstOrDefault()!.ExternalRoundId);
		Assert.Equal("9110847", actual.FirstOrDefault()!.ExternalWagerId);
		Assert.Equal("am_5", actual.FirstOrDefault()!.ExternalGameId);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.PlayerCode);
		Assert.True(actual.FirstOrDefault()!.IsClosed);
		Assert.False(actual.FirstOrDefault()!.IsVoid);
		Assert.False(actual.FirstOrDefault()!.IsFreeGame);
		Assert.False(actual.FirstOrDefault()!.IsResettle);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalClosedOn);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Bet);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.ValidBet);
		Assert.Equal(1.6m, actual.FirstOrDefault()!.Settle);
		Assert.Equal(0.6000m, actual.FirstOrDefault()!.WinLoss);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Jackpot);
		Assert.Equal(betLog.AdditionalData, actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Win_Some_Betamount_3_Betresult_Negative_2_01()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-5),
			EndTime: DateTimeOffset.UtcNow,
			Page: 1,
			PageSize: 5000);

		var betLog = new BetLogResult(
			Hall: "gpk2",
			Category: "electronic",
			Rowid: "88062550",
			Round: "88062550",
			CasinoAccount: "4ucmqplxb029",
			Betvalid: 3.0000m,
			BetAmount: 3.0000m,
			Betresult: -2.1000m,
			PcaContribute: 0.0m,
			PcaWin: 0.0m,
			Revenue: 0.0m,
			Bettime: new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4)),
			PayoutTime: new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4)),
			Reporttime: new DateTimeOffset(DateTime.Parse("2024-04-10 03:46:40"), TimeSpan.FromHours(-4)),
			Status: BetLogStatus.Payout,
			GameId: "at2_035",
			Trace: false,
			Freegame: 0,
			AdditionalData: new JsonObject
			{
				["reporttime"] = "2024-04-10 03:46:40",
				["hall"] = "gpk2",
				["category"] = "electronic",
				["rowid"] = "88062550",
				["game_name"] = "Fortune God Coming",
				["casino_account"] = "4ucmqplxb029",
				["betvalid"] = "3.0000",
				["betamount"] = "3.0000",
				["betresult"] = "-2.1000",
				["bettime"] = "2024-04-09 22:04:35",
				["status"] = "1",
				["gameid"] = "at2_035",
				["payout_time"] = "2024-04-09 22:04:35",
				["round"] = "88062550",
				["pca_contribute"] = "0",
				["pca_win"] = "0",
				["revenue"] = "0",
				["trace"] = false,
				["freegame"] = null,
				["bettype"] = null,
				["gameresult"] = null,
				["betinfo"] = null
			}.ToJsonString());

		_ = fakeGameApiClient.BetLogAsync(
			apiUrl: Arg.Is(query.ApiUrl),
				apiKey: Arg.Is(query.ApiKey),
				apiToken: Arg.Is(query.ApiToken),
				gamehall: Arg.Is(Gamehall.TP2),
				startTime: Arg.Is(query.StartTime),
				endTime: Arg.Is(query.EndTime),
				page: Arg.Is(query.Page),
				pageSize: Arg.Is(query.PageSize),
				cancellationToken: Arg.Any<CancellationToken>())
				.Returns(new BetLogResult[] { betLog
	}.ToAsyncEnumerable());

		var sut = new TP2GameWagerBetLogQueryService(
			fakeGameApiClient);

		var actual = await sut.QueryAsync(query)
			.ToArrayAsync()
			?? Enumerable.Empty<GameWagerResult>();

		Assert.NotNull(actual);
		Assert.Equal("88062550", actual.FirstOrDefault()!.ExternalRoundId);
		Assert.Equal("88062550", actual.FirstOrDefault()!.ExternalWagerId);
		Assert.Equal("at2_035", actual.FirstOrDefault()!.ExternalGameId);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.PlayerCode);
		Assert.True(actual.FirstOrDefault()!.IsClosed);
		Assert.False(actual.FirstOrDefault()!.IsVoid);
		Assert.False(actual.FirstOrDefault()!.IsFreeGame);
		Assert.False(actual.FirstOrDefault()!.IsResettle);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.ExternalClosedOn);
		Assert.Equal(3.0000m, actual.FirstOrDefault()!.Bet);
		Assert.Equal(3.0000m, actual.FirstOrDefault()!.ValidBet);
		Assert.Equal(0.9000m, actual.FirstOrDefault()!.Settle);
		Assert.Equal(-2.1000m, actual.FirstOrDefault()!.WinLoss);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Jackpot);
		Assert.Equal(betLog.AdditionalData, actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Distinct()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerBetLogQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-5),
			EndTime: DateTimeOffset.UtcNow,
			Page: 1,
			PageSize: 5000);

		var betLog = new BetLogResult(
			Hall: "gpk2",
			Category: "electronic",
			Rowid: "9110842",
			Round: "9110842",
			CasinoAccount: "4ucmqplxb029",
			Betvalid: 1.0000m,
			BetAmount: 1.0000m,
			Betresult: 4.6000m,
			PcaContribute: 0.0m,
			PcaWin: 0.0m,
			Revenue: 0.0m,
			Bettime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)),
			PayoutTime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)),
			Reporttime: new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)),
			Status: BetLogStatus.Payout,
			GameId: "am_5",
			Trace: false,
			Freegame: 0,
			AdditionalData: new JsonObject
			{
				["reporttime"] = "2024-04-09 22:08:22",
				["hall"] = "gpk2",
				["category"] = "electronic",
				["rowid"] = "9110842",
				["game_name"] = "FaFaFa",
				["casino_account"] = "4ucmqplxb029",
				["betvalid"] = "1.0000",
				["betamount"] = "1.0000",
				["betresult"] = "4.6000",
				["bettime"] = "2024-04-09 22:04:21",
				["status"] = "1",
				["gameid"] = "am_5",
				["payout_time"] = "2024-04-09 22:04:21",
				["round"] = "9110842",
				["pca_contribute"] = "0",
				["pca_win"] = "0",
				["revenue"] = "0",
				["trace"] = false,
				["freegame"] = null,
				["bettype"] = null,
				["gameresult"] = null,
				["betinfo"] = null
			}.ToJsonString());

		_ = fakeGameApiClient.BetLogAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			startTime: Arg.Is(query.StartTime),
			endTime: Arg.Is(query.EndTime),
			page: Arg.Is(query.Page),
			pageSize: Arg.Is(query.PageSize),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new BetLogResult[] { betLog, betLog }.ToAsyncEnumerable());

		var sut = new TP2GameWagerBetLogQueryService(
			fakeGameApiClient);

		var actual = await sut.QueryAsync(query)
			.ToArrayAsync()
			?? Enumerable.Empty<GameWagerResult>();

		Assert.NotNull(actual);
		Assert.Single(actual);
	}
}
