using System.Net;

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class PlayerCreatedUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";

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
			{ "account",account },
			{ "password","tgspassw0rd" },
			{ "nickname",account }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "81370f5b31f175c15bf4a28115067506";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["account"] = "4ucmqplxb029",
				["name"] = "4ucmqplxb029",
				["agent_account"] = "GPTEST",
				["agent_code"] = "hja0a",
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712650625
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/player").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		await sut.PlayerCreatedAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account);
	}

	[Fact]
	public async Task Player_account_duplicated()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";

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
			{ "account",account },
			{ "password","tgspassw0rd" },
			{ "nickname",account }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "81370f5b31f175c15bf4a28115067506";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

		var response = new JsonObject
		{
			["data"] = null,
			["status"] = new JsonObject
			{
				["code"] = 101,
				["message"] = "Player account duplicated.",
				["timestamp"] = 1712650676
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/player").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		await sut.PlayerCreatedAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account);
	}
}
