using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Web;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;
public class PlayerIsGamingUnitTest
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
			{ "account", account },
			{ "gamehall", gamehall }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "c7deb77edbe5c8581c11bebd9c4111e0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/player/is-gaming"));
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
				["is_gaming"] = true
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

		var actual = await sut.PlayerIsGamingAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 account: account);

		Assert.True(actual);
	}

	[Fact]
	public async Task Policy()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";

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
			{ "account", account },
			{ "gamehall", gamehall }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "c7deb77edbe5c8581c11bebd9c4111e0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, "/api/player/is-gaming"));
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
				["is_gaming"] = true
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

		var actual = await sut.PlayerIsGamingAsync(
			 apiUrl: apiUrl,
			 apiKey: apiKey,
			 apiToken: apiToken,
			 gamehall: gamehall,
			 account: account);

		Assert.True(actual);
	}
}
