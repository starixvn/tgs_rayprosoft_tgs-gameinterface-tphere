using System.Reflection;
using Tgs.GameInterface.TPhere.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services,
		Action<PolicySettingsOptions, IServiceProvider> policySettingsOptions)
		=> services
		.AddOptions<PolicySettingsOptions>()
		.Configure(policySettingsOptions)
		.Services
		.AddHttpClient<IGameApiClient, GameApiClient>()
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<ICryptographyUtensil, CryptographyUtensil>();
}
