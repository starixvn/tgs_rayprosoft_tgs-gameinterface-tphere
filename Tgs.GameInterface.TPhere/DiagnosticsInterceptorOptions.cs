using System.Diagnostics;

namespace Tgs.GameInterface.TPhere;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } =
		new ActivitySource($"Tgs.GameInterface.TPhere.DiagnosticsInterceptor");
}
