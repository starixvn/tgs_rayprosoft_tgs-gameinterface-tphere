namespace Tgs.GameInterface.TPhere.ViewModels;

public record PlayerLogoutViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode);
