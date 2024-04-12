using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
namespace Tgs.GameInterface.TPhere.Core.ApplicationServices;
public record FundStatusQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	string OrderId);
