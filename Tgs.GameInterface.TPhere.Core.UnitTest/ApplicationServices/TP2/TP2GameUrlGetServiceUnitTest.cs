using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;

public class TP2GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameUrlQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029",
			GameCode: "at2_035",
			IsMobile: false,
			Language: "en");

		_ = fakeGameApiClient.GameLinkAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			gamecode: Arg.Is(query.GameCode),
			account: Arg.Is(query.PlayerCode),
			lang: Arg.Is(query.Language),
			isMobile: Arg.Is(query.IsMobile),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new GameLinkResult(
				IsPlayerExist: true,
				GameUrl: "https:\\/\\/m-api.yahutech.com\\/Login?Account=hja0a4ucmqplxb029&GameId=076&Lang=zh-CN&AgentId=xh4gc&Key=f2e6478ca5ad5f0dec8cff8386567388e32231013fd6"));

		var sut = new TP2GameUrlGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.NotEmpty(actual.GameUrl);
	}

	[Fact]
	public async Task Player_does_not_exist()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameUrlQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029",
			GameCode: "at2_035",
			IsMobile: false,
			Language: "en");

		_ = fakeGameApiClient.GameLinkAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			gamecode: Arg.Is(query.GameCode),
			account: Arg.Is(query.PlayerCode),
			lang: Arg.Is(query.Language),
			isMobile: Arg.Is(query.IsMobile),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new GameLinkResult(
				IsPlayerExist: false,
				GameUrl: string.Empty));

		var sut = new TP2GameUrlGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Empty(actual.GameUrl);
	}
}
