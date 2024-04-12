using System.Net;

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class GameLinkUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var gamecode = "at2_076";
		var account = "4ucmqplxb029";
		var lang = "zh-tw";
		var isMobile = false;

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
			{ "gamecode",gamecode },
			{ "account",account },
			{ "lang",lang },
			{ "platform",isMobile? "mobile":"web"},
			{ "sub_gamehall",string.Empty },
			{ "return_url",string.Empty },
			{ "is_free_trial",0.ToString() },
			{ "register_url",string.Empty },
			{ "room_id",string.Empty },
			{ "agent_id",string.Empty },
			{ "bounsmode",string.Empty }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = "df9ff1af5c24dea800c13aa45fe10813";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/game/game-link"));
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
				["url"] = "https:\\/\\/m-api.yahutech.com\\/Login?Account=hja0a4ucmqplxb029&GameId=076&Lang=zh-CN&AgentId=xh4gc&Key=f2e6478ca5ad5f0dec8cff8386567388e32231013fd6"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712653143
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

		var actual = await sut.GameLinkAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 gamecode: gamecode,
			 account: account,
			 lang: lang,
			 isMobile: isMobile,
			 sub_gamehall: string.Empty,
			 return_url: string.Empty,
			 isFreeTrial: 0,
			 registerUrl: string.Empty,
			 roomId: string.Empty,
			 agentId: string.Empty,
			 bounsmode: string.Empty);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.NotEmpty(actual.GameUrl);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var gamecode = "at2_076";
		var account = "4ucmqplxb029";
		var lang = "zh-tw";
		var isMobile = false;

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
			{ "gamecode",gamecode },
			{ "account",account },
			{ "lang",lang },
			{ "platform",isMobile? "mobile":"web"},
			{ "sub_gamehall",string.Empty },
			{ "return_url",string.Empty },
			{ "is_free_trial",0.ToString() },
			{ "register_url",string.Empty },
			{ "room_id",string.Empty },
			{ "agent_id",string.Empty },
			{ "bounsmode",string.Empty }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var sign = "df9ff1af5c24dea800c13aa45fe10813";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/game/game-link"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var parameter in parameters)
		{
			query[parameter.Key] = parameter.Value;
		}

		query["sign"] = sign;
		builder.Query = query.ToString();
		var response = new JsonObject
		{
			["data"] = null,
			["status"] = new JsonObject
			{
				["code"] = 102,
				["message"] = "Player does not exist.",
				["timestamp"] = 1712647728
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

		var actual = await sut.GameLinkAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 gamecode: gamecode,
			 account: account,
			 lang: lang,
			 isMobile: isMobile,
			 sub_gamehall: string.Empty,
			 return_url: string.Empty,
			 isFreeTrial: 0,
			 registerUrl: string.Empty,
			 roomId: string.Empty,
			 agentId: string.Empty,
			 bounsmode: string.Empty);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Empty(actual.GameUrl);
	}
}
