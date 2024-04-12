using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class TransactionUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var transactionId = "d240409025030338";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var sign = "022a5b6fd891038fe0016a7800d355b0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, $"/api/transaction/{transactionId}"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["sign"] = sign;
		builder.Query = query.ToString();

		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["transaction_id"] = "GPTEST_d240409025030338",
				["type"] = "player",
				["source_account"] = "GPTEST",
				["destination_account"] = "4ucmqplxb029",
				["amount"] = "1.00",
				["casino"] = "gpk2",
				["updated_at"] = "2024-04-09 04:33:27",
				["created_at"] = "2024-04-09 04:33:27",
				["created_user"] = "GPTEST",
				["balance_before"] = "0.00",
				["balance_after"] = "1.0000",
				["casino_transid"] = "gh6vu6s3l326a4317ab1673fd",
				["currency"] = "CNY",
				["status"] = "success",
				["is_auto"] = false,
				["error_message"] = null
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712654451
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

		var actual = await sut.TransactionAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			transactionId: transactionId);

		Assert.True(actual);
	}

	[Fact]
	public async Task Transaction_ID_not_exist()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var transactionId = "d240409025030338";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var sign = "022a5b6fd891038fe0016a7800d355b0";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(apiKey))
			.Returns(sign);

		var builder = new UriBuilder(new Uri(apiUrl, $"/api/transaction/{transactionId}"));
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["sign"] = sign;
		builder.Query = query.ToString();

		var response = new JsonObject
		{
			["data"] = null,
			["status"] = new JsonObject
			{
				["code"] = 303,
				["message"] = "Transaction ID not exist.",
				["timestamp"] = 1712654514
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

		var actual = await sut.TransactionAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			transactionId: transactionId);

		Assert.False(actual);
	}

	[Fact]
	public async Task The_maximum_length_of_this_string_30()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var transactionId = "d123456789012345678901234567890";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();
		var handler = new MockHttpMessageHandler();
		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
			() => sut.TransactionAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			transactionId: transactionId).AsTask());
	}

	[Fact]
	public async Task The_minimum_length_of_this_string_4()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var transactionId = "d12";

		var policySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			DepositRetryCount = 1,
			DepositRetryWaitSec = 1,
			WithdrawRetryCount = 1,
			WithdrawRetryWaitSec = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();
		var handler = new MockHttpMessageHandler();
		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
			() => sut.TransactionAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			transactionId: transactionId).AsTask());
	}
}
