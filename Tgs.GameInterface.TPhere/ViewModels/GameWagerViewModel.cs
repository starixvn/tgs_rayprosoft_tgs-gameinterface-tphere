namespace Tgs.GameInterface.TPhere.ViewModels;

public record GameWagerViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime,
	int Page = 1,
	int PageSize = 5000);
