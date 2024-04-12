using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Core.Extensions;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;

public class TP2FundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2FundInQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029",
			OrderId: "w240409025030338",
			Amount: 1.0m);

		var json = new JsonObject
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
		};
		_ = fakeGameApiClient.TransactionWithdrawAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			account: Arg.Is(query.PlayerCode),
			transactionId: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new TransactionResult(
				Type: json["type"]!.ToString(),
				TransactionId: json["transaction_id"]!.ToString(),
				SourceAccount: json["source_account"]!.ToString(),
				DestinationAccount: json["destination_account"]!.ToString(),
				Amount: decimal.Parse(json["amount"]!.ToString()),
				Casino: json["casino"]!.ToString(),
				UpdatedAt: json["updated_at"]!.ToString().StringConvertToDatetime(),
				CreatedAt: json["created_at"]!.ToString().StringConvertToDatetime(),
				BalanceBefore: decimal.Parse(json["balance_before"]!.ToString()),
				BalanceAfter: decimal.Parse(json["balance_after"]!.ToString()),
				CasinoTransid: json["casino_transid"]!.ToString(),
				Status: json["status"]!.ToString(),
				Res: json.ToJsonString()));

		var sut = new TP2FundInGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal("w240409025030338", actual.TransactionId);
		Assert.Equal("w240409025030338", actual.CasinoTransid);
		Assert.Equal(0.0m, actual.Balance);
		Assert.Equal(json.ToJsonString(), actual.Res);
	}
}
