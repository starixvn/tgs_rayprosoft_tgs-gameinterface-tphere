using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
namespace Tgs.GameInterface.TPhere.ViewModels;
public record FundViewModel(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string PlayerCode,
	string OrderId,
	decimal Amount);
