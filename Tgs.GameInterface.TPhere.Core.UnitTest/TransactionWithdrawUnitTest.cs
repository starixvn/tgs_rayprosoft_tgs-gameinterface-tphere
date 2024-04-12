using System.Net;

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.TPhere.Core.Exceptions;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class TransactionWithdrawUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "w240409025030338";
		var amount = 1.00m;

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
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "863abdda80f8f0c2ba8dc74c6528753c";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

		var response = new JsonObject
		{
			["data"] = new JsonObject
			{
				["transaction_id"] = "GPTEST_w240409025030338",
				["type"] = "player",
				["source_account"] = "4ucmqplxb029",
				["destination_account"] = "GPTEST",
				["amount"] = "1.00",
				["casino"] = "gpk2",
				["created_user"] = "GPTEST",
				["balance_before"] = "1.0000",
				["balance_after"] = "0.0000",
				["casino_transid"] = "w240409025030338",
				["currency"] = "CNY",
				["status"] = "success",
				["is_auto"] = false,
				["updated_at"] = "2024-04-09 04:45:22",
				["created_at"] = "2024-04-09 04:45:22"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712652323
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/withdraw").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.TransactionWithdrawAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount);

		Assert.NotNull(actual);
		Assert.Equal("GPTEST_w240409025030338", actual.TransactionId);
		Assert.Equal("w240409025030338", actual.CasinoTransid);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:45:22"), TimeSpan.FromHours(-4)), actual.UpdatedAt);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:45:22"), TimeSpan.FromHours(-4)), actual.CreatedAt);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "w240409025030338";
		var amount = 1.00m;

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
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "863abdda80f8f0c2ba8dc74c6528753c";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

		var response = new JsonObject
		{
			["data"] = null,
			["status"] = new JsonObject
			{
				["code"] = 102,
				["message"] = "Player does not exist.",
				["timestamp"] = 1712649739
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/withdraw").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ErrorCodeException>(
			() => sut.TransactionWithdrawAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
	}

	[Fact]
	public async Task Transaction_ID_duplicated()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "w240409025030338";
		var amount = 1.00m;

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
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "863abdda80f8f0c2ba8dc74c6528753c";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

		var response = new JsonObject
		{
			["data"] = null,
			["status"] = new JsonObject
			{
				["code"] = 301,
				["message"] = "Transaction ID duplicated.",
				["timestamp"] = 1712649739
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/withdraw").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ErrorCodeException>(
			() => sut.TransactionWithdrawAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
	}

	[Fact]
	public async Task The_maximum_length_of_this_string_30()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "w123456789012345678901234567890";
		var amount = 1.00m;

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
			() => sut.TransactionWithdrawAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
	}

	[Fact]
	public async Task The_minimum_length_of_this_string_4()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "w12";
		var amount = 1.00m;

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
			() => sut.TransactionWithdrawAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
	}
}
