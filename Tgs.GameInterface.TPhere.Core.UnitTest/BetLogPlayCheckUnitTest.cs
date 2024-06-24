using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.TPhere.Core.Exceptions;
using Tgs.GameInterface.TPhere.Core.Extensions;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class BetLogPlayCheckUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var betId = "88062550";
		var betTime = new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4));
		var lang = "zh-cn";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall",gamehall },
			{ "betID",betId },
			{ "bet_time", betTime.DatetimeConvertToString()},
			{ "lang", lang}
		};

		var key = string.Concat("betID", "=", parameters["betID"], "&", "bet_time", "=", parameters["bet_time"], "&", "gamehall", "=", parameters["gamehall"], "&", "lang", "=", parameters["lang"]);
		var sign = "942d505fc192672b2dd04b6c4c04f89f";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog/playcheck"));
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
				["playCheck"] = "https:\\/\\/m-client.yahutech.com\\/GUI\\/slot_fortunegod\\/query\\/index.html?WagersId=88062550&GameId=035&multiple=1&Platform=aHR0cHM6Ly9tLWFwaS55YWh1dGVjaC5jb20=&PlatId=JIG&Lang=zh-CN"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712890625
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, builder.ToString())
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogPlayCheckAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 betId: betId,
			 betTime: betTime,
			 lang: lang);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task Betlog_not_exist()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var betId = "88062550";
		var betTime = new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4));
		var lang = "zh-cn";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall",gamehall },
			{ "betID",betId },
			{ "bet_time", betTime.DatetimeConvertToString()},
			{ "lang", lang}
		};

		var key = string.Concat("betID", "=", parameters["betID"], "&", "bet_time", "=", parameters["bet_time"], "&", "gamehall", "=", parameters["gamehall"], "&", "lang", "=", parameters["lang"]);
		var sign = "942d505fc192672b2dd04b6c4c04f89f";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog/playcheck"));
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
				["playCheck"] = null
			},
			["status"] = new JsonObject
			{
				["code"] = 401,
				["message"] = "Betlog not exist.",
				["timestamp"] = 1712890754
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, builder.ToString())
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ErrorCodeException>(() => sut.BetLogPlayCheckAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			betId: betId,
			betTime: betTime,
			lang: lang).AsTask());
	}

	[Fact]
	public async Task Sign_invalid()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var betId = "88062550";
		var betTime = new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4));
		var lang = "zh-cn";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall",gamehall },
			{ "betID",betId },
			{ "bet_time", betTime.DatetimeConvertToString()},
			{ "lang", lang}
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = "86ea2d86818475b03ca370ebab9454a6";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Any<string>())
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog/playcheck"));
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
				["playCheck"] = null
			},
			["status"] = new JsonObject
			{
				["code"] = 2,
				["message"] = "Sign invalid.",
				["timestamp"] = 1712891092
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, builder.ToString())
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ErrorCodeException>(() => sut.BetLogPlayCheckAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			betId: betId,
			betTime: betTime,
			lang: lang).AsTask());
	}

	[Fact]
	public async Task Policy()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var betId = "88062550";
		var betTime = new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4));
		var lang = "zh-cn";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall",gamehall },
			{ "betID",betId },
			{ "bet_time", betTime.DatetimeConvertToString()},
			{ "lang", lang}
		};

		var key = string.Concat("betID", "=", parameters["betID"], "&", "bet_time", "=", parameters["bet_time"], "&", "gamehall", "=", parameters["gamehall"], "&", "lang", "=", parameters["lang"]);
		var sign = "942d505fc192672b2dd04b6c4c04f89f";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/betlog/playcheck"));
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
				["playCheck"] = "https:\\/\\/m-client.yahutech.com\\/GUI\\/slot_fortunegod\\/query\\/index.html?WagersId=88062550&GameId=035&multiple=1&Platform=aHR0cHM6Ly9tLWFwaS55YWh1dGVjaC5jb20=&PlatId=JIG&Lang=zh-CN"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712890625
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Get, builder.ToString())
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(new JsonObject
				{
					["data"] = null,
					["status"] = new JsonObject
					{
						["code"] = 7,
						["message"] = "Exceed api call limit.",
						["timestamp"] = 1719216902
					}
				}));
		_ = handler.Expect(HttpMethod.Get, builder.ToString())
				.WithHeaders("Authorization", apiToken)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.BetLogPlayCheckAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 betId: betId,
			 betTime: betTime,
			 lang: lang);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}
}
