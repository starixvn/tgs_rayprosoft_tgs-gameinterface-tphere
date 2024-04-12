using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.UnitTest.ApplicationServices.TP2;
public class TP2GameWagerDetailGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new TP2GameWagerDetailQuery(
			ApiUrl: new Uri("https://apidemo.tphere.com"),
			ApiKey: "6309572cf90658e17847a7b021723106",
			ApiToken: "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
			ExternalWagerId: "88062550",
			ExternalCreatedOn: new DateTimeOffset(DateTime.Parse("2024-04-10 03:43:05"), TimeSpan.FromHours(-4)),
			Language: "zh-cn");

		_ = fakeGameApiClient.BetLogPlayCheckAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiToken: Arg.Is(query.ApiToken),
			gamehall: Arg.Is(Gamehall.TP2),
			betId: Arg.Is(query.ExternalWagerId),
			betTime: Arg.Is(query.ExternalCreatedOn),
			lang: Arg.Is(query.Language),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns("https:\\/\\/m-client.yahutech.com\\/GUI\\/slot_fortunegod\\/query\\/index.html?WagersId=88062550&GameId=035&multiple=1&Platform=aHR0cHM6Ly9tLWFwaS55YWh1dGVjaC5jb20=&PlatId=JIG&Lang=zh-CN");

		var sut = new TP2GameWagerDetailGetService(
			fakeGameApiClient);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}
}
