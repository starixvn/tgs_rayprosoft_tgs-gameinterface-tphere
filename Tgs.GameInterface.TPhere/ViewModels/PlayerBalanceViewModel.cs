namespace Tgs.GameInterface.TPhere.ViewModels;

public record PlayerBalanceViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode);
