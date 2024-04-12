using Tgs.GameInterface.TPhere.Core.Models;

namespace Tgs.GameInterface.TPhere.Core.ApplicationServices;
public record GameUrlResult(
	bool IsPlayerExist,
	string GameUrl);
