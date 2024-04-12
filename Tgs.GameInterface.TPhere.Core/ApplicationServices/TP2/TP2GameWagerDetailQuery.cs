namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2GameWagerDetailQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string ExternalWagerId,
	DateTimeOffset ExternalCreatedOn,
	string Language);
