namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2GameUrlQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode,
	string GameCode,
	bool IsMobile,
	string Language);
