namespace Tgs.GameInterface.TPhere.Core.UnitTest;

public class CryptographyUtensilUnitTest
{
	[Fact]
	public void GameLink()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", "tp2" },
			{ "gamecode","at2_076" },
			{ "account","4ucmqplxb029" },
			{ "lang","zh-tw" },
			{ "platform", "web"},
			{ "sub_gamehall",string.Empty },
			{ "return_url",string.Empty },
			{ "is_free_trial",0.ToString() },
			{ "register_url",string.Empty },
			{ "room_id",string.Empty },
			{ "agent_id",string.Empty },
			{ "bounsmode",string.Empty }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "df9ff1af5c24dea800c13aa45fe10813";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void PlayerCreated()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "account","4ucmqplxb029" },
			{ "password","tgspassw0rd" },
			{ "nickname","4ucmqplxb029" }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "81370f5b31f175c15bf4a28115067506";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void PlayerLogout()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall", "tp2" },
			{ "account","4ucmqplxb029" }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "c7deb77edbe5c8581c11bebd9c4111e0";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void PlayerWallet()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "account", "4ucmqplxb029" },
			{ "gamehall", "tp2" }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "c7deb77edbe5c8581c11bebd9c4111e0";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TransactionDeposit()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "account","4ucmqplxb029" },
			{ "gamehall", "tp2" },
			{ "transaction_id", "d240409025030338" },
			{ "amount", 1.00m.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "0c9035be567247984ac3cb07f497d30c";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void TransactionWithdraw()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "account","4ucmqplxb029" },
			{ "gamehall", "tp2" },
			{ "transaction_id", "w240409025030338" },
			{ "amount", 1.00m.ToString("0.00") }
		};

		var key = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "863abdda80f8f0c2ba8dc74c6528753c";
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void BetLogPlayCheckA()
	{
		var apiKey = "6309572cf90658e17847a7b021723106";
		var parameters = new SortedDictionary<string, string>
		{
			{ "gamehall","tp2" },
			{ "betID","88062550" },
			{ "bet_time","2024-04-10 03:43:05"},
			{ "lang", "zh-cn"}
		};

		var key = string.Concat("betID", "=", parameters["betID"], "&", "bet_time", "=", parameters["bet_time"], "&", "gamehall", "=", parameters["gamehall"], "&", "lang", "=", parameters["lang"]);

		var sut = new CryptographyUtensil();

		var actual = sut.Md5Encrypt(key + apiKey);

		var expected = "942d505fc192672b2dd04b6c4c04f89f";
		Assert.Equal(expected, actual);
	}
}
