namespace Tgs.GameInterface.TPhere.ViewModels;

public record GameUrlViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode,
	string GameCode,
	bool IsMobile,
	string Language = "zh-tw");
