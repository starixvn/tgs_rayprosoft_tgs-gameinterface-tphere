namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2GameWagerBetLogByBettimeQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime,
	int Page = 1,
	int PageSize = 5000);
