namespace Tgs.GameInterface.TPhere.Core.Models;
public record TransactionResult(
	string Type,
	string TransactionId,
	string SourceAccount,
	string DestinationAccount,
	decimal Amount,
	string Casino,
	DateTimeOffset UpdatedAt,
	DateTimeOffset CreatedAt,
	decimal BalanceBefore,
	decimal BalanceAfter,
	string CasinoTransid,
	string Status,
	string Res);
