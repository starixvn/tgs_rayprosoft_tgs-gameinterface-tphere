using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.TPhere.Core;

namespace Tgs.GameInterface.TPhere.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
	/// <param name="_hostingEnvironment">The hosting environment.</param>
	/// <param name="policySettingsOptions">The policy settings options.</param>
	/// <returns></returns>
	[HttpGet]
	public async Task<object> GetAsync(
		[FromServices] IConfiguration configuration,
		[FromServices] IWebHostEnvironment _hostingEnvironment,
		[FromServices] IOptions<PolicySettingsOptions> policySettingsOptions)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);
		return new
		{
			Data = new
			{
				Environment.MachineName,
				HostName = hostName,
				HostIp = hostIp?.ToString() ?? string.Empty,
				Environment = _hostingEnvironment.EnvironmentName,
				OsVersion = $"{Environment.OSVersion}",
				Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
				ProcessCount = Environment.ProcessorCount,
				ReleaseNamespace = configuration.GetValue<string>("RELEASE_NAMESPACE"),
				PodIPAddress = configuration.GetValue<string>("POD_IP"),
				PolicySettingsOptions = policySettingsOptions.Value
			}
		};
	}
}
