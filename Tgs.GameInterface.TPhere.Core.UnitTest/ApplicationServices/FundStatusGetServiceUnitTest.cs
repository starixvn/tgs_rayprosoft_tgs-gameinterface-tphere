using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new FundStatusQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			OrderId: "D1777814779117309952");

		_ = fakeGameApiClient.TransactionAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			transactionId: Arg.Is(query.OrderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new FundStatusGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
