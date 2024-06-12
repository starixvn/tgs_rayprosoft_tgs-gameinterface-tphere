using System.Net.Http.Json;
using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Web;
using Microsoft.Extensions.Options;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class BetLogUnitTest
{
	private readonly TimeZoneInfo _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");

	[Fact]
	public async Task Win()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 10, 03, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 10, 05, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "025adfa52c0078138a7046e8fe9893a9";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["reporttime"]= "2024-04-09 22:08:22",
						["hall"]= "gpk2",
						["category"]= "electronic",
						["rowid"]= "9110842",
						["game_name"]= "FaFaFa",
						["casino_account"]= "4ucmqplxb029",
						["betvalid"]= "1.0000",
						["betamount"]= "1.0000",
						["betresult"]= "4.6000",
						["bettime"]= "2024-04-09 22:04:21",
						["status"]= "1",
						["gameid"]= "am_5",
						["payout_time"]= "2024-04-09 22:04:21",
						["round"]= "9110842",
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["freegame"]= null,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 2,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 20
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712717283
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("electronic", actual.FirstOrDefault()!.Category);
		Assert.Equal("9110842", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("9110842", actual.FirstOrDefault()!.Round);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(4.6000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:21"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.Payout, actual.FirstOrDefault()!.Status);
		Assert.Equal("am_5", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Loss()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 10, 03, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 10, 05, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "025adfa52c0078138a7046e8fe9893a9";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["reporttime"]= "2024-04-09 22:08:22",
						["hall"]= "gpk2",
						["category"]= "electronic",
						["rowid"]= "9110848",
						["game_name"]= "FaFaFa",
						["casino_account"]= "4ucmqplxb029",
						["betvalid"]= "1.0000",
						["betamount"]= "1.0000",
						["betresult"]= "-1.0000",
						["bettime"]= "2024-04-09 22:04:39",
						["status"]= "1",
						["gameid"]= "am_5",
						["payout_time"]= "2024-04-09 22:04:39",
						["round"]= "9110848",
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["freegame"]= null,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 2,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 20
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712717283
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("electronic", actual.FirstOrDefault()!.Category);
		Assert.Equal("9110848", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("9110848", actual.FirstOrDefault()!.Round);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(-1.0000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:39"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.Payout, actual.FirstOrDefault()!.Status);
		Assert.Equal("am_5", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Win_Some()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 10, 03, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 10, 05, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "025adfa52c0078138a7046e8fe9893a9";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["reporttime"]= "2024-04-09 22:08:22",
						["hall"]= "gpk2",
						["category"]= "electronic",
						["rowid"]= "9110847",
						["game_name"]= "FaFaFa",
						["casino_account"]= "4ucmqplxb029",
						["betvalid"]= "1.0000",
						["betamount"]= "1.0000",
						["betresult"]= "0.6000",
						["bettime"]= "2024-04-09 22:04:35",
						["status"]= "1",
						["gameid"]= "am_5",
						["payout_time"]= "2024-04-09 22:04:35",
						["round"]= "9110847",
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["freegame"]= null,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 2,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 20
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712717283
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("electronic", actual.FirstOrDefault()!.Category);
		Assert.Equal("9110847", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("9110847", actual.FirstOrDefault()!.Round);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(0.6000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:04:35"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 22:08:22"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.Payout, actual.FirstOrDefault()!.Status);
		Assert.Equal("am_5", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}

	/// <summary>
	/// Buys the free game.
	/// 2024-04-10 遊戲商目前沒有支援Freegame，所以回應都是Null
	/// </summary>
	[Fact]
	public async Task Buy_Free_Game()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 14, 13, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 14, 14, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "025adfa52c0078138a7046e8fe9893a9";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["reporttime"]= "2024-04-10 02:17:10",
						["hall"]= "gpk2",
						["category"]= "electronic",
						["rowid"]= "88065450",
						["game_name"]= "777 More Cash",
						["casino_account"]= "4ucmqplxb029",
						["betvalid"]= "128.0000",
						["betamount"]= "128.0000",
						["betresult"]= "512.0000",
						["bettime"]= "2024-04-10 02:13:36",
						["status"]= "1",
						["gameid"]= "at2_029",
						["payout_time"]= "2024-04-10 02:13:36",
						["round"]= "88065450",
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["freegame"]= null,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 2,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 2
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712730364
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("electronic", actual.FirstOrDefault()!.Category);
		Assert.Equal("88065450", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("88065450", actual.FirstOrDefault()!.Round);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(128.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(128.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(512.0000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 02:13:36"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 02:13:36"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 02:17:10"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.Payout, actual.FirstOrDefault()!.Status);
		Assert.Equal("at2_029", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}

	/// <summary>
	/// Frees the game.
	/// 2024-04-10 遊戲商目前沒有支援Freegame，所以回應都是Null
	/// </summary>
	[Fact]
	public async Task Free_Game()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 15, 42, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 15, 43, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "91cd19ea538a68da1027ba48869637d8";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["reporttime"]= "2024-04-10 03:45:59",
						["hall"]= "gpk2",
						["category"]= "electronic",
						["rowid"]= "88062549",
						["game_name"]= "Fortune God Coming",
						["casino_account"]= "4ucmqplxb029",
						["betvalid"]= "3.0000",
						["betamount"]= "3.0000",
						["betresult"]= "38.4000",
						["bettime"]= "2024-04-10 03:42:26",
						["status"]= "1",
						["gameid"]= "at2_035",
						["payout_time"]= "2024-04-10 03:42:26",
						["round"]= "88062549",
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["freegame"]= null,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 30,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 30
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712735468
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("electronic", actual.FirstOrDefault()!.Category);
		Assert.Equal("88062549", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("88062549", actual.FirstOrDefault()!.Round);
		Assert.Equal("4ucmqplxb029", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(3.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(3.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(38.4000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 03:42:26"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 03:42:26"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-10 03:45:59"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.Payout, actual.FirstOrDefault()!.Status);
		Assert.Equal("at2_035", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}

	[Fact]
	public async Task Unsettled()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var startTime = new DateTimeOffset(2024, 04, 10, 15, 42, 00, TimeSpan.FromHours(8));
		var endTime = new DateTimeOffset(2024, 04, 10, 15, 43, 00, TimeSpan.FromHours(8));
		var page = 1;
		var pageSize = 5000;

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", gamehall },
			{ "start_time", TimeZoneInfo.ConvertTime(startTime.DateTime,_timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "end_time", TimeZoneInfo.ConvertTime(endTime.DateTime, _timeZone).ToString("yyyy-MM-dd HH:mm:ss")},
			{ "page", page.ToString() },
			{ "page_size", pageSize.ToString() }
		};

		var sign = "91cd19ea538a68da1027ba48869637d8";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["page_result"] = new JsonArray
				{
					new JsonObject
					{
						["hall"]= "gpk2",
						["category"]= "live",
						["rowid"]= "70281207668500004",
						["casino_account"]= "4umv37wchzks",
						["game_name"]= "Lucky Wheel",
						["betvalid"]= "0",
						["betamount"]= "1",
						["betresult"]= "0",
						["bettime"]= "2024-06-12 02:38:01",
						["payout_time"]= null,
						["reporttime"]= "2024-06-12 02:42:35",
						["status"]= "0",
						["gameid"]= "ga_tP",
						["round"]= "840aa001240612143756tP001134352luckyRoulette",
						["freegame"]= null,
						["pca_contribute"]= "0",
						["pca_win"]= "0",
						["revenue"]= "0",
						["trace"]= false,
						["bettype"]= null,
						["gameresult"]= null,
						["betinfo"]= null
					}
				},
				["current_page"] = 1,
				["from"] = 1,
				["to"] = 30,
				["per_page"] = 5000,
				["last_page"] = 1,
				["total"] = 30
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712735468
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, "/api/betlog")
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 startTime: startTime,
			 endTime: endTime,
			 page: page,
			 pageSize: pageSize)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("gpk2", actual.FirstOrDefault()!.Hall);
		Assert.Equal("live", actual.FirstOrDefault()!.Category);
		Assert.Equal("70281207668500004", actual.FirstOrDefault()!.Rowid);
		Assert.Equal("840aa001240612143756tP001134352luckyRoulette", actual.FirstOrDefault()!.Round);
		Assert.Equal("4umv37wchzks", actual.FirstOrDefault()!.CasinoAccount);
		Assert.Equal(0.0000m, actual.FirstOrDefault()!.Betvalid);
		Assert.Equal(1.0000m, actual.FirstOrDefault()!.BetAmount);
		Assert.Equal(0.0000m, actual.FirstOrDefault()!.Betresult);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaContribute);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.PcaWin);
		Assert.Equal(0.0m, actual.FirstOrDefault()!.Revenue);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-06-12 02:38:01"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Bettime);
		Assert.Equal(DateTimeOffset.UnixEpoch, actual.FirstOrDefault()!.PayoutTime);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-06-12 02:42:35"), TimeSpan.FromHours(-4)), actual.FirstOrDefault()!.Reporttime);
		Assert.Equal(BetLogStatus.NoPayout, actual.FirstOrDefault()!.Status);
		Assert.Equal("ga_tP", actual.FirstOrDefault()!.GameId);
		Assert.False(actual.FirstOrDefault()!.Trace);
		Assert.Equal(0, actual.FirstOrDefault()!.Freegame);
		Assert.Equal(response["data"]!["page_result"]!.AsArray().FirstOrDefault()!.ToJsonString(), actual.FirstOrDefault()!.AdditionalData);
	}
}
