namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2PlayerCreateCommand(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode);
