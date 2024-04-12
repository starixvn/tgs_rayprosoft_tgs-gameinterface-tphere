namespace Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
public record TP2PlayerLogoutCommand(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode);
