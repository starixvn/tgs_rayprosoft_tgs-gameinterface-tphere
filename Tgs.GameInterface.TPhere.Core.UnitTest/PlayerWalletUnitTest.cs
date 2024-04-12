using System.Net;

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class PlayerWalletUnitTest
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
			{ "account", account },
			{ "gamehall", gamehall }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "c7deb77edbe5c8581c11bebd9c4111e0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/player/wallet"));
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
				["gamehall"] = "gpk2",
				["balance"] = "0.00"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712650961
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

		var actual = await sut.PlayerWalletAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 account: account);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
	}

	[Fact]
	public async Task Player_does_not_exist()
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
			{ "account", account },
			{ "gamehall", gamehall }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "c7deb77edbe5c8581c11bebd9c4111e0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/player/wallet"));
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
				["timestamp"] = 1712645407
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

		var actual = await sut.PlayerWalletAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 account: account);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
	}
}
