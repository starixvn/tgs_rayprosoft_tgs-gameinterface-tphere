namespace Tgs.GameInterface.TPhere.ViewModels;

public record PlayerCreateViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode);
