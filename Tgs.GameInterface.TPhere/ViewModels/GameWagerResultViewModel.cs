namespace Tgs.GameInterface.TPhere.ViewModels;

public record GameWagerResultViewModel(
	string ExternalRoundId,
	string ExternalWagerId,
	string ExternalGameId,
	string PlayerCode,
	bool IsClosed,
	bool IsVoid,
	bool IsFreeGame,
	bool IsResettle,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal WinLoss,
	decimal Jackpot,
	string AdditionalData);
