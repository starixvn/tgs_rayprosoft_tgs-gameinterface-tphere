using System.Net;

using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.TPhere.Core.Exceptions;

namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class TransactionDepositUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "d240409025030338";
		var amount = 1.00m;

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
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "0c9035be567247984ac3cb07f497d30c";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

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
				["created_user"] = "GPTEST",
				["balance_before"] = "0.00",
				["balance_after"] = "1.0000",
				["casino_transid"] = "gh6vu6s3l326a4317ab1673fd",
				["currency"] = "CNY",
				["status"] = "success",
				["is_auto"] = false,
				["updated_at"] = "2024-04-09 04:33:27",
				["created_at"] = "2024-04-09 04:33:27"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712651607
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/deposit").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.TransactionDepositAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount);

		Assert.NotNull(actual);
		Assert.Equal("GPTEST_d240409025030338", actual.TransactionId);
		Assert.Equal("gh6vu6s3l326a4317ab1673fd", actual.CasinoTransid);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:33:27"), TimeSpan.FromHours(-4)), actual.UpdatedAt);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:33:27"), TimeSpan.FromHours(-4)), actual.CreatedAt);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "d240409025030338";
		var amount = 1.00m;

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
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "0c9035be567247984ac3cb07f497d30c";
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
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/deposit").ToString())
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
			() => sut.TransactionDepositAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
		/*
		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
		*/
	}

	[Fact]
	public async Task Transaction_ID_duplicated()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "d240409025030338";
		var amount = 1.00m;

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
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "0c9035be567247984ac3cb07f497d30c";
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
				["timestamp"] = 1712899299
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/deposit").ToString())
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
			() => sut.TransactionDepositAsync(
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
		var transactionId = "d123456789012345678901234567890";
		var amount = 1.00m;

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
		var handler = new MockHttpMessageHandler();
		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
			() => sut.TransactionDepositAsync(
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
		var transactionId = "d12";
		var amount = 1.00m;

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
		var handler = new MockHttpMessageHandler();
		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
			() => sut.TransactionDepositAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount).AsTask());
	}

	[Fact]
	public async Task Policy()
	{
		var apiUrl = new Uri("https://apidemo.tphere.com");
		var apiKey = "6309572cf90658e17847a7b021723106";
		var apiToken = "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb";
		var gamehall = "tp2";
		var account = "4ucmqplxb029";
		var transactionId = "d240409025030338";
		var amount = 1.00m;

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
			{ "account",account },
			{ "gamehall", gamehall },
			{ "transaction_id", transactionId },
			{ "amount", amount.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sign = "0c9035be567247984ac3cb07f497d30c";
		_ = fakeCryptographyUtensil.Md5Encrypt(
			source: Arg.Is(key + apiKey))
			.Returns(sign);

		parameters.Add("sign", sign);

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
				["created_user"] = "GPTEST",
				["balance_before"] = "0.00",
				["balance_after"] = "1.0000",
				["casino_transid"] = "gh6vu6s3l326a4317ab1673fd",
				["currency"] = "CNY",
				["status"] = "success",
				["is_auto"] = false,
				["updated_at"] = "2024-04-09 04:33:27",
				["created_at"] = "2024-04-09 04:33:27"
			},
			["status"] = new JsonObject
			{
				["code"] = 0,
				["message"] = "success",
				["timestamp"] = 1712651607
			}
		};
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/deposit").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
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
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/api/transaction/deposit").ToString())
				.WithHeaders("Authorization", apiToken)
				.WithExactFormData(parameters)
				.Respond(HttpStatusCode.OK, JsonContent.Create(response));

		var fakeHttpClient = handler.ToHttpClient();

		var sut = new GameApiClient(
			fakeLog,
			policySettingsOptions,
			fakeCryptographyUtensil,
			fakeHttpClient);

		var actual = await sut.TransactionDepositAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiToken: apiToken,
			gamehall: gamehall,
			account: account,
			transactionId: transactionId,
			amount: amount);

		Assert.NotNull(actual);
		Assert.Equal("GPTEST_d240409025030338", actual.TransactionId);
		Assert.Equal("gh6vu6s3l326a4317ab1673fd", actual.CasinoTransid);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:33:27"), TimeSpan.FromHours(-4)), actual.UpdatedAt);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-04-09 04:33:27"), TimeSpan.FromHours(-4)), actual.CreatedAt);
	}
}
