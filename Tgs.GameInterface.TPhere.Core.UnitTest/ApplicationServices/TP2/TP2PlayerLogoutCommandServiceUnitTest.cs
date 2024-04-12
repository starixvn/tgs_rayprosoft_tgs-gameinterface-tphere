using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;

public class TP2PlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var command = new TP2PlayerLogoutCommand(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			PlayerCode: "4ucmqplxb029");

		var sut = new TP2PlayerLogoutCommandService(
			fakeGameApiClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameApiClient
			.Received(1)
			.PlayerLogoutAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			apiKey: Arg.Is(command.ApiKey),
			apiToken: Arg.Is(command.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			account: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
