using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.TPhere.Core.ApplicationServices.TP2;
using Tgs.GameInterface.TPhere.ViewModels;
using Tgs.ServiceUtility;
using Tgs.GameInterface.TPhere.Core.ApplicationServices;

namespace Tgs.GameInterface.TPhere.Controllers;
[Route("[controller]")]
[ApiController]
public class TPhereController : ControllerBase
{
	/// <summary>
	/// Funds the status asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///			{
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"orderId": "D1777814779117309952"
	///			}
	/// </remarks>
	[HttpPost("fund/Status", Name = "FundStatus")]
	public async ValueTask<bool> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, bool> service,
		FundStatusViewModel source)
	{
		var query = new FundStatusQuery(
			ApiUrl: source.ApiUrl,
			ApiKey: source.ApiKey,
			ApiToken: source.ApiToken,
			OrderId: source.OrderId);

		return await service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}
}
