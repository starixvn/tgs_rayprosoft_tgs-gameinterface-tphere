using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;

public class TP2PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2PlayerBalanceQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029");

		_ = fakeGameApiClient.PlayerWalletAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			account: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new PlayerWalletResult(
				IsPlayerExist: true,
				Balance: 1.0m));

		var sut = new TP2PlayerBalanceGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(1.0m, actual.Balance);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2PlayerBalanceQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029");

		_ = fakeGameApiClient.PlayerWalletAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			account: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new PlayerWalletResult(
				IsPlayerExist: false,
				Balance: 0.0m));

		var sut = new TP2PlayerBalanceGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
	}
}
