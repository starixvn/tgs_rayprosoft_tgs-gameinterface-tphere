namespace Tgs.GameInterface.TPhere.ViewModels;

public record GameWagerDetailViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string ExternalWagerId,
	DateTimeOffset ExternalCreatedOn,
	string Language);
