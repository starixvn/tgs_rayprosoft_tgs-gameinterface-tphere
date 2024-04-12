namespace Tgs.GameInterface.TPhere.Core;

public enum BetLogStatus
{
	/// <summary>
	/// The no payout
	/// 未派彩
	/// </summary>
	NoPayout = 0,

	/// <summary>
	/// The payout
	/// 已派彩
	/// </summary>
	Payout = 1,

	/// <summary>
	/// The payout reset
	/// 已派彩但注單修改過
	/// </summary>
	PayoutReset = 2,

	/// <summary>
	/// The cancel
	/// 撤單
	/// </summary>
	Void = 3
}
