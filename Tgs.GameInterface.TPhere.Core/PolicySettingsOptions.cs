namespace Tgs.GameInterface.TPhere.Core;

public record PolicySettingsOptions
{
	/// <summary>
	/// The deposit retry count
	/// </summary>
	public int DepositRetryCount { get; set; }
	/// <summary>
	/// The deposit retry wait sec
	/// </summary>
	public int DepositRetryWaitSec { get; set; }
	/// <summary>
	/// The withdraw retry count
	/// </summary>
	public int WithdrawRetryCount { get; set; }
	/// <summary>
	/// The withdraw retry wait sec
	/// </summary>
	public int WithdrawRetryWaitSec { get; set; }
	/// <summary>
	/// The global retry count
	/// </summary>
	public int GlobalRetryCount { get; set; }
	/// <summary>
	/// The global retry wait sec
	/// </summary>
	public int GlobalRetryWaitSec { get; set; }
}
