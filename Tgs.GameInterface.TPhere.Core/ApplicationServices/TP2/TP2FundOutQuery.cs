namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2FundOutQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode,
	string OrderId,
	decimal Amount);
